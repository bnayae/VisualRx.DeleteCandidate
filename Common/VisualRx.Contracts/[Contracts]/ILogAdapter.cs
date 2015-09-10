
using System;

namespace VisualRx.Contracts
{
    /// <summary>
    /// Loggers plug in contract
    /// </summary>
    public interface ILogAdapter
    {
        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        void Warn(string text, Exception ex = null);

        /// <summary>
        /// Errors the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="ex">The ex.</param>
        void Error(string text, Exception ex = null);
    }
}