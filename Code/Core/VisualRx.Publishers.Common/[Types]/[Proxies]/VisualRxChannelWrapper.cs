using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Publishers.Common
{
    /// <summary>
    /// Visual Rx sender channel
    /// Wrap the actual channel and create a batch (bulk) send.
    /// </summary>
    internal sealed class VisualRxChannelWrapper : IDisposable
    {
        #region Private / Protected Fields

        private readonly IVisualRxChannel _actualChannel;
        private ISubject<Marble> _subject;
        private IDisposable _unsubSubject;

        // level, message, error
        private readonly Action<LogLevel, string, Exception> _logger;

        #endregion Private / Protected Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualRxChannelWrapper" /> class.
        /// </summary>
        /// <param name="actualChannel">The actual channel.</param>
        /// <param name="logger">level, message, error</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public VisualRxChannelWrapper(
            IVisualRxChannel actualChannel,
            Action<LogLevel, string, Exception> logger)
        {
            if (actualChannel == null)
                throw new ArgumentNullException(nameof(actualChannel));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _actualChannel = actualChannel;
            _logger = logger;
        }

        #endregion // Ctor

        #region ActualChannel

        /// <summary>
        /// Gets the actual channel.
        /// </summary>
        internal IVisualRxChannel ActualChannel => _actualChannel;

        #endregion // ActualChannel

        #region ProviderName

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        public string ProviderName => _actualChannel.ProviderName;

        #endregion ProviderName

        #region Methods

        #region Initialize

        /// <summary>
        /// Initializes this channel.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>
        /// Initialize information
        /// </returns>
        public Task<ChannelInfo> InitializeAsync(
            IScheduler scheduler)
        {
            _subject = new Subject<Marble>();
            var marbleStream = _subject
                    .Select(m => Unit.Default);

            var completionTrigger = _subject
                    .Where(m => m.Kind != NotificationKind.OnNext)
                    .Select(m => Unit.Default);

            var bufferTrigger = _actualChannel
                .BulkTrigger(marbleStream, scheduler)
                .Merge(completionTrigger);

            var tmpStream = _subject
                .ObserveOn(scheduler) // single thread
                                       //.Synchronize()
                .Retry()
                .Buffer(bufferTrigger)
                .Where(items => items.Count != 0);
            _unsubSubject = tmpStream.Subscribe(
                m => _actualChannel.BulkSend(m));

            return _actualChannel.InitializeAsync(scheduler);
        }

        #endregion Initialize

        #region Send

        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Send(Marble item)
        {
            _subject.OnNext(item);
        }

        #endregion Send

        #endregion Methods

        #region Finalizer and Dispose

        /// <summary>
        /// Performs application-defined tasks associated
        /// with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "DisposeInternal does call Dispose(disposed)")]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeInternal(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void DisposeInternal(bool disposed)
        {
            try
            {
                IDisposable unsubSubject = _unsubSubject;
                if (unsubSubject != null)
                    unsubSubject.Dispose();

                _actualChannel.Dispose();

                Dispose(disposed);
            }
            catch (Exception ex)
            {
                _logger(LogLevel.Error, $"{this.GetType().Name}.{nameof(Dispose)}", ex);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposed)
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="VisualRxChannelWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "DisposeInternal does call Dispose(disposed)")]
        ~VisualRxChannelWrapper()
        {
            DisposeInternal(false);
        }

        #endregion Finalizer and Dispose
    }
}