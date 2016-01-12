using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Client.WPF
{
    public class MarbleDiagram
    {
        public string Name { get; set; }
        public ObservableCollection<Marble> Items { get; set; } = new ObservableCollection<Marble>();
    }
}
