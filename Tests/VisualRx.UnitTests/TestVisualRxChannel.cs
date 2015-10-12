using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.UnitTests
{
    public class TestVisualRxChannel : IVisualRxChannel
    {
        private readonly IObserver<IEnumerable<Marble>> _observer;

        public TestVisualRxChannel(TestScheduler scheduler)
        {
            _observer = scheduler.CreateObserver<IEnumerable<Marble>>();
        }
        public Guid InstanceId { get; } = Guid.Empty;

        public string ProviderName { get; } = "Testing";

        public Task BulkSend(IEnumerable<Marble> items)
        {
            _observer.OnNext(items);
            return Task.CompletedTask;
        }

        public IObservable<Unit> BulkTrigger(IObservable<Unit> streamRate)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<ProxyInfo> InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
