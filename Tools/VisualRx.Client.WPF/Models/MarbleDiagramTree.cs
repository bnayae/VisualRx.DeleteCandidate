using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualRx.Client.WPF
{
    public class MarbleDiagramTree
    {
        public string Name { get; set; }
        public SimpleObservableCollection<MarbleDiagramTree> Categories { get; set; } = new SimpleObservableCollection<MarbleDiagramTree>();
        public SimpleObservableCollection<MarbleDiagram> Items { get; set; } = new SimpleObservableCollection<MarbleDiagram>();
        public IEnumerable<MarbleDiagram> ChildItems =>
            Items.Concat(Categories.SelectMany(c => c.ChildItems));
    }
}
