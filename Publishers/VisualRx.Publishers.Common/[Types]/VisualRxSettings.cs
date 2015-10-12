using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Publishers.Common
{
    /// <summary>
    /// Visual Rx publish side settings
    /// </summary>
    public class VisualRxSettings
    {
        public static VisualRxSettings Default { get; } = new VisualRxSettings();

        private volatile bool _enable = true;
        private readonly ConcurrentDictionary<Guid, VisualRxProxyWrapper> _proxies =
            new ConcurrentDictionary<Guid, VisualRxProxyWrapper>();
        //                                     id, Func<streamKey, proxy provider name, bool>
        private readonly ConcurrentDictionary<Guid, Func<string, string, bool>> _filters =
            new ConcurrentDictionary<Guid, Func<string, string, bool>>();

        #region TryAddProxies

        /// <summary>
        /// Adds the proxies.
        /// </summary>
        /// <param name="proxies">The proxies.</param>
        /// <returns>
        /// initialization stream which complete when all the 
        /// proxies initialized
        /// each initialized proxy send the proxy initialization
        /// information via the Observable
        /// </returns>
        public IObservable<ProxyInfo> TryAddProxies(
            params IVisualRxChannel[] proxies)
        {
            if (proxies == null || !proxies.Any())
                throw new ArgumentException("missing proxies");

            var results =
                from p in proxies.ToObservable()
                from info in Observable.StartAsync(() => p.InitializeAsync())
                select new { Info = info, Proxy = p };

            results = results.Do(item =>
            {
                if (string.IsNullOrEmpty(item.Info.Error))
                {
                    var w = new VisualRxProxyWrapper(item.Proxy, Log);
                    _proxies.TryAdd(item.Proxy.InstanceId, w);
                    item.Info.Completion.ContinueWith(t =>
                    {
                        VisualRxProxyWrapper proxy;
                        _proxies.TryRemove(item.Proxy.InstanceId, out proxy);
                    });
                }
            });

            var hot = results.Select(m => m.Info)
                             .Publish();
            hot.Connect();
            return hot;
        }

        #endregion // TryAddProxies

        #region ClearProxies

        /// <summary>
        /// Removes all proxies.
        /// </summary>
        public void ClearProxies()
        {
            foreach (var p in _proxies.Values)
            {
                p.Dispose();
            }
        }

        #endregion // ClearProxies

        #region AddFilter

        /// <summary>
        /// Add Filter
        /// </summary>
        /// <param name="filter">
        /// get marble stream key and proxy provider name
        /// return true for using the proxy
        /// </param>
        /// <returns></returns>
        public IDisposable AddFilter(
            Func<string, string, bool> filter)
        {
            Guid key = Guid.NewGuid();
            var d = Disposable.Create(() =>
                {
                    _filters.TryRemove(key, out filter);
                });
            if (_filters.TryAdd(key, filter))
                return d;
            else
                return Disposable.Empty;
        }

        #endregion // AddFilter

        #region GetProxies

        /// <summary>
        /// Gets the proxies.
        /// </summary>
        /// <param name="streamKey">The candidate.</param>
        /// <returns></returns>
        internal VisualRxProxyWrapper[] GetProxies(
            string streamKey)
        {
            var proxies = from p in _proxies.Values
                          where _filters.Values.All(f => f(streamKey, p.ProviderName))
                          select p;
            return proxies.ToArray();
        }

        #endregion // GetProxies

        #region Send

        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void Send(Marble item, IEnumerable<VisualRxProxyWrapper> proxies)
        {
            #region Validation

            if (proxies == null || !proxies.Any())
            {
                Log.Warn($"{nameof(VisualRxSettings)}: No proxy found");
                return;
            }

            #endregion Validation

            foreach (VisualRxProxyWrapper proxy in proxies)
            {
                try
                {
                    //string kind = proxy.Kind;
                    //if (!VisualRxSettings.Filter(item, kind))
                    //    continue;

                    //if (!proxy.Filter(item))
                    //    continue;

                    // the proxy wrapper apply parallelism and batching (VIA Rx Subject)
                    proxy.Send(item);
                }
                #region Exception Handling

                catch (Exception ex)
                {
                    Log.Error(nameof(VisualRxSettings), ex);
                }

                #endregion Exception Handling
            }
        }

        #endregion Send

        #region Enable

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VisualRxSettings"/> is enable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enable; otherwise, <c>false</c>.
        /// </value>
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
            }
        }

        #endregion Enable

        #region Log

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public ILogAdapter Log { get; internal set; } = new DefaultLogger();

        #endregion // Log

        #region MachineInfo

        /// <summary>
        /// Gets the machine information (ex: MachineName, IP ...).
        /// </summary>
        /// <value>
        /// The machine information.
        /// </value>
        public string MachineInfo { get; internal set; }

        #endregion // MachineInfo

        #region DefaultLogger

        /// <summary>
        /// Default log implementation
        /// </summary>
        private class DefaultLogger : ILogAdapter
        {
            public void Error(string text, Exception ex = null)
            {
                Debug.WriteLine($"{text}: {ex}");
            }

            public void Warn(string text, Exception ex = null)
            {
                Debug.WriteLine($"{text}: {ex}");
            }
        }

        #endregion // DefaultLogger
    }
}