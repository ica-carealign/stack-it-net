using System;
using System.Collections.Generic;
using System.Linq;

namespace Ica.StackIt.Core
{
	public interface IRepository<TEntity> : IRepository<TEntity, Guid> {}

	public interface IRepository<TEntity, in TId> where TId : struct
	{
		void Add(TEntity item);

		void Update(TEntity item);

		void Delete(TEntity item);

		void Delete(TId id);

		void DeleteAll();

		IEnumerable<TEntity> FindAll();

		IQueryable<TEntity> CreateQuery();

		TEntity Find(TId id);
	}
}