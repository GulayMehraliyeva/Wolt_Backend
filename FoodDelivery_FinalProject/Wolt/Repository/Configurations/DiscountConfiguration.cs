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
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(d => d.DiscountPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(d => d.IsActive)
                .IsRequired();

            builder.Property(d => d.EndDate)
                .HasColumnType("date");
        }

    }
}
