using System;
using System.Collections.Generic;
using System.Linq;

using Ica.StackIt.Core;

using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Ica.StackIt.Infrastructure.Mongo
{
	public abstract class MongoRepositoryBase<TEntity> : IRepository<TEntity>
		where TEntity : IEntity<Guid>
	{
		private readonly MongoCollection<TEntity> _collection;

		protected MongoRepositoryBase(MongoCollection<TEntity> collection)
		{
			_collection = collection;
		}

		public void Add(TEntity item)
		{
			_collection.Insert(item);
		}

		public void Update(TEntity item)
		{
			if (item is IVersioned)
			{
				UpdateWithVersion(item);
			}
			else
			{
				_collection.Save(item);
			}
		}

		private void UpdateWithVersion(TEntity item)
		{
			var versioned = (IVersioned) item;
			int origVersion = versioned.Version;
			versioned.Version++;

			// where id = ? and (version = ? or version is null)
			IMongoQuery query = Query.And(
				Query<TEntity>.EQ(dbItem => dbItem.Id, item.Id),
				Query.Or(
					Query.EQ("Version", origVersion),
					Query.NotExists("Version"))
				);

			WriteConcernResult result = _collection.Update(query, Update<TEntity>.Replace(item));

			if (result.DocumentsAffected == 0)
			{
				versioned.Version--;
				string message = string.Format(
					"Attempted to update {0} (Id={1},Version={2}) but it has been updated by another transaction or deleted.",
					typeof (TEntity).Name, item.Id, origVersion);
				throw new StaleObjectException(message);
			}
		}

		public void Delete(TEntity item)
		{
			Delete(item.Id);
		}

		public void Delete(Guid id)
		{
			IMongoQuery query = Query<TEntity>.EQ(e => e.Id, id);
			_collection.Remove(query);
		}

		public void DeleteAll()
		{
			_collection.RemoveAll();
		}

		public IEnumerable<TEntity> FindAll()
		{
			return _collection.FindAll();
		}

		public IQueryable<TEntity> CreateQuery()
		{
			return _collection.AsQueryable();
		}

		public TEntity Find(Guid id)
		{
			IMongoQuery query = Query<TEntity>.EQ(e => e.Id, id);
			return _collection.FindOne(query);
		}
	}
}