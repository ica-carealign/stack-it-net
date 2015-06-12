using System;
using System.Collections.Generic;
using System.Linq;

using Ica.StackIt.Core;

namespace Ica.StackIt.Infrastructure
{
	public class ValidatingRepository<TEntity> : IRepository<TEntity>
	{
		private readonly IRepository<TEntity> _inner;

		public ValidatingRepository(IRepository<TEntity> inner)
		{
			_inner = inner;
		}

		public void Add(TEntity item)
		{
			ModelValidator.ValidateObject(item);
			_inner.Add(item);
		}

		public void Update(TEntity item)
		{
			ModelValidator.ValidateObject(item);
			_inner.Update(item);
		}

		public void Delete(TEntity item)
		{
			_inner.Delete(item);
		}

		public void Delete(Guid id)
		{
			_inner.Delete(id);
		}

		public void DeleteAll()
		{
			_inner.DeleteAll();
		}

		public IEnumerable<TEntity> FindAll()
		{
			return _inner.FindAll();
		}

		public IQueryable<TEntity> CreateQuery()
		{
			return _inner.CreateQuery();
		}

		public TEntity Find(Guid id)
		{
			return _inner.Find(id);
		}
	}
}