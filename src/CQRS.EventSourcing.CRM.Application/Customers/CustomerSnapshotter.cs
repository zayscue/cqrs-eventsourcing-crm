using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Entities;
using CQRS.EventSourcing.CRM.Domain.Events.Customers;
using Newtonsoft.Json;

namespace CQRS.EventSourcing.CRM.Application.Customers
{
    public class CustomerSnapshotter : ISnapshotter
    {
        private readonly IEventStore _eventStore;
        private string AggregateType => typeof(Customer).FullName;

        public CustomerSnapshotter(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task CreateSnapshots()
        {
            var customerSnapshotDeltas = await _eventStore.QuerySnapshotDeltas(AggregateType);
            foreach (var delta in customerSnapshotDeltas)
            {
                ReduxStore<Customer> redux = null;
                if (delta.LastSnapshotVersion > 0)
                {
                    var snapShot = await _eventStore.GetSnapshot(delta.AggregateId, delta.LastSnapshotVersion);
                    var customerSnapshot = JsonConvert.DeserializeObject<Customer>(snapShot.SerializedData);
                    redux = new ReduxStore<Customer>(Customer.Reducer, customerSnapshot);
                }
                else
                {
                    redux = new ReduxStore<Customer>(Customer.Reducer, new Customer
                    {
                        Id = delta.AggregateId
                    });
                }

                var @events = await _eventStore.GetEvents(delta.AggregateId,
                    delta.LastSnapshotVersion, delta.CurrentVersion);
                foreach (var @event in @events)
                {
                    switch (@event.Name)
                    {
                        case "CustomerCreatedEvent":
                            redux.Dispatch(@event.TimeStamp);
                            redux.Dispatch(JsonConvert.DeserializeObject<CustomerCreatedEvent>(@event.Data));
                            break;
                        case "CustomerFirstNameChangedEvent":
                            redux.Dispatch(JsonConvert.DeserializeObject<CustomerFirstNameChangedEvent>(@event.Data));
                            break;
                        case "CustomerLastNameChangedEvent":
                            redux.Dispatch(JsonConvert.DeserializeObject<CustomerLastNameChangedEvent>(@event.Data));
                            break;
                        case "CustomerPrefixChangedEvent":
                            redux.Dispatch(JsonConvert.DeserializeObject<CustomerPrefixChangedEvent>(@event.Data));
                            break;
                        case "CustomerTitleChangedEvent":
                            redux.Dispatch(JsonConvert.DeserializeObject<CustomerTitleChangedEvent>(@event.Data));
                            break;
                        default:
                            break;
                    }
                }
                await _eventStore.SaveSnapshot(new Snapshot
                {
                    AggregateId = delta.AggregateId,
                    SerializedData = JsonConvert.SerializeObject(redux.State),
                    Version = delta.CurrentVersion
                });
            }
        }
    }
}