using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIElement = Windows.UI.Xaml.UIElement;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

namespace Invaders.ViewModel
{
    class InvadersViewModel
    {
        /// <summary>
        /// Stores elements displayed on play area.
        /// </summary>
        private readonly ObservableCollection<UIElement> _sprites = new ObservableCollection<UIElement>();
        /// <summary>
        /// Provides possibility of event CollectionChanged subscription 
        /// </summary>
        public INotifyCollectionChanged Sprites { get { return _sprites; } }

        public InvadersViewModel()
        {
        
        }
    }
}
