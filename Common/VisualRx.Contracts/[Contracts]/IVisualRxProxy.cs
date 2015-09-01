using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace VisualRx.Contracts
{
    /// <summary>
    /// the contract of Visual Rx proxy
    /// (proxy is responsible to send the monitored datum through a channel)
    /// </summary>
    public interface IVisualRxProxy: IDisposable
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>initialize information</returns>
        Task<string> OnInitialize();

        /// <summary>
        /// Send a balk.
        /// </summary>
        /// <param name="items">The items.</param>
        Task OnBulkSend(IEnumerable<Marble> items);

        /// <summary>
        /// Gets the monitor provider kind (unique like WebApi, WCF, ETW, File, etc.).
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Gets the bulk trigger
        /// determine the buffering boundaries
        /// for example:
        /// you can use Observable.Interval(...)  for time based interval
        /// or streamRate.Skip(1000).FirstOrDefaultAsync().Repeat() for buffering 1000 items
        /// </summary>
        /// <param name="streamRate">The stream rate.</param>
        /// <returns></returns>
        /// <value>
        /// The bulk trigger.
        /// </value>
        IObservable<Unit> BulkTrigger(IObserver<Unit> streamRate);
    }
}