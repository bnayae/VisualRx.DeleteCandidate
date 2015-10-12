
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
        void Write(string text);
    }
}