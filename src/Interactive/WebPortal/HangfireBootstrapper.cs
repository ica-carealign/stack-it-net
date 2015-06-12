using System.Messaging;
using System.Web.Hosting;

using Autofac;

using Hangfire;

using Ica.StackIt.Application.Configuration;
using Ica.StackIt.Application.Hangfire;

namespace Ica.StackIt.Interactive.WebPortal
{
	public class HangfireBootstrapper : IRegisteredObject
	{
		public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

		private readonly object _lockObject = new object();
		private bool _started;

		private BackgroundJobServer _unorderedJobServer;
		private BackgroundJobServer _orderedJobServer;

		private IContainer Container { get; set; }

		public HangfireBootstrapper()
		{
			Container = new ContainerGenerator().GetContainer();
		}

		public void Start()
		{
			lock (_lockObject)
			{
				if (_started) return;
				_started = true;

				HostingEnvironment.RegisterObject(this);

				var hfConfiguration = Container.Resolve<IHangfireConfiguration>();
				var queueConfiguration = Container.Resolve<IQueueingConfiguration>();
				string orderedQueue = queueConfiguration.OrderedCommandQueue;
				string unorderedQueue = queueConfiguration.UnorderedCommandQueue;

				GlobalConfiguration.Configuration.UseAutofacActivator(Container);
				GlobalConfiguration.Configuration.UseSqlServerStorage(hfConfiguration.ConnectionString)
					.UseMsmqQueues(queueConfiguration.OrderedCommandQueue, Constants.OrderedQueueName)
					.UseMsmqQueues(queueConfiguration.UnorderedCommandQueue, Constants.UnorderedQueueName);

				var unorderedOptions = SetupQueue(unorderedQueue, Constants.UnorderedQueueName);
				_unorderedJobServer = new BackgroundJobServer(unorderedOptions);

				var orderedOptions = SetupQueue(orderedQueue, Constants.OrderedQueueName);
				_orderedJobServer = new BackgroundJobServer(orderedOptions);
			}
		}

		public void Stop()
		{
			lock (_lockObject)
			{
				if (_unorderedJobServer != null)
				{
					_unorderedJobServer.Dispose();
				}

				if (_orderedJobServer != null)
				{
					_orderedJobServer.Dispose();
				}

				HostingEnvironment.UnregisterObject(this);
			}
		}

		void IRegisteredObject.Stop(bool immediate)
		{
			Stop();
		}

		private static BackgroundJobServerOptions SetupQueue(string queuePath, string queueName)
		{
			CreateQueueIfNecessary(queuePath);

			var options = new BackgroundJobServerOptions
			{
				ServerName = queueName,
				Queues = new[] { queueName }
			};

			return options;
		}

		private static void CreateQueueIfNecessary(string queuePath)
		{
			if (!MessageQueue.Exists(queuePath))
			{
				MessageQueue.Create(queuePath, true);
			}
		}
	}
}