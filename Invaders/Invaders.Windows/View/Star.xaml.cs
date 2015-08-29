using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Invaders.View
{
    /// <summary>
    /// Big star representation
    /// </summary>
    public sealed partial class Star : UserControl
    {
        public Star()
        {
            this.InitializeComponent();
        }

            
        public Brush Fill
        {
            get { return star.Fill; }
            set { Fill = value; }
        }


        public void SetFill(SolidColorBrush solidColorBrush)
        {
            star.Fill = solidColorBrush;
        }
    }
}
