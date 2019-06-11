using CQRS.EventSourcing.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRS.EventSourcing.CRM.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.StreetAddress)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.StreetAddress2)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.StateOrProvince)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.ISOCountryCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.PostalCode)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasOne(x => x.Customer)
                .WithMany(y => y.Addresses)
                .HasForeignKey(x => x.CustomerId);
        }
    }
}