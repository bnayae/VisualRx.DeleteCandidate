using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Listeners.Common
{
    /// <summary>
    /// Listener contract
    /// </summary>
    public interface IVisualRxListener: IDisposable 
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        Action<LogLevel, string, Exception> Log { get; set; }

        /// <summary>
        /// Gets the marble stream asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IObservable<Marble>> GetStreamAsync();
        
    }
}
