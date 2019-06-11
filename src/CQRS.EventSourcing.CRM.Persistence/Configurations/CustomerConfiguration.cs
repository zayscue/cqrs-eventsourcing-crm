using CQRS.EventSourcing.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRS.EventSourcing.CRM.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Prefix)
                .HasMaxLength(5);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Created)
                .IsRequired();
        }
    }
}