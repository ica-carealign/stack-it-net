using System;
using System.Collections.Generic;
using System.Linq;

using Ica.StackIt.Core;

namespace Ica.StackIt.Infrastructure
{
	public class QuiescingRepository<T> : IRepository<T>
	{
		private readonly IRepository<T> _innerRepository;
		private readonly bool _canCast;

		public QuiescingRepository(IRepository<T> innerRepository)
		{
			_innerRepository = innerRepository;
			// figure out the cast once and then skip it later if possible
			_canCast = typeof (IQuiesceable).IsAssignableFrom(typeof (T));
		}

		private void QuiesceEntity(T entity)
		{
			if (_canCast)
			{
				((IQuiesceable) entity).Quiesce();
			}
		}

		public void Add(T item)
		{
			QuiesceEntity(item);
			_innerRepository.Add(item);
		}

		public void Update(T item)
		{
			QuiesceEntity(item);
			_innerRepository.Update(item);
		}

		public void Delete(T item)
		{
			_innerRepository.Delete(item);
		}

		public void Delete(Guid id)
		{
			_innerRepository.Delete(id);
		}

		public void DeleteAll()
		{
			_innerRepository.DeleteAll();
		}

		public IEnumerable<T> FindAll()
		{
			return _innerRepository.FindAll();
		}

		public IQueryable<T> CreateQuery()
		{
			return _innerRepository.CreateQuery();
		}

		public T Find(Guid id)
		{
			return _innerRepository.Find(id);
		}
	}
}