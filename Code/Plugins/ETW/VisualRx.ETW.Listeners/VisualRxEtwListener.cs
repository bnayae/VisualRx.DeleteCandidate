using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualRx.Contracts;
using VisualRx.ETW.Common;
using VisualRx.Listeners.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// TODO: Bnaya, 2015-10, #5, Consider session management strategy

namespace VisualRx.ETW.Listeners
{
    /// <summary>
    /// ETW listener
    /// </summary>
    /// 
    public class VisualRxEtwListener : IVisualRxListener
    {
        private static readonly Action<LogLevel, string, Exception> DefaultLog =
                (level, message, ex) => Trace.WriteLine($"VisualRx Listener LOG [{level}]: {message}\r\n\t{ex?.ToString()?.Replace(Environment.NewLine, Environment.NewLine + "\t")}");
        private readonly Subject<Marble> _subject = new Subject<Marble>();
        private readonly JsonSerializerSettings _setting;
        private TraceEventSession _session;
        private readonly Task _initialized;

        #region Ctor

        #region Overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualRxEtwListener" /> class.
        /// </summary>
        /// <param name="setting">The _setting.</param>
        /// <param name="log">The log.</param>
        public VisualRxEtwListener(
            JsonSerializerSettings setting = null,
            Action<LogLevel, string, Exception> log = null)
            :this(CancellationToken.None, setting, log)
        {
        }

        #endregion // Overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualRxEtwListener" /> class.
        /// </summary>
        /// <param name="token">will call dispose.</param>
        /// <param name="setting">The _setting.</param>
        /// <param name="log">The log.</param>
        public VisualRxEtwListener(
            CancellationToken token,
            JsonSerializerSettings setting = null,
            Action<LogLevel, string, Exception> log = null)
        {
            _setting = setting ?? new JsonSerializerSettings();
            if (setting == null)
            {
                _setting.Converters.Add(new StringEnumConverter());
                _setting.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            }
            Log = log ?? DefaultLog;

            // Today you have to be Admin to turn on ETW events (anyone can write ETW events).   
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                string warn = "To turn on ETW events you need to be Administrator, please run from an Admin process.";
                Debugger.Break();
                throw new SecurityException(warn);
            }

            _initialized = ListenAsync(token);
        }

        #endregion // Ctor

        #region Log
        /// <summary>
        /// Gets the log.
        /// </summary>
        public Action<LogLevel, string, Exception> Log { get; set; }

        #endregion // Log

        #region GetStreamAsync

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        public async Task<IObservable<Marble>> GetStreamAsync()
        {
            await _initialized;
            return _subject;
        }

        #endregion // GetStreamAsync

        #region ListenAsync

        /// <summary>
        /// Listens the asynchronous.
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private async Task ListenAsync(CancellationToken ct)
        {
            #region Log

            Log(LogLevel.Information, $@"Creating a '{EtwConstants.SessionName}' session
    Use 'logman query -ets' to see active sessions.
    Use 'logman query -ets ""{EtwConstants.SessionName}""' for details
    Use 'logman stop {EtwConstants.SessionName} -ets' to manually stop orphans.", null);

            #endregion // Log

            #region Documentation

            /*****************************************************
            *  To listen to ETW events you need a session, 
            *  which allows you to control which events will be produced
            *  Note that it is the session and not the source 
            *  that buffers events, and by default sessions will buffer
            *  64MB of events before dropping events.
            *  Thus even if you don't immediately connect up the source and
            *  read the events you should not lose them. 
            * 
            *  As mentioned below, sessions can outlive the process that created them.  
            *  Thus you may need a way of naming the session 
            *  so that you can 'reconnect' to it from another process.  
            *  This is what the name is for.  
            *  It can be anything, but it should be descriptive and unique.
            *  If you expect multiple versions of your program 
            *  to run simultaneously, you need to generate unique names 
            *  (e.g. add a process ID suffix) 
            *  however this is dangerous because you can leave data 
            *  collection on if the program ends unexpectedly.  
            *****************************************************/

            #endregion // Documentation
            _session = new TraceEventSession(EtwConstants.SessionName);
            _session.StopOnDispose = true;

            #region Documentation

            /*****************************************************
            *  BY DEFAULT ETW SESSIONS SURVIVE THE DEATH OF 
            *  THE PROESS THAT CREATES THEM! 
            *  
            *  Unlike most other resources on the system, 
            *  ETW session live beyond the lifetime of the process 
            *  that created them. 
            *  This is very useful in some scenarios, but also 
            *  creates the very real possibility of leaving 
            *  'orphan' sessions running.  
            * 
            *  To help avoid this by default TraceEventSession 
            *  sets 'StopOnDispose' so that it will stop
            *  the ETW session if the TraceEventSession dies.   
            *  Thus executions that 'clean up' the TraceEventSession
            *  will clean up the ETW session.   
            *  This covers many cases (including throwing exceptions)
            *   
            *  However if the process is killed manually (including control C)
            *  this cleanup will not happen.  
            *
            *  Thus best practices include
            *      * Add a Control C handler that calls session.Dispose() 
            *        so it gets cleaned up in this common case
            *      * use the same session name (say your program name) 
            *        run-to-run so you don't create many orphans. 
            * 
            *  By default TraceEventSessions are in 'create' 
            *  mode where it assumes you want to create a new session.
            *  In this mode if a session already exists, 
            *  it is stopped and the new one is created.   
            *  
            *  Here we install the Control C handler.   
            *  It is OK if Dispose is called more than once.  
            *****************************************************/
            #endregion // Documentation         
            Console.CancelKeyPress += (sender, e) => _session.Dispose();

            #region UnhandledEvents

            _session.Source.UnhandledEvents += (TraceEvent data) =>
            {
                if ((int)data.ID != 0xFFFE)         // The EventSource manifest events show up as unhanded, filter them out.
                    Log(LogLevel.Warning, $"Unhandled {data.ToString()}", null);
            };

            #endregion // UnhandledEvents

            #region _session.EnableProvider(...)

            #region Documentation

            /*****************************************************
            *  At this point we have created a TraceEventSession, 
            *  hooked it up to a TraceEventSource, and hooked the
            *  TraceEventSource to a TraceEventParser 
            *  (you can do several of these), and then hooked up callbacks
            *  up to the TraceEventParser (again you can have several).  
            *  However we have NOT actually told any
            *  provider (EventSources) to actually send any events 
            *  to our TraceEventSession.  
            *  We do that now.  
            *  Enable my provider, you can call many of these 
            *  on the same session to get events from other providers.  
            *  Because this EventSource did not define any keywords, 
            *  I can only turn on all events or none.  
            *  var restarted = session.EnableProvider(ETW_PROVIDER_NAME);
            *****************************************************/
            #endregion // Documentation         

            var restarted = _session.EnableProvider(
                VisualRxEventSource.ProviderGuid,
                TraceEventLevel.Always);

            #region Validation

            if (restarted) // Generally you don't bother with this warning. 
            {
                Log(LogLevel.Warning,
$@"The session {EtwConstants.SessionName} was already active, 
it has been restarted.", null);
            }

            #endregion // Validation

            #endregion // _session.EnableProvider(...)

            #region IObservable<TraceEvent> sending = ...

            IObservable<TraceEvent> sending =
                    _session.Source.Dynamic.Observe(
                        VisualRxEventSource.ProviderName,
                        nameof(VisualRxEventSource.Send));
            sending.Select(m =>
                {
                    var json = m.FormattedMessage;
                    var marble = JsonConvert.DeserializeObject<Marble>(json, _setting);
                    return marble;
                })
                .Subscribe(_subject);

            #endregion // IObservable<TraceEvent> sending = ... 

            // cancel the session
            ct.Register(() => _session.Source.Dispose());

            // go into a loop processing events can calling the callbacks.  
            // Because this is live data (not from a file)
            // processing never completes by itself, 
            // but only because someone called 'source.Dispose()'.  
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {
                    _session.Source.Process();
                }
                catch (ThreadAbortException) { }
                Log(LogLevel.Information, $"Session [{EtwConstants.SessionName}] Stopped.", null);
            }, TaskCreationOptions.LongRunning);

            await Task.Delay(10); // make sure that session.Source.Process() is listening (avoid racing)
        }

        private void Source_AllEvents(TraceEvent obj)
        {
            throw new NotImplementedException();
        }

        #endregion // ListenAsync

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
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_session == null)
                return;
            _session.Dispose();
            _subject.Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VisualRxEtwListener" /> class.
        /// </summary>
        ~VisualRxEtwListener()
        {
            Dispose(false);
        }

        #endregion // Dispose Pattern
    }
}
