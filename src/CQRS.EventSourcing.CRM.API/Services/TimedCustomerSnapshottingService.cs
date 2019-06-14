using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Customers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CQRS.EventSourcing.CRM.API.Services
{
    internal class TimedCustomerSnapshottingService : IHostedService, IDisposable
    {
        private readonly CustomerSnapshotter _snapshotter;
        private readonly ILogger _logger;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);
        private Task _outputTask;

        public TimedCustomerSnapshottingService(CustomerSnapshotter snapshotter, ILogger<TimedCustomerSnapshottingService> logger)
        {
            _snapshotter = snapshotter;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Snapshotting Background Service is starting.");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

            _outputTask = Task.Factory.StartNew<Task>(
                DoWork,
                null,
                TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        private async Task DoWork(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _logger.LogInformation("Starting to snapshot customer aggregates.");
                await _snapshotter.CreateSnapshots();
                await Task.Delay(_interval, _cancellationTokenSource.Token);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Customer Snapshotting Background Service is stopping.");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _outputTask?.Dispose();
        }
    }
}