using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRS.EventSourcing.CRM.Application.Interfaces
{
    public interface ICRMDbContext
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<Address> Addresses { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}