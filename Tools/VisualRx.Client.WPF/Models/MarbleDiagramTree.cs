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
        public ObservableCollection<MarbleDiagramTree> Categories { get; set; } = new ObservableCollection<MarbleDiagramTree>();
        public ObservableCollection<MarbleDiagram> Items { get; set; } = new ObservableCollection<MarbleDiagram>();
        public IEnumerable<MarbleDiagram> ChildItems =>
            Items.Concat(Categories.SelectMany(c => c.ChildItems));
    }
}
