using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace VisualRx.Contracts
{
    /// <summary>
    /// marble kind
    /// </summary>
    public enum MarbleKind
    {
        /// <summary>
        /// On next
        /// </summary>
        OnNext = 0,

        /// <summary>
        /// On error
        /// </summary>
        OnError = 1,

        /// <summary>
        /// On complete
        /// </summary>
        OnCompleted = 2,
    }
}