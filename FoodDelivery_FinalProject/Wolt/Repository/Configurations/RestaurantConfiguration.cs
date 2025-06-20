using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.Property(r => r.Name)
           .IsRequired()
           .HasMaxLength(20);

            builder.Property(r => r.Address)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Phone)
                .IsRequired();

            builder.Property(r => r.Image)
                .IsRequired();

            builder.Property(r => r.EstimatedDeliveryTime)
                .IsRequired();

            builder.Property(r => r.DeliveryCost)
                .IsRequired()
                .HasColumnType("decimal(6,2)");

            builder.Property(r => r.RestaurantCategoryId)
                .IsRequired();
        }
    }
}
