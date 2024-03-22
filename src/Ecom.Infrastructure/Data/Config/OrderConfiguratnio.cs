using Ecom.Core.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Data.Config
{
	public class OrderConfiguratnio : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			// Convert OrderStatus enum to string for storage in the database
			builder.Property(x => x.OrderStatus).HasConversion(
				o => o.ToString(),
				o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o));

			builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

			builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
		}
	}
}
