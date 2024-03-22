using Ecom.Core.Interfaces;
using Ecom.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
	// Unit of work class responsible for coordinating repository actions
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		private readonly IFileProvider _fileProvider;

		// Properties for accessing repositories
		public ICategoryRepository CategoryRepository { get; }
		public IProductRepository ProductRepository { get; }

		public UnitOfWork(ApplicationDbContext context, IFileProvider fileProvider)
		{
			_context = context;
			_fileProvider = fileProvider;

			// Initialize repository instances
			CategoryRepository = new CategoryRepository(_context);
			ProductRepository = new ProductRepository(_context, _fileProvider);
		}
	}
}
