using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// Visual Rx publish side settings
    /// IMPORTANT: nothing will be published until setting
    ///            both filters and channels
    /// </summary>
    public class VisualRxSettings
    {
        public static VisualRxSettings Default { get; } = new VisualRxSettings();

        private readonly ConcurrentDictionary<Guid, VisualRxChannelWrapper> _channels =
            new ConcurrentDictionary<Guid, VisualRxChannelWrapper>();
        //                                     id, Func<streamKey, channel, bool>
        private readonly ConcurrentDictionary<Guid, Func<string, IVisualRxChannel, bool>> _filters =
            new ConcurrentDictionary<Guid, Func<string, IVisualRxChannel, bool>>();

        #region Ctor

        public VisualRxSettings()
        {
            Scheduler = new EventLoopScheduler();
        }

        #endregion // Ctor

        #region TryAddChannels

        /// <summary>
        /// Adds the channels.
        /// </summary>
        /// <param name="channels">The channels for sending the marbles.</param>
        /// <returns>
        /// initialization stream which complete when all the
        /// channels initialized
        /// each initialized channel send the channel initialization
        /// information via Observable
        /// </returns>
        public IObservable<ChannelInfo> TryAddChannels(
            params IVisualRxChannel[] channels)
        {
            if (channels == null || !channels.Any())
                throw new ArgumentException("missing proxies");

            var results =
                from channel in channels.ToObservable()
                let channelWrapper = new VisualRxChannelWrapper(channel, Log)
                from info in Observable.StartAsync(
                    () => channelWrapper.InitializeAsync(Scheduler))
                select new { Info = info, Channel = channelWrapper };

            results = results.Do(item =>
            {
                if (string.IsNullOrEmpty(item.Info.Error))
                {
                    _channels.TryAdd(
                        item.Channel.ActualChannel.InstanceId, 
                        item.Channel);
                    item.Info.Completion.ContinueWith(t =>
                    {
                        VisualRxChannelWrapper channel;
                        _channels.TryRemove(
                            item.Channel.ActualChannel.InstanceId, 
                            out channel);
                    });
                }
            });

            var hot = results.Select(m => m.Info)
                             .Replay();
            hot.Connect();
            return hot;
        }

        #endregion // TryAddChannels

        #region ClearChannels

        /// <summary>
        /// Removes all channels.
        /// </summary>
        public void ClearChannels()
        {
            foreach (var channel in _channels.Values)
            {
                channel.Dispose();
            }
        }

        #endregion // ClearChannels

        #region GetChannel

        /// <summary>
        /// Gets the proxies.
        /// </summary>
        /// <param name="streamKey">The candidate.</param>
        /// <returns></returns>
        internal VisualRxChannelWrapper[] GetChannel(
            string streamKey)
        {
            var channels = from channel in _channels.Values
                          where _filters.Values.Any(
                              f => f(streamKey, channel.ActualChannel))
                          select channel;
            return channels.ToArray();
        }

        #endregion // GetChannel

        // TODO: Bnaya, 2015-10, Use overload with Filter DTO (StreamKey [equals, start with, etc.], provider name [equals, start with, etc.]) 
        #region AddFilter

        /// <summary>
        /// Add Filter
        /// </summary>
        /// <param name="filter">
        /// get marble stream key and channel
        /// return the registration key
        /// </param>
        /// <returns></returns>
        public Guid AddFilter(
            Func<string, IVisualRxChannel, bool> filter)
        {
            Guid key = Guid.NewGuid();
            if (!_filters.TryAdd(key, filter))
                return Guid.Empty;

            return key;
        }

        #endregion // AddFilter

        #region RemoveFilter

        /// <summary>
        /// Removes the filter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool RemoveFilter(Guid key)
        {
            Func<string, IVisualRxChannel, bool> filter;
            bool result = _filters.TryRemove(key, out filter);
            return result;
        }

        #endregion // RemoveFilter

        #region Log

        /// <summary>
        /// Gets or set the logger
        /// </summary>
        /// <value>
        /// Action<level, message, exception>
        /// </value>
        public Action<LogLevel, string, Exception> Log { get; set; } = 
            (level, message, ex) => Debug.WriteLine($"VisualRx Publisher LOG [{message}]: {message}\r\n\t{ex?.ToString()?.Replace(Environment.NewLine, Environment.NewLine + "\t")}");

        #endregion // Log

        #region MachineInfo

        /// <summary>
        /// Gets the machine information (ex: MachineName, IP ...).
        /// </summary>
        /// <value>
        /// The machine information.
        /// </value>
        public string MachineInfo { get; set; }

        #endregion // MachineInfo

        #region Scheduler

        private IScheduler _scheduler;

        /// <summary>
        /// Gets or sets the scheduler (for publishing).
        /// </summary>
        /// <value>
        /// The scheduler.
        /// </value>
        public IScheduler Scheduler
        {
            get { return _scheduler; }
            set
            {
                _scheduler = value ?? DefaultScheduler.Instance;

                _scheduler.Catch<Exception>(e =>
                {
                    Log(LogLevel.Error, $"Scheduling (OnBulkSend): {e}", null);
                    return true;
                });
            }
        }

        #endregion // Scheduler
    }
}