using Autofac;

using FluentAssertions;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Infrastructure;
using Ica.StackIt.Infrastructure.Mongo;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests.Autofac
{
	internal class InfrastructureModuleTests
	{
		IContainer Container { get; set; }

		[SetUp]
		public void SetUp()
		{
			var builder = new ContainerBuilder();
			builder.RegisterInfrastructure();
			Container = builder.Build();
		}

		[Test]
		public void CanResolveInstanceRepository()
		{
			var repository = Container.Resolve<IRepository<Instance>>();
			repository.Should().NotBeNull();
			repository.Should().BeOfType<QuiescingRepository<Instance>>();
		}

		[Test]
		public void CanResolveStackRepository()
		{
			var repository = Container.Resolve<IRepository<Stack>>();
			repository.Should().NotBeNull();
			repository.Should().BeOfType<QuiescingRepository<Stack>>();
		}
	}
}