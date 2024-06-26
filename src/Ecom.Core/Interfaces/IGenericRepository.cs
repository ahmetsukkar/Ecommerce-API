﻿using Ecom.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
	public interface IGenericRepository<T> where T : BaseEntity<int>
	{
		Task<IEnumerable<T>> GetAllAsync();

		Task<T> GetByIdAsync(int id);
		Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

		Task AddAsync(T entity);

		Task UpdateAsync(int id, T entity);

		Task<bool> DeleteAsync(int id);

		Task<int> CountAsync();
	}
}
