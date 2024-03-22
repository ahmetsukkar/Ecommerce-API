using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity<int>
	{
		private readonly ApplicationDbContext _context;

		public GenericRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task<int> CountAsync() 
			=> await _context.Set<T>().CountAsync();

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _context.Set<T>().AsNoTracking().ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
		{
			var query = _context.Set<T>().AsQueryable();

			//apply any includes
			foreach (var item in includes)
			{
				query = query.Include(item);
			}
			return await query.ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
		{
			IQueryable<T> query = _context.Set<T>().Where(x => x.Id == id);

			//apply includes
			foreach (var item in includes)
			{
				query = query.Include(item);
			}
			return await query.FirstOrDefaultAsync();
		}

		public async Task UpdateAsync(int id, T entity)
		{
			var currentEntity = await _context.Set<T>().FindAsync(id);
			if (currentEntity is not null)
			{
				_context.Set<T>().Update(currentEntity);
				await _context.SaveChangesAsync();
			}
		}
	}
}
