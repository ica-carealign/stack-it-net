using System;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

using NUnit.Framework;

namespace Ica.StackIt.Testing.IntegrationTests
{
	[TestFixture]
	public abstract class MongoDbTest
	{
		#region Setup/Teardown

		[SetUp]
		public virtual void SetUp()
		{
			MongoClient = new MongoClient(_connectionString);
			MongoServer = MongoClient.GetServer();
			MongoServer.DropDatabase(_databaseName);
			MongoDatabase = MongoServer.GetDatabase(_databaseName);
			MongoDatabase.SetProfilingLevel(ProfilingLevel.All);
		}

		[TearDown]
		public void TearDown()
		{
			MongoCollection<BsonDocument> profilerCollection = MongoDatabase.GetCollection("system.profile");
			foreach (BsonDocument item in profilerCollection.FindAll())
			{
				Console.WriteLine(item.ToJson(new JsonWriterSettings {Indent = true}));
			}
		}

		#endregion

		private const string _connectionString = "mongodb://127.0.0.1";
		private const string _databaseName = "MongoDbTest";

		private MongoClient MongoClient { get; set; }
		private MongoServer MongoServer { get; set; }
		protected MongoDatabase MongoDatabase { get; set; }

		protected MongoCollection<TEntity> GetCollection<TEntity>()
		{
			string collectionName = typeof (TEntity).Name;
			return MongoDatabase.GetCollection<TEntity>(collectionName);
		}
	}
}