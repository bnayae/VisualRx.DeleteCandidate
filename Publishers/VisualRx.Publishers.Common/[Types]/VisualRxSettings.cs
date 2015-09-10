using System;
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
    public static class VisualRxSettings
    {
        private static volatile bool _enable = true;

        #region AddProxies

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
        public static IObservable<ProxyInfo> AddProxies(
            params IVisualRxProxy[] proxies)
        {
            if (proxies == null || !proxies.Any())
                throw new ArgumentException("missing proxies");

            IObservable<ProxyInfo> infos =
                from p in proxies.ToObservable()
                from info in Observable.StartAsync(() => p.InitializeAsync())
                select info;

            infos = infos.Do(v =>
            {
                // TODO: Add proxy to collections
            });

            return infos;
        }

        #endregion // AddProxies

        #region RemoveProxies

        /// <summary>
        /// Removes the proxies.
        /// </summary>
        /// <param name="proxies">The proxies.</param>
        /// <exception cref="System.ArgumentException">missing proxies</exception>
        public static void RemoveProxies(params IVisualRxProxy[] proxies)
        {
            if (proxies == null || !proxies.Any())
                throw new ArgumentException("missing proxies");

            //lock (_syncProxies)
            {
                IEnumerable<VisualRxProxyWrapper> left =
                    from w in Proxies
                    where !proxies.Any(p => p == w.ActualProxy)
                    select w;

                Proxies = left.ToArray();
            }
            Task.Factory.StartNew(state =>
            {
                #region Dispose

                var removed = state as IVisualRxProxy[];
                foreach (var p in removed)
                {
                    try
                    {
                        p.Dispose();
                    }
                    #region Exception Handling

                    catch (Exception ex)
                    {
                        Log.Error($"Dispose proxy: [{p.ProviderName}]", ex);
                    }

                    #endregion // Exception Handling
                }

                #endregion // Dispose
            }, proxies);
        }

        #endregion // RemoveProxies

        #region Elapsed

        /// <summary>
        /// Elapsed the specified candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static TimeSpan Elapsed(MarbleCandidate candidate)
        {
            throw new NotImplementedException();
        }
        #endregion // Elapsed

        #region GetProxies

        /// <summary>
        /// Gets the proxies.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        internal static VisualRxProxyWrapper[] GetProxies(MarbleCandidate candidate)
        {
            if (Proxies == null || !Proxies.Any())
            {
                Log.Warn("MonitorOperator: No proxy found");
                return new VisualRxProxyWrapper[0];
            }

            //var proxies = (from p in Proxies
            //               where VisualRxSettings.Filter(candidate, p.Kind) &&
            //                    p.Filter(candidate)
            //               select p).ToArray();
            //return proxies;
            return null;
        }

        #endregion // GetProxies

        #region Send

        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal static void Send(Marble item, IEnumerable<VisualRxProxyWrapper> proxies)
        {
            #region Validation

            if (Proxies == null || !Proxies.Any())
            {
                Log.Warn("MonitorOperator: No proxy found");
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
                    Log.Error("MonitorOperator", ex);
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
        public static bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
            }
        }

        #endregion Enable

        #region Proxies

        /// <summary>
        /// Gets the proxies.
        /// </summary>
        /// <value>
        /// The proxies.
        /// </value>
        private static VisualRxProxyWrapper[] Proxies { get; set; }

        #endregion Proxies

        #region Log

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public static ILogAdapter Log { get; internal set; }
        #endregion // Log

        #region MachineInfo

        /// <summary>
        /// Gets the machine information (ex: MachineName, IP ...).
        /// </summary>
        /// <value>
        /// The machine information.
        /// </value>
        public static string MachineInfo { get; internal set; }
        #endregion // MachineInfo
    }
}