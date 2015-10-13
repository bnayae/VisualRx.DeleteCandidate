using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualRx.Contracts;
using VisualRx.Listeners.Common;

namespace VisualRx.ETW.Listeners
{
    /// <summary>
    /// ETW listener
    /// </summary>
    /// 
    public class VisualRxEtwListener : IVisualRxListener
    {
        private const string SESSION_NAME = "VisualRx.V1";
        private static readonly Action<LogLevel, string, Exception> DefaultLog =
                (level, message, ex) => Trace.WriteLine($"VisualRx Listener LOG [{message}]: {message}\r\n\t{ex?.ToString()?.Replace(Environment.NewLine, Environment.NewLine + "\t")}");
        private readonly Subject<Marble> _subject = new Subject<Marble>();
        private bool _disposed = false;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualRxEtwListener"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public VisualRxEtwListener(
            Action<LogLevel, string, Exception> log = null)
        {
            Log = log ?? DefaultLog;
        }

        #endregion // Ctor

        #region Log
        /// <summary>
        /// Gets the log.
        /// </summary>
        public Action<LogLevel, string, Exception> Log { get; set; }

        #endregion // Log

        #region Subscribe

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<Marble> observer)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(VisualRxEtwListener));

            return _subject.Subscribe(observer);
        }

        #endregion // Subscribe

        #region Connect

        /// <summary>
        /// Start listening.
        /// </summary>
        /// <returns></returns>
        public IDisposable Connect()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(VisualRxEtwListener));

            // Today you have to be Admin to turn on ETW events (anyone can write ETW events).   
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                string log = "To turn on ETW events you need to be Administrator, please run from an Admin process.";
                Debugger.Break();
                throw new SecurityException(log);
            }

            var disp = new CancellationDisposable();
            Listen(disp.Token);
            return disp;
        }

        #endregion // Connect

        static void Listen(CancellationToken ct)
        {
//            #region Log

//            Log(LogLevel.Information, $@"Creating a '{SESSION_NAME}' session
//Use 'logman query -ets' to see active sessions.
//Use 'logman stop {SESSION_NAME} -ets' to manually stop orphans.", null);

//            #endregion // Log

//            #region Documentation

//            /*****************************************************
//            *  To listen to ETW events you need a session, 
//            *  which allows you to control which events will be produced
//            *  Note that it is the session and not the source 
//            *  that buffers events, and by default sessions will buffer
//            *  64MB of events before dropping events.
//            *  Thus even if you don't immediately connect up the source and
//            *  read the events you should not lose them. 
//            * 
//            *  As mentioned below, sessions can outlive the process that created them.  
//            *  Thus you may need a way of naming the session 
//            *  so that you can 'reconnect' to it from another process.  
//            *  This is what the name is for.  
//            *  It can be anything, but it should be descriptive and unique.
//            *  If you expect multiple versions of your program 
//            *  to run simultaneously, you need to generate unique names 
//            *  (e.g. add a process ID suffix) 
//            *  however this is dangerous because you can leave data 
//            *  collection on if the program ends unexpectedly.  
//            *****************************************************/

//            #endregion // Documentation
//            using (var session = new TraceEventSession(SESSION_NAME))
//            {
//                #region Documentation

//                /*****************************************************
//                *  BY DEFAULT ETW SESSIONS SURVIVE THE DEATH OF 
//                *  THE PROESS THAT CREATES THEM! 
//                *  
//                *  Unlike most other resources on the system, 
//                *  ETW session live beyond the lifetime of the process 
//                *  that created them. 
//                *  This is very useful in some scenarios, but also 
//                *  creates the very real possibility of leaving 
//                *  'orphan' sessions running.  
//                * 
//                *  To help avoid this by default TraceEventSession 
//                *  sets 'StopOnDispose' so that it will stop
//                *  the ETW session if the TraceEventSession dies.   
//                *  Thus executions that 'clean up' the TraceEventSession
//                *  will clean up the ETW session.   
//                *  This covers many cases (including throwing exceptions)
//                *   
//                *  However if the process is killed manually (including control C)
//                *  this cleanup will not happen.  
//                *
//                *  Thus best practices include
//                *      * Add a Control C handler that calls session.Dispose() 
//                *        so it gets cleaned up in this common case
//                *      * use the same session name (say your program name) 
//                *        run-to-run so you don't create many orphans. 
//                * 
//                *  By default TraceEventSessions are in 'create' 
//                *  mode where it assumes you want to create a new session.
//                *  In this mode if a session already exists, 
//                *  it is stopped and the new one is created.   
//                *  
//                *  Here we install the Control C handler.   
//                *  It is OK if Dispose is called more than once.  
//                *****************************************************/
//                #endregion // Documentation         
//                Console.CancelKeyPress += (sender, e) => session.Dispose();

//                #region // session.Source.Dynamic.All += ...

////                #region Documentation

////                // For debugging, and demo purposes, hook up a callback for every event that 'Dynamic' knows about (this is not EVERY
////                // event only those know about by DynamiceTraceEventParser).   However the 'UnhandledEvents' handler below will catch
////                // the other ones. 

////                #endregion // Documentation
////                session.Source.Dynamic.All += (TraceEvent data) =>
////                {
////                    // ETW buffers events and only delivers them after buffering up for some amount of time.  Thus 
////                    // there is a small delay of about 2-4 seconds between the timestamp on the event (which is very 
////                    // accurate), and the time we actually get the event.  We measure that delay here.     
////                    var delay = (DateTime.Now - data.TimeStamp).TotalSeconds;
////                    WriteLine($"Got Event Delay={delay:f1}sec", ConsoleColor.Gray);
////                    WriteLine($@"{data.EventName}: ID: {data.ID}, Level: {data.Level}, Version: {data.Version}
////    Activity: {data.ActivityID}, Related Activity: {data.RelatedActivityID}, Index: {data.EventIndex}
////    Time:       {data.TimeStamp}, Relative: {data.TimeStampRelativeMSec} (sec)
////    Provider:   {data.ProviderName} ({data.ProviderGuid})
////    Process:    {data.ProcessName} ({data.ProcessID}), Thread Id: {data.ThreadID}
////    Task:       {data.TaskName} ({data.Task}), Opcode:{data.OpcodeName} ({data.Opcode})
////    Logged by Core: {data.ProcessorNumber}
////    Keywords: {data.Keywords}
////", ConsoleColor.White);
////                    WriteLine(data.FormattedMessage, ConsoleColor.Yellow);
////                    Console.WriteLine();
////                };

//                #endregion // session.Source.Dynamic.All += ...

//                #region // session.Source.UnhandledEvents += ...

//                //// The callback above will only be called for events the parser recognizes (in the case of DynamicTraceEventParser, EventSources)
//                //// It is sometimes useful to see the other events that are not otherwise being handled.  The source knows about these and you 
//                //// can ask the source to send them to you like this.  
//                //session.Source.UnhandledEvents += (TraceEvent data) =>
//                //{
//                //    if ((int)data.ID != 0xFFFE)         // The EventSource manifest events show up as unhanded, filter them out.
//                //        WriteLine("GOT UNHANDLED EVENT: " + data.Dump(), ConsoleColor.Magenta);
//                //};

//                #endregion // session.Source.UnhandledEvents += ...

//                #region AddCallbackForProviderEvent (specific)

//                session.Source.Dynamic.AddCallbackForProviderEvent(
//                      PlaygoundContracts.ETW_PROVIDER_NAME,
//                      nameof(CustomEventSource.StartX),
//                      (TraceEvent data) =>
//                      {
//                          WriteLine($"START Event (#{data.PayloadByName("value")}), {data.FormattedMessage}", ConsoleColor.Yellow);
//                      });

//                session.Source.Dynamic.AddCallbackForProviderEvent(
//                      PlaygoundContracts.ETW_PROVIDER_NAME,
//                      nameof(CustomEventSource.StopX),
//                      (TraceEvent data) =>
//                      {
//                          WriteLine($"STOP Event (Related = {data.RelatedActivityID})", ConsoleColor.Yellow);
//                      });

//                //session.Source.Dynamic.AddCallbackForProviderEvent(
//                //      PlaygoundContracts.ETW_PROVIDER_NAME, 
//                //      nameof(CustomEventSource.Send),
//                //      (TraceEvent data) =>
//                //      {
//                //          WriteLine($"Send {string.Join(",", data.PayloadNames)}", ConsoleColor.Green);
//                //      });

//                #endregion // AddCallbackForProviderEvent (specific)

//                //session.Source.Dynamic.Observe<TraceEvent>().Select(m => m.EventName)
//                //    .Subscribe(m => WriteLine(m, ConsoleColor.Green));

//                #region session.EnableProvider(...)

//                // At this point we have created a TraceEventSession, hooked it up to a TraceEventSource, and hooked the
//                // TraceEventSource to a TraceEventParser (you can do several of these), and then hooked up callbacks
//                // up to the TraceEventParser (again you can have several).  However we have NOT actually told any
//                // provider (EventSources) to actually send any events to our TraceEventSession.  
//                // We do that now.  

//                // Enable my provider, you can call many of these on the same session to get events from other providers.  
//                // Because this EventSource did not define any keywords, I can only turn on all events or none.  
//                //var restarted = session.EnableProvider(PlaygoundContracts.ETW_PROVIDER_NAME);
//                var restarted = session.EnableProvider(
//                    PlaygoundContracts.ETW_PROVIDER_GUID,
//                    TraceEventLevel.Always);
//                //var restarted = session.EnableProvider(
//                //    PlaygoundContracts.ETW_PROVIDER_NAME,
//                //    TraceEventLevel.Always);

//                if (restarted)      // Generally you don't bother with this warning, but for the demo we do. 
//                    WriteLine($"The session {SESSION_NAME} was already active, it has been restarted.", ConsoleColor.Magenta);

//                #endregion // session.EnableProvider(...)

//                #region IObservable<TraceEvent> hits = ...

//                IObservable<TraceEvent> hits =
//                        session.Source.Dynamic.Observe(
//                            PlaygoundContracts.ETW_PROVIDER_NAME,
//                            nameof(CustomEventSource.Hit));
//                hits.Subscribe(onNext: data => WriteLine($"RX: '{data.EventName}'", ConsoleColor.White));

//                #endregion // IObservable<TraceEvent> hits = ... 

//                #region IObservable<TraceEvent> anys = ...

//                IObservable<TraceEvent> anys =
//                        session.Source.Dynamic.Observe((provider, name) => EventFilterResponse.AcceptEvent);
//                anys.Subscribe(onNext: data => WriteLine($"RX-any: '{data.EventName}'", ConsoleColor.Green));

//                #endregion // IObservable<TraceEvent> anys = ...

//                #region Rx (combine)

//                IObservable<TraceEvent> starts =
//                        session.Source.Dynamic.Observe(
//                                            PlaygoundContracts.ETW_PROVIDER_NAME,
//                                            $"{nameof(CustomEventSource.StartX)}/Start");
//                IObservable<TraceEvent> stops =
//                        session.Source.Dynamic.Observe(
//                                            PlaygoundContracts.ETW_PROVIDER_NAME,
//                                            $"{nameof(CustomEventSource.StartX)}/Stop");
//                var xs = from duration in starts.CombineLatest(stops, (start, stop) => stop.TimeStampRelativeMSec - start.TimeStampRelativeMSec)
//                         where duration > 0
//                         select TimeSpan.FromMilliseconds(duration);

//                xs.Subscribe(m => WriteLine($"Duration = {m.TotalSeconds:N3} sec", ConsoleColor.Yellow));

//                //Func<IGroupedObservable<TraceEventTask, TraceEvent>, IObservable<TraceEvent>> durationSelector =
//                //            g => stops.Where(m => m.Task == g.Key)
//                //                        .Timeout(TimeSpan.FromSeconds(3))
//                //                        .OnErrorResumeNext(Observable.Return<TraceEvent>(null));
//                //var xys = from g in starts.GroupByUntil(d => d.Task, durationSelector)
//                //                from pair in g.FirstAsync().Zip(stops.Where(m => m.Task == g.Key)), )

//                #endregion // Rx (combine)


//                // cancel the session
//                ct.Register(() => session.Source.Dispose());

//                // go into a loop processing events can calling the callbacks.  Because this is live data (not from a file)
//                // processing never completes by itself, but only because someone called 'source.Dispose()'.  
//                session.Source.Process();
//                WriteLine("Stopping the collection of events.", ConsoleColor.Blue);
//            }
        }

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
        public void Dispose(bool disposing)
        {
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
