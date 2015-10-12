using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using VisualRx.Publishers.Common;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using VisualRx.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace VisualRx.UnitTests
{
    public class TestVisualRxChannel : IVisualRxChannel
    {
        public TestVisualRxChannel(Guid? instanceKey = null)
        {
            InstanceId = instanceKey ?? Guid.Empty;
        }
        private readonly List<Marble> _marbles = new List<Marble>();

        public Guid InstanceId { get; } 

        public string ProviderName { get; } = "Testing";

        public Task BulkSend(IEnumerable<Marble> items)
        {
            _marbles.AddRange(items);
            if (items.LastOrDefault()?.Kind == MarbleKind.OnCompleted)
                _completion.SetResult(null);
            else if (items.LastOrDefault()?.Kind == MarbleKind.OnError)
                _completion.SetException(new Exception(items.Last().Value.ToString()));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Bulks the trigger.
        /// </summary>
        /// <param name="streamRate">The stream rate.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public IObservable<Unit> BulkTrigger(
                    IObservable<Unit> streamRate,
                    IScheduler scheduler)
        {
            var result = streamRate.Buffer(3)
                .Select(_ => Unit.Default);
            return result;
        }

        public void Dispose()
        {
        }

        public Task<ChannelInfo> InitializeAsync(IScheduler scheduler)
        {
            var info = new ChannelInfo(ProviderName, "meta", () => { });
            return Task.FromResult(info);
        }

        public Marble[] Results => _marbles
            .Where(m => m.Kind == MarbleKind.OnNext)
            .ToArray();

        private readonly TaskCompletionSource<object> _completion = new TaskCompletionSource<object>();
        public Task Completion => _completion.Task;
    }
}
