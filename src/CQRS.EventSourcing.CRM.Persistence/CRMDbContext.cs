using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRS.EventSourcing.CRM.Persistence
{
    public class CRMDbContext : DbContext, ICRMDbContext
    {
        public CRMDbContext(DbContextOptions<CRMDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CRMDbContext).Assembly);
        }
    }
}