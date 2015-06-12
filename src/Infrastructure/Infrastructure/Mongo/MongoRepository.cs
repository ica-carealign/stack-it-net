using System;

using Ica.StackIt.Core;

using MongoDB.Driver;

namespace Ica.StackIt.Infrastructure.Mongo
{
	public class MongoRepository<TEntity> : MongoRepositoryBase<TEntity> where TEntity : IEntity<Guid>
	{
		public MongoRepository(MongoCollection<TEntity> collection) : base(collection) {}
	}
}