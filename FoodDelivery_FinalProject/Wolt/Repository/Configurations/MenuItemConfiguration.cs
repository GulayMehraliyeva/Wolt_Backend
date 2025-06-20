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
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.Property(m => m.Name)
               .IsRequired()
               .HasMaxLength(20);

            builder.Property(m => m.Description)
                   .HasMaxLength(30);

            builder.Property(m => m.Image)
               .IsRequired();

            builder.Property(m => m.Price)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(m => m.CategoryId)
                   .IsRequired();
        }
    }
}
