using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Customers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CQRS.EventSourcing.CRM.API
{
    internal class TimedCustomerSnapshottingService : IHostedService, IDisposable
    {
        private readonly CustomerSnapshotter _snapshotter;
        private readonly ILogger _logger;
        private Timer _timer;

        public TimedCustomerSnapshottingService(CustomerSnapshotter snapshotter, ILogger<TimedCustomerSnapshottingService> logger)
        {
            _snapshotter = snapshotter;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Snapshotting Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _snapshotter.CreateSnapshots().GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Snapshotting Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}