using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace Invaders.ViewModel
{
    class InvadersViewModel
    {
        public ObservableCollection<BitmapImage> Sprites { get; private set; }
    }
}
