using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualRx.Contracts;

namespace VisualRx.Client.WPF
{
    static class TreeExtensions
    {
        public static void ToTree(this ObservableCollection<MarbleDiagramTree> tree, IEnumerable<Marble> items)
        {
            foreach (var item in items)
            {
                tree.ToTree(item);
            }
        }

        public static void ToTree(this ObservableCollection<MarbleDiagramTree> tree, Marble item)
        {
            var path = $"{item.StreamKey}.{item.MachineName}".Split('.');
            var mi = GetItem(tree, path[0]);
            for (int i = 1; i < path.Length; i++)
            {
                mi = GetItem(mi.Categories, path[i]);
            }
            var diagram = GetDiagram(mi, item.StreamKey);
            diagram.Items.Add(item);
        }

        private static MarbleDiagramTree GetItem(ObservableCollection<MarbleDiagramTree> tree, string name)
        {
            var mi = tree.FirstOrDefault(m => m.Name == name);
            if (mi == null)
            {
                mi = new MarbleDiagramTree()
                {
                    Name = name
                };
                tree.Add(mi);
            }

            return mi;
        }

        private static MarbleDiagram GetDiagram(MarbleDiagramTree tree, string name)
        {
            var mi = tree.Items.FirstOrDefault(m => m.Name == name);
            if (mi == null)
            {
                mi = new MarbleDiagram()
                {
                    Name = name
                };
                tree.Items.Add(mi);
            }

            return mi;
        }
    }
}
