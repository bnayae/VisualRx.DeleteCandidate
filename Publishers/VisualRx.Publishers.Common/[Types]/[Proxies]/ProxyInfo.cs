using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Publishers.Common
{
    /// <summary>
    /// Load information on the monitor proxy plug-ins
    /// </summary>
    public class ProxyInfo
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInfo" /> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="metadata">initialization information</param>
        /// <param name="error">The error.</param>
        public ProxyInfo(
            string providerName, 
            string metadata,
            string error = null)
        {
            ProviderName = providerName;
            Metadata = metadata;
            Error = error;
        }

        #endregion Ctor

        #region ProviderName

        /// <summary>
        /// Gets the proxy provider name.
        /// </summary>
        public string ProviderName { get; }

        #endregion ProviderName

        #region Metadata

        /// <summary>
        /// Gets the proxy initialize information
        /// </summary>
        public string Metadata { get; }

        #endregion Metadata

        #region Error

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; } 
        #endregion // Error
    }
}