using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Actions.Customers;
using CQRS.EventSourcing.CRM.Domain.Entities;
using CQRS.EventSourcing.CRM.Domain.EventStore;
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
                Customer customer = null;
                if (delta.LastSnapshotVersion > 0)
                {
                    var snapShot = await _eventStore.GetSnapshot(delta.AggregateId, delta.LastSnapshotVersion);
                    customer = JsonConvert.DeserializeObject<Customer>(snapShot.SerializedData);
                }
                else
                {
                    customer = new Customer { Id = delta.AggregateId };
                }
                var redux = new ReduxStore<Customer>(Customer.Reducer, customer);

                var @events = await _eventStore.GetEvents(delta.AggregateId,
                    delta.LastSnapshotVersion, delta.CurrentVersion);
                foreach (var @event in @events)
                {
                    switch (@event.Name)
                    {
                        case "CustomerCreated":
                            customer.Created = @event.TimeStamp;
                            redux.Dispatch(JsonConvert.DeserializeObject<CreateCustomer>(@event.Data));
                            break;
                        case "CustomersFirstNameUpdated":
                            redux.Dispatch(JsonConvert.DeserializeObject<UpdateCustomersFirstName>(@event.Data));
                            break;
                        case "CustomersLastNameUpdated":
                            redux.Dispatch(JsonConvert.DeserializeObject<UpdateCustomersLastName>(@event.Data));
                            break;
                        case "CustomersPrefixUpdated":
                            redux.Dispatch(JsonConvert.DeserializeObject<UpdateCustomersPrefix>(@event.Data));
                            break;
                        case "CustomersTitleUpdated":
                            redux.Dispatch(JsonConvert.DeserializeObject<UpdateCustomersTitle>(@event.Data));
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