using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualRx.Contracts;
using VisualRx.ETW.Listeners;

namespace VisualRx.Client.WPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public SimpleObservableCollection<MarbleDiagramTree> Tree { get; } = new SimpleObservableCollection<MarbleDiagramTree>();

        public MainViewModel()
        {
            var t = Initialize();
        }

        private async Task Initialize()
        {
            var listener = new VisualRxEtwListener();

            IObservable<Marble> listenStream =
                await listener.GetStreamAsync();
            listenStream
                .Subscribe(marble => Tree.ToTree(marble));
        }
    }
}
