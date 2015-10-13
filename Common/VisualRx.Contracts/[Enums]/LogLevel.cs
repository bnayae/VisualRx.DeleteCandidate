using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace VisualRx.Contracts
{
    /// <summary>
    /// log level
    /// </summary>
    public enum LogLevel
    {
        Critical = 1,
        Error = 3,
        Warning = 7,
        Information = 15,
        Verbose = 31,
    }
}