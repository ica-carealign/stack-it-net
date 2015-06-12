using System;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Builder;

using Castle.DynamicProxy;

using Microsoft.AspNet.SignalR;

namespace Ica.StackIt.Interactive.WebPortal
{
	/// <summary>
	///     This code has been inlined from:
	///     https://github.com/autofac/Autofac.SignalR/compare/master...kingpong:support-hub-lifetimescope
	///     Hopefully it will be merged into Autofac.Integration.SignalR soon and we can drop this.
	/// </summary>
	public static class SignalRIntegration
	{
		private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

		/// <summary>
		///     Registers a <see cref="Hub" /> class whose dependencies are resolved within a lifetime scope.
		///     Any components that are registered with InstancePerLifetimeScope will be created for
		///     and disposed after every hub operation, since SignalR creates a new instance of the hub for
		///     each operation.
		/// </summary>
		/// <typeparam name="T">The <see cref="Hub" /> class to be registered.</typeparam>
		/// <param name="builder">The container builder.</param>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle>
			RegisterHubWithLifetimeScope<T>(this ContainerBuilder builder) where T : Hub
		{
			var options = new ProxyGenerationOptions {Hook = new HubDisposalProxyGenerationHook()};
			Type proxyType = _proxyGenerator.ProxyBuilder.CreateClassProxyType(typeof (T), new Type[] {}, options);
			builder.RegisterType(proxyType).ExternallyOwned();

			return builder.Register(ctx =>
			{
				var lifetimeScope = ctx.Resolve<ILifetimeScope>();
				ILifetimeScope newScope = lifetimeScope.BeginLifetimeScope();

				// The proxy class has a constructor that looks like:
				//   ctor(IInterceptor[], origArg1, origArg2, ...)
				var interceptor = new HubDisposalInterceptor(newScope);
				var interceptorParam = new TypedParameter(typeof (IInterceptor[]), new IInterceptor[] {interceptor});
				return (T) newScope.Resolve(proxyType, interceptorParam);
			}).As<T>().ExternallyOwned();
		}
	}

	internal class HubDisposalProxyGenerationHook : IProxyGenerationHook
	{
		/// <summary>
		///     Enables the proxy class created in
		///     <see cref="SignalRIntegration.RegisterHubWithLifetimeScope{T}" />
		///     to intercept <b>only</b> the virtual Dispose(bool) method.
		/// </summary>
		// ReSharper disable once EmptyConstructor
		public HubDisposalProxyGenerationHook() {}

		public void MethodsInspected() {}

		public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) {}

		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}

			// only intercept "protected void Dispose(bool)"
			return methodInfo.IsFamily
			       && methodInfo.ReturnType == typeof (void)
			       && methodInfo.Name == "Dispose"
			       && methodInfo.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] {typeof (bool)});
		}

		/// <summary>
		///     Returns true if the other object is of the same class.
		///     This allows Castle DynamicProxy to reuse the same generated class.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			// This class contains no data so every instance is equivalent to every other instance.
			return obj.GetType() == GetType();
		}

		/// <summary>
		///     Returns the same value for every instance of this class.
		///     Implemented for good measure to match the behavior of <see cref="Equals" />
		/// </summary>
		public override int GetHashCode()
		{
			// This class contains no data so every instance is equivalent to every other instance.
			return GetType().GetHashCode();
		}
	}

	internal class HubDisposalInterceptor : IInterceptor
	{
		private readonly ILifetimeScope _scope;

		/// <summary>
		///     Disposes the implicit
		///     <param name="scope">lifetime scope</param>
		///     when the
		///     <see cref="Hub" /> that owns it is disposed.
		/// </summary>
		public HubDisposalInterceptor(ILifetimeScope scope)
		{
			_scope = scope;
		}

		public void Intercept(IInvocation invocation)
		{
			if (invocation == null)
			{
				throw new ArgumentNullException("invocation");
			}

			try
			{
				invocation.Proceed();
			}
			finally
			{
				_scope.Dispose();
			}
		}
	}
}