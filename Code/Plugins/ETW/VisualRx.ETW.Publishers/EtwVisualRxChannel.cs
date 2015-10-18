using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;
using VisualRx.Publishers.Common;
using System.Reactive;
using System.Reactive.Concurrency;
using VisualRx.ETW.Common;

namespace VisualRx.ETW.Publishers
{
    public sealed class EtwVisualRxChannel : IVisualRxChannel
    {
        private readonly JsonSerializerSettings _jsonSetting;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EtwVisualRxChannel"/> class.
        /// </summary>
        /// <param name="jsonSetting">The json setting.</param>
        public EtwVisualRxChannel(JsonSerializerSettings jsonSetting = null)
        {
            _jsonSetting = jsonSetting ?? Constants.JsonDefaultSetting;
        }

        #endregion // Ctor

        #region InstanceId

        /// <summary>
        /// Gets the instance identifier.
        /// </summary>
        /// <value>
        /// The instance identifier.
        /// </value>
        public Guid InstanceId { get; } = Guid.NewGuid();

        #endregion // InstanceId

        #region ProviderName

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string ProviderName { get; } = "ETW";

        #endregion // ProviderName

        #region BulkSend

        /// <summary>
        /// Bulks the send.
        /// </summary>
        /// <param name="marbles">The marbles.</param>
        /// <returns></returns>
        public Task BulkSend(IEnumerable<Marble> marbles)
        {
            foreach (var marble in marbles)
            {
                VisualRxEventSource.Send(marble, _jsonSetting);
            }
            return Task.CompletedTask;
        }

        #endregion // BulkSend

        #region BulkTrigger

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
            return streamRate;
        }

        #endregion // BulkTrigger

        #region InitializeAsync

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public Task<ChannelInfo> InitializeAsync(System.Reactive.Concurrency.IScheduler scheduler)
        {
            var info = new ChannelInfo(ProviderName, null, Dispose);
            return Task.FromResult(info);
        }

        #endregion // InitializeAsync

        #region Dispose Pattern

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="EtwVisualRxChannel" /> class.
        /// </summary>
        ~EtwVisualRxChannel()
        {
            Dispose(false);
        }

        #endregion // Dispose Pattern
    }
}
