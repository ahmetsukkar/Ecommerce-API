using Ecom.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(x => x.Id).IsRequired();
			builder.Property(x => x.Name).HasMaxLength(30);
			builder.Property(x => x.Price).HasColumnType("decimal(18,2)");

			// Seed initial data
			builder.HasData(GenerateProductData());
		}

		private List<Product> GenerateProductData()
		{
			// List of book names
			var bookNames = new List<string>
			{
				"Clean Code",
				"The Pragmatic Programmer",
				"Design Patterns",
				"Refactoring",
				"Code Complete",
				"Introduction to Algorithms",
				"Head First Design Patterns",
				"Effective C#",
				"The Clean Coder",
				"Programming Pearls",
				"Cracking the Coding Interview",
				"JavaScript: The Good Parts",
				"Domain-Driven Design",
				"Python Crash Course",
				"Clean Architecture",
				"Test Driven Development",
				"Eloquent JavaScript",
				"Head First Java",
				"Learning Python",
				"Algorithms: Part I",
				"Algorithms: Part II",
				"Effective Java",
				"Java Concurrency in Practice",
				"Refactoring to Patterns",
				"Head First Python",
				"The Mythical Man-Month",
				"Grokking Algorithms",
				"Concurrency in C# Cookbook",
				"Programming Entity Framework",
				"The Algorithm Design Manual",
				"Java Performance",
				"Operating System Concepts",
				"Database Management Systems",
				"Automate the Boring Stuff"
			};

			var rnd = new Random();
			var products = new List<Product>();

			for (int i = 0; i < bookNames.Count(); i++)
			{
				var bookName = bookNames[i];
				var ProductPictureName = bookName.Replace(" ", "");
				var product = new Product
				{
					Id = i + 1,
					Name = bookName,
					Description = $"Description for {bookName}",
					Price = rnd.Next(1, 1000),
					CategoryId = 1, // Category ID for books
					ProductPicture = $"/images/products/{ProductPictureName}.jpg"
				};
				products.Add(product);
			}

			return products;
		}
	}
}
