using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Listeners.Common
{
    /// <summary>
    /// Listener contract
    /// </summary>
    public interface IVisualRxListener: IConnectableObservable<Marble>, IDisposable 
    {
        Action<LogLevel, string, Exception> Log { get; set; }
    }
}
