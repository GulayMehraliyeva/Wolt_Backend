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
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {

            builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(30);

            builder.Property(s => s.Value)
                .IsRequired();

            builder.Property(s => s.Type)
                .IsRequired()
                .HasMaxLength(30);

        }
    }
}
