#region Using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using VisualRx.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion Using

namespace VisualRx.Contracts
{
    /// <summary>
    /// Candidate data before it is constructed into a marble
    /// Used by Enable / Disable Filters
    /// </summary>
    public struct MarbleCandidate
    {
        public MarbleCandidate(string name, MarbleKind kind, string[] keywords)
        {
            Name = name;
            Kind = kind;
            Keywords = keywords;
        }

        public string Name { get; }
        public MarbleKind Kind { get; }
        public string[] Keywords { get; }
    }

}