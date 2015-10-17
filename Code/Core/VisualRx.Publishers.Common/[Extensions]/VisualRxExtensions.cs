#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using VisualRx.Contracts;
using VisualRx.Publishers.Common;

#endregion Using

namespace System.Reactive.Linq
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class VisualRxExtensions
    {
        private static int _order = 0;
        
        #region Monitor Many

        #region Overloads

        /// <summary>
        /// Monitors many streams (like window).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IObservable<T>> MonitorMany<T>(
            this IObservable<IObservable<T>> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            var result = MonitorMany(instance, name, null, 
                (Func<T, object>)null, setting);
            return result;
        }

        /// <summary>
        /// Monitors many streams (like window).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingBaseIndex">Index of the ordering base.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IObservable<T>> MonitorMany<T>(
            this IObservable<IObservable<T>> instance,
            string name,
            double orderingBaseIndex,
            VisualRxPublishersSettings setting = null)
        {
            var result = MonitorMany(instance, name, orderingBaseIndex, 
                (Func<T, object>)null, setting);
            return result;
        }

        #endregion // Overloads

        /// <summary>
        /// Monitors many streams (like window).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingBaseIndex">Index of the ordering base.</param>
        /// <param name="surrogate">The surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IObservable<T>> MonitorMany<T>(
            this IObservable<IObservable<T>> instance,
            string name,
            double? orderingBaseIndex,
            Func<T, object> surrogate,
            VisualRxPublishersSettings setting = null)
        {
            setting = setting ?? VisualRxPublishersSettings.Default;
            double order = orderingBaseIndex ?? Interlocked.Increment(ref _order);
            int index = 0;
            var xs = from obs in instance
                     let idx = Interlocked.Increment(ref index)
                     select obs.Monitor(name + " " + idx, order + (idx / 100000.0), surrogate, setting);
            return xs;
        }

        #endregion Monitor Many

        #region Monitor Group

        #region Overloads

        /// <summary>
        /// Monitor Group by stream
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IGroupedObservable<TKey, TElement>> MonitorGroup<TKey, TElement>(
            this IObservable<IGroupedObservable<TKey, TElement>> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            return MonitorGroup(instance, name, null,
                (Func<TElement, object>)null, setting);
        }

        /// <summary>
        /// Monitor Group by stream
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingBaseIndex">Index of the ordering base.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IGroupedObservable<TKey, TElement>> MonitorGroup<TKey, TElement>(
            this IObservable<IGroupedObservable<TKey, TElement>> instance,
            string name,
            double orderingBaseIndex,
            VisualRxPublishersSettings setting = null)
        {
            return MonitorGroup(instance, 
                name, orderingBaseIndex,
                (Func<TElement, object>)null,
                setting);
        }

        #endregion // Overloads

        /// <summary>
        /// Monitor Group by stream
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingBaseIndex">Index of the ordering base.</param>
        /// <param name="elementSurrogate">The element surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<IGroupedObservable<TKey, TElement>> MonitorGroup<TKey, TElement>(
            this IObservable<IGroupedObservable<TKey, TElement>> instance,
            string name,
            double? orderingBaseIndex,
            Func<TElement, object> elementSurrogate,
            VisualRxPublishersSettings setting = null)
        {
            setting = setting ?? VisualRxPublishersSettings.Default;
            Func<IGroupedObservable<TKey, TElement>, object> keySurrogate =
                g => $"Key = {g.Key}";

            double order = orderingBaseIndex ?? Interlocked.Increment(ref _order);
            instance = instance.Monitor(name + " (keys)", order,
                keySurrogate, setting);

            int index = 0;
            var xs = from g in instance
                     let idx = Interlocked.Increment(ref index)
                     let ord = order + (idx / 100000.0)
                     select new GroupedMonitored<TKey, TElement>(
                                        g, $"{name}:{g.Key} ({idx})", ord, elementSurrogate);

            return xs;
        }


        #endregion Monitor Group

        #region Monitor IConnectableObservable

        #region Overloads

        /// <summary>
        /// Monitor IConnectableObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IConnectableObservable<T> Monitor<T>(
            this IConnectableObservable<T> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            var result = Monitor(instance, name, null,
                (Func<T, object>)null, setting);
            return result;
        }

        /// <summary>
        /// Monitor IConnectableObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IConnectableObservable<T> Monitor<T>(
            this IConnectableObservable<T> instance,
            string name,
            double orderingIndex,
            VisualRxPublishersSettings setting = null)
        {
            var result = Monitor<T>(instance, name, orderingIndex, 
                (Func<T, object>)null, setting);
            return result;
        }

        #endregion Overloads

        /// <summary>
        /// Monitor IConnectableObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="surrogate">The surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IConnectableObservable<T> Monitor<T>(
            this IConnectableObservable<T> instance,
            string name,
            double? orderingIndex,
            Func<T, object> surrogate,
            VisualRxPublishersSettings setting = null)
        {
            setting = setting ?? VisualRxPublishersSettings.Default;
            double order = orderingIndex ?? Interlocked.Increment(ref _order);
            var monitor = new StreamChannel<T>(
                setting, name, order, surrogate);

            var watcher = monitor.AttachTo(instance);
            return watcher;
        }

        #endregion Monitor IConnectableObservable

        #region Monitor ISubject

        #region Overloads

        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<T> Monitor<T>(
            this ISubject<T> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            return Monitor(
                instance,
                name,
                null,
                (Func<T, object>)null,
                setting);
        }


        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<T> Monitor<T>(
            this ISubject<T> instance,
            string name,
            double orderingIndex,
            VisualRxPublishersSettings setting = null)
        {
            return Monitor<T>(
                instance,
                name,
                orderingIndex,
                (Func<T, object>)null,
                setting);
        }

        #endregion Overloads

        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="surrogate">The surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<T> Monitor<T>(
            this ISubject<T> instance,
            string name,
            double? orderingIndex,
            Func<T, object> surrogate,
            VisualRxPublishersSettings setting = null)
        {
            setting = setting ?? VisualRxPublishersSettings.Default;
            double order = orderingIndex ?? Interlocked.Increment(ref _order);
            var monitor = new StreamChannel<T>(
                setting, name, order, surrogate);

            var watcher = monitor.AttachTo(instance);
            return watcher;

        }

        #region Overloads

        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="TIn">The type of the in.</typeparam>
        /// <typeparam name="TOut">The type of the out.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<TIn, TOut> Monitor<TIn, TOut>(
            this ISubject<TIn, TOut> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            var result = Monitor(instance, name, null, 
                (Func<TOut, object>)null, setting);
            return result;
        }

        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="TIn">The type of the in.</typeparam>
        /// <typeparam name="TOut">The type of the out.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<TIn, TOut> Monitor<TIn, TOut>(
            this ISubject<TIn, TOut> instance,
            string name,
            double orderingIndex,
            VisualRxPublishersSettings setting = null)
        {
            var result = Monitor<TIn, TOut>(instance, name, orderingIndex,
                (Func<TOut, object>)null, setting);
            return result;
        }

        #endregion Overloads

        /// <summary>
        /// Monitor ISubject stream
        /// </summary>
        /// <typeparam name="TIn">The type of the in.</typeparam>
        /// <typeparam name="TOut">The type of the out.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="surrogate">The surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static ISubject<TIn, TOut> Monitor<TIn, TOut>(
            this ISubject<TIn, TOut> instance,
            string name,
            double? orderingIndex,
            Func<TOut, object> surrogate,
            VisualRxPublishersSettings setting = null)
        {
            double order = orderingIndex ?? Interlocked.Increment(ref _order);
            var monitor = new StreamChannel<TOut>(
                setting, name, order, surrogate);

            var watcher = monitor.AttachTo(instance);
            return watcher;
        }

        #endregion Monitor ISubject

        #region Monitor

        #region Overloads

        /// <summary>
        /// Monitor IObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<T> Monitor<T>(
            this IObservable<T> instance,
            string name,
            VisualRxPublishersSettings setting = null)
        {
            var result = Monitor<T>(
                instance, name, null,
                (Func<T, object>)null, setting);
            return result;
        }

        /// <summary>
        /// Monitor IObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">the ordering of the marble diagram</param>
        /// <returns></returns>
        public static IObservable<T> Monitor<T>(
            this IObservable<T> instance,
            string name,
            double orderingIndex)
        {
            var result = Monitor<T>(
                instance, name, orderingIndex,
                (Func<T, object>)null);
            return result;
        }

        #endregion Overloads

        /// <summary>
        /// Monitor IObservable stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <param name="orderingIndex">Index of the ordering.</param>
        /// <param name="surrogate">a surrogate.</param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IObservable<T> Monitor<T>(
            this IObservable<T> instance,
            string name,
            double? orderingIndex,
            Func<T, object> surrogate,
            VisualRxPublishersSettings setting = null)
        {
            setting = setting ?? VisualRxPublishersSettings.Default;
            double order = orderingIndex ?? Interlocked.Increment(ref _order);
            var monitor = new StreamChannel<T>(
                 setting, name, order, surrogate);

            var watcher = monitor.AttachTo(instance);
            return watcher;
        }

        #endregion Monitor

        #region GroupedMonitored (nested class)

        /// <summary>
        /// Group wrapper
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        private class GroupedMonitored<TKey, TElement> : IGroupedObservable<TKey, TElement>
        {
            private readonly TKey _key;
            private readonly IObservable<TElement> _groupStream;

            #region Ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="GroupedMonitored{TKey, TElement}" /> class.
            /// </summary>
            /// <param name="group">The group.</param>
            /// <param name="name">The name.</param>
            /// <param name="order">The order.</param>
            /// <param name="surrogate">The surrogate.</param>
            public GroupedMonitored(
                IGroupedObservable<TKey, TElement> group,
                string name,
                double order,
                Func<TElement, object> surrogate)
            {
                _key = group.Key;
                _groupStream = group.Monitor(name, order, surrogate);
            }

            #endregion // Ctor

            #region Key

            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <value>
            /// The key.
            /// </value>
            /// <exception cref="System.NotImplementedException"></exception>
            public TKey Key => _key;

            #endregion // Key

            #region Subscribe

            /// <summary>
            /// Subscribes the specified observer.
            /// </summary>
            /// <param name="observer">The observer.</param>
            /// <returns></returns>
            public IDisposable Subscribe(IObserver<TElement> observer)
            {
                return _groupStream.Subscribe(observer);
            }

            #endregion // Subscribe
        }

        #endregion // GroupedMonitored (nested class)
    }
}