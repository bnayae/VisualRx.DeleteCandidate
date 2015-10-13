using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Publishers.Common
{
    /// <summary>
    /// the contract of Visual Rx channel (proxy)
    /// (proxy is responsible to send the monitored datum through a channel)
    /// </summary>
    public interface IVisualRxChannel : IDisposable
    {
        /// <summary>
        /// Get the proxy's instance id
        /// </summary>
        Guid InstanceId { get; }

        /// <summary>
        /// Gets the monitor provider Name (unique like WebApi, WCF, ETW, File, etc.).
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Initialized indication.
        /// </summary>
        /// <returns>initialize information</returns>
        Task<ChannelInfo> InitializeAsync(IScheduler scheduler);

        /// <summary>
        /// Gets the bulk trigger
        /// determine the buffering boundaries
        /// for example:
        /// you can use Observable.Interval(...)  for time based interval
        /// or streamRate.Skip(1000).FirstOrDefaultAsync().Repeat() for buffering 1000 items
        /// </summary>
        /// <param name="streamRate">The stream rate.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        /// <value>
        /// The bulk trigger.
        /// </value>
        IObservable<Unit> BulkTrigger(IObservable<Unit> streamRate, IScheduler scheduler);

        /// <summary>
        /// Send a balk.
        /// </summary>
        /// <param name="items">The items.</param>
        Task BulkSend(IEnumerable<Marble> items);
    }
}