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
    /// Visual Rx sender proxy
    /// Wrap the actual proxy and create a batch (bulk) send.
    /// </summary>
    internal sealed class VisualRxProxyWrapper : IDisposable
    {
        #region Private / Protected Fields

        private readonly IVisualRxProxy _actualProxy;
        private ISubject<Marble> _subject;
        private IDisposable _unsubSubject;

        private IScheduler _scheduler;

        #endregion Private / Protected Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualRxProxyWrapper" /> class.
        /// </summary>
        public VisualRxProxyWrapper(IVisualRxProxy actualProxy)
        {
            if (actualProxy == null)
                throw new ArgumentNullException(nameof(actualProxy));

            _actualProxy = actualProxy;
        }

        #endregion // Ctor

        #region ActualProxy

        /// <summary>
        /// Gets the actual proxy.
        /// </summary>
        /// <value>
        /// The actual proxy.
        /// </value>
        internal IVisualRxProxy ActualProxy { get { return _actualProxy; } }

        #endregion // ActualProxy

        #region Kind

        /// <summary>
        /// Gets the proxy kind.
        /// </summary>
        public string Kind { get { return _actualProxy.ProviderName; } }

        #endregion Kind

        #region Methods

        #region Initialize

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>Initialize information</returns>
        public Task<ProxyInfo> InitializeAsync()
        {
            #region _scheduler = new EventLoopScheduler(...)

            _scheduler = new EventLoopScheduler();

            _scheduler.Catch<Exception>(e =>
            {
                VisualRxSettings.Log.Error("Scheduling (OnBulkSend): {0}", e);
                return true;
            });

            #endregion _scheduler = new EventLoopScheduler(...)

            _subject = new Subject<Marble>();
            var tmpStream = _subject
                .ObserveOn(_scheduler) // single thread
                                       //.Synchronize()
                .Retry()
                .Buffer(_actualProxy.BulkTrigger(_subject.Select(m => Unit.Default)))
                .Where(items => items.Count != 0);
            _unsubSubject = tmpStream.Subscribe(
                m => _actualProxy.BulkSend(m));

            return _actualProxy.InitializeAsync();
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

                _actualProxy.Dispose();

                Dispose(disposed);
            }
            catch (Exception ex)
            {
                VisualRxSettings.Log.Error($"{this.GetType().Name}.{nameof(Dispose)}", ex);
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
        /// <see cref="VisualRxProxyWrapper"/> is reclaimed by garbage collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "DisposeInternal does call Dispose(disposed)")]
        ~VisualRxProxyWrapper()
        {
            DisposeInternal(false);
        }

        #endregion Finalizer and Dispose
    }
}