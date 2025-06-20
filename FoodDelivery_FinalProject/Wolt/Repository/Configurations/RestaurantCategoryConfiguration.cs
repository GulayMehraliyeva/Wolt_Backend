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
    public class RestaurantCategoryConfiguration : IEntityTypeConfiguration<RestaurantCategory>
    {
        public void Configure(EntityTypeBuilder<RestaurantCategory> builder)
        {
            builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(20);

            builder.Property(m => m.Image)
            .IsRequired();
        }
    }
}
