﻿using Newtonsoft.Json;
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
    /// Main class for the monitor logic and coordination
    /// single instance per stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StreamChannel<T>
    {
        #region Private / Protected Fields

        private readonly VisualRxPublishersSettings _setting;
        private readonly double _indexOrder;
        private readonly Func<T, object> _surrogate;
        private static readonly Stopwatch _time = Stopwatch.StartNew();
        private readonly TimeSpan _offset; //offset duration

        #endregion Private / Protected Fields

        #region Constructors

        public StreamChannel(
            VisualRxPublishersSettings setting,
            string streamKey,
            double indexOrder,
            Func<T, object> surrogate)
        {
            #region Validation

            if (surrogate == null)
                surrogate = m => m;

            #endregion Validation

            _setting = setting;

            StreamKey = streamKey;
            _surrogate = surrogate;
            _indexOrder = indexOrder;
            _offset = _time.Elapsed;
        }

        #endregion Constructors

        #region StreamKey

        /// <summary>
        /// Gets the stream key.
        /// </summary>
        public string StreamKey { get; }

        #endregion // StreamKey

        #region Methods

        #region AttachTo

        /// <summary>
        /// Attaches to.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IObservable<T> AttachTo(IObservable<T> instance) =>
            instance.Do(PublishOnNext, PublishError, PublishComplete);
        
        /// <summary>
        /// Attaches to.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IConnectableObservable<T> AttachTo(IConnectableObservable<T> instance)
        {
            IObservable<T> monitorStream = instance.Do(PublishOnNext, PublishError, PublishComplete);
            return new ConnectableWraper(monitorStream, instance);
        }

        /// <summary>
        /// Attaches to.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public ISubject<T> AttachTo(ISubject<T> instance)
        {
            IObservable<T> monitorStream = instance.Do(PublishOnNext, PublishError, PublishComplete);
            return new SubjectWrapper(monitorStream, instance);
        }

        /// <summary>
        /// Attaches to.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public ISubject<TIn, T> AttachTo<TIn>(ISubject<TIn, T> instance)
        {
            var monitorStream = instance.Do(PublishOnNext, PublishError, PublishComplete);
            return new SubjectWrapper<TIn, T>(monitorStream, instance);
        }

        #endregion AttachTo

        #region Elapsed

        /// <summary>
        /// Elapsed the specified candidate.
        /// </summary>
        /// <returns></returns>
        private TimeSpan Elapsed()
        {
            TimeSpan elapsed = _time.Elapsed;
            TimeSpan result = elapsed - _offset;
            return result;
        }

        #endregion // Elapsed

        #region Publish

        #region PublishOnNext

        /// <summary>
        /// Publishes the on next.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PublishOnNext(T value)
        {
            VisualRxChannelWrapper[] channels = _setting.GetChannel(StreamKey);

            #region Validation

            if (channels.Length == 0)
                return;

            #endregion // Validation

            object item = _surrogate(value);
            TimeSpan elapsed = Elapsed(); 
            var marbleItem = Marble.CreateNext(
                                StreamKey,
                                item, 
                                elapsed,
                                _setting.MachineInfo);
            Publish(marbleItem, channels);
        }

        #endregion PublishOnNext

        #region PublishComplete

        /// <summary>
        /// Publishes the complete.
        /// </summary>
        public void PublishComplete()
        {
            VisualRxChannelWrapper[] channels = _setting.GetChannel(StreamKey);

            #region Validation

            if (channels.Length == 0)
                return;

            #endregion // Validation

            TimeSpan elapsed = Elapsed();
            var marbleItem = Marble.CreateCompleted(
                StreamKey,
                elapsed, 
                _setting.MachineInfo);
            Publish(marbleItem, channels);
        }

        #endregion PublishComplete

        #region PublishError

        /// <summary>
        /// Publishes the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void PublishError(Exception ex)
        {
            VisualRxChannelWrapper[] channels = _setting.GetChannel(StreamKey);

            #region Validation

            if (channels.Length == 0)
                return;

            #endregion // Validation

            TimeSpan elapsed = Elapsed();
            var marbleItem = Marble.CreateError(
                StreamKey, 
                ex, 
                elapsed, 
                _setting.MachineInfo);
            Publish(marbleItem, channels);
        }

        #endregion PublishError

        /// <summary>
        /// Publishes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void Publish(
            Marble item, 
            VisualRxChannelWrapper[] channels)
        {
            #region Validation

            if (channels == null || !channels.Any())
            {
                _setting.Log(LogLevel.Warning, $"{nameof(VisualRxPublishersSettings)}: No proxy found", null);
                return;
            }

            #endregion Validation

            item.IndexOrder = _indexOrder;

            foreach (VisualRxChannelWrapper channel in channels)
            {
                try
                {
                    // the channel wrapper apply parallelism and batching (VIA Rx Subject)
                    channel.Send(item);
                }
                #region Exception Handling

                catch (Exception ex)
                {
                    _setting.Log(LogLevel.Error, nameof(VisualRxPublishersSettings), ex);
                }

                #endregion Exception Handling
            }
        }

        #endregion Publish

        #endregion Methods

        #region Nested Types

        #region ConnectableWraper

        /// <summary>
        /// Connectable observable monitor wrapper
        /// </summary>
        private class ConnectableWraper : IConnectableObservable<T>
        {
            private IConnectableObservable<T> _internalStream;
            private IObservable<T> _monitorStream;

            #region Ctor

            public ConnectableWraper(IObservable<T> monitorStream, IConnectableObservable<T> internalStream)
            {
                _internalStream = internalStream;
                _monitorStream = monitorStream;
            }

            #endregion Ctor

            #region Connect

            /// <summary>
            /// start the observable.
            /// </summary>
            /// <returns></returns>
            public IDisposable Connect()
            {
                return _internalStream.Connect();
            }

            #endregion Connect

            #region Subscribe

            /// <summary>
            /// Subscribes the specified observer.
            /// </summary>
            /// <param name="observer">The observer.</param>
            /// <returns></returns>
            public IDisposable Subscribe(IObserver<T> observer)
            {
                var callbackDisposable = new CancellationDisposable();
                IDisposable disp = _monitorStream
                    .Finally(() =>
                    {
                        _internalStream = null;
                        _monitorStream = null;
                    })
                    .Subscribe(observer);
                callbackDisposable.Token.Register(() =>
                {
                    _internalStream = null;
                    _monitorStream = null;
                });
                return new CompositeDisposable(callbackDisposable, disp);
            }

            #endregion Subscribe
        }

        #endregion ConnectableWraper

        #region SubjectWrapper

        /// <summary>
        /// Connectable observable monitor wrapper
        /// </summary>
        private class SubjectWrapper : ISubject<T>
        {
            private ISubject<T> _internalStream;
            private IObservable<T> _monitorStream;

            #region Ctor

            public SubjectWrapper(IObservable<T> monitorStream, ISubject<T> internalStream)
            {
                _internalStream = internalStream;
                _monitorStream = monitorStream;
            }

            #endregion Ctor

            #region Subscribe

            /// <summary>
            /// Subscribes the specified observer.
            /// </summary>
            /// <param name="observer">The observer.</param>
            /// <returns></returns>
            public IDisposable Subscribe(IObserver<T> observer)
            {
                var callbackDisposable = new CancellationDisposable();
                IDisposable disp = _monitorStream
                    .Finally(() =>
                    {
                        _internalStream = null;
                        _monitorStream = null;
                    })
                    .Subscribe(observer);
                callbackDisposable.Token.Register(() =>
                {
                    _internalStream = null;
                    _monitorStream = null;
                });
                return new CompositeDisposable(callbackDisposable, disp);
            }

            #endregion Subscribe

            #region IObserver Members

            /// <summary>
            /// Called when [completed].
            /// </summary>
            public void OnCompleted()
            {
                _internalStream.OnCompleted();
            }

            /// <summary>
            /// Called when [error].
            /// </summary>
            /// <param name="error">The error.</param>
            public void OnError(Exception error)
            {
                _internalStream.OnError(error);
            }

            /// <summary>
            /// Called when [next].
            /// </summary>
            /// <param name="value">The value.</param>
            public void OnNext(T value)
            {
                _internalStream.OnNext(value);
            }

            #endregion IObserver Members
        }

        /// <summary>
        /// Subject monitor wrapper
        /// </summary>
        /// <typeparam name="TIn">The type of the in.</typeparam>
        /// <typeparam name="TOunt">The type of the out.</typeparam>
        private class SubjectWrapper<TIn, TOunt> : ISubject<TIn, TOunt>
        {
            private ISubject<TIn, TOunt> _internalStream;
            private IObservable<TOunt> _monitorStream;

            #region Ctor

            public SubjectWrapper(IObservable<TOunt> monitorStream, ISubject<TIn, TOunt> internalStream)
            {
                _internalStream = internalStream;
                _monitorStream = monitorStream;
            }

            #endregion Ctor

            #region Subscribe

            /// <summary>
            /// Subscribes the specified observer.
            /// </summary>
            /// <param name="observer">The observer.</param>
            /// <returns></returns>
            public IDisposable Subscribe(IObserver<TOunt> observer)
            {
                var callbackDisposable = new CancellationDisposable();
                IDisposable disp = _monitorStream
                    .Finally(() =>
                    {
                        _internalStream = null;
                        _monitorStream = null;
                    })
                    .Subscribe(observer);
                callbackDisposable.Token.Register(() =>
                {
                    _internalStream = null;
                    _monitorStream = null;
                });
                return new CompositeDisposable(callbackDisposable, disp);
            }

            #endregion Subscribe

            #region IObserver Members

            /// <summary>
            /// Called when [completed].
            /// </summary>
            public void OnCompleted()
            {
                _internalStream.OnCompleted();
            }

            /// <summary>
            /// Called when [error].
            /// </summary>
            /// <param name="error">The error.</param>
            public void OnError(Exception error)
            {
                _internalStream.OnError(error);
            }

            /// <summary>
            /// Called when [next].
            /// </summary>
            /// <param name="value">The value.</param>
            public void OnNext(TIn value)
            {
                _internalStream.OnNext(value);
            }

            #endregion IObserver Members
        }

        #endregion SubjectWrapper

        #endregion Nested Types
    }
}