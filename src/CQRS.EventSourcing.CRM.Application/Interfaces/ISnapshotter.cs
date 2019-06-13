using System.Threading.Tasks;

namespace CQRS.EventSourcing.CRM.Application.Interfaces
{
    public interface ISnapshotter
    {
        Task CreateSnapshots();
    }
}