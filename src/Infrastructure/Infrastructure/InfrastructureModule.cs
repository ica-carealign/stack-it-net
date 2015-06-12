using System;

using Autofac;

using Ica.StackIt.Core;
using Ica.StackIt.Core.Entities;
using Ica.StackIt.Infrastructure.Mongo;

using MongoDB.Driver;

namespace Ica.StackIt.Infrastructure
{
	public class InfrastructureModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<MongoDatabaseConfiguration>().As<IDatabaseConfiguration>();
			builder.Register(context => new MongoClient(context.Resolve<IDatabaseConfiguration>().ConnectionString));

			builder.Register(context =>
			{
				var client = context.Resolve<MongoClient>();
				string connectionString = context.Resolve<IDatabaseConfiguration>().ConnectionString;
				var mongoUrl = new MongoUrl(connectionString);
				MongoServer server = client.GetServer();
				MongoDatabase database = server.GetDatabase(mongoUrl.DatabaseName);
				return database;
			});

			RegisterRepository<Stack>(builder);
			RegisterRepository<Instance>(builder);
			RegisterRepository<AwsProfile>(builder);
			RegisterRepository<Product>(builder);
			RegisterRepository<BaseImage>(builder);
			RegisterRepository<IPRange>(builder);
			RegisterRepository<ResourceLedger>(builder);
			RegisterRepository<Schedule>(builder);

			builder.RegisterType<SystemClock>().As<IClock>();
		}

		private static void RegisterRepository<T>(ContainerBuilder builder) where T : IEntity<Guid>
		{
			// new QuiescingRepository(new ValidatingRepository(new MongoRepository<T>()))
			builder.RegisterType<MongoRepository<T>>();

			builder.RegisterType<ValidatingRepository<T>>()
			       .WithParameter(
				       (info, context) => info.ParameterType == typeof (IRepository<T>),
				       (info, context) => context.Resolve<MongoRepository<T>>());

			builder.RegisterType<QuiescingRepository<T>>()
			       .WithParameter(
				       (info, context) => info.ParameterType == typeof (IRepository<T>),
				       (info, context) => context.Resolve<ValidatingRepository<T>>())
			       .As<IRepository<T>>();

			builder.Register(context => context.Resolve<MongoDatabase>().GetCollection<T>(typeof (T).Name))
			       .As<MongoCollection<T>>();
		}
	}

	public static class ContainerBuilderExtensions
	{
		public static ContainerBuilder RegisterInfrastructure(this ContainerBuilder self)
		{
			self.RegisterModule<InfrastructureModule>();
			return self;
		}
	}
}