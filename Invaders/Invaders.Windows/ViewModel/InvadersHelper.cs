using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using UIElement = Windows.UI.Xaml.UIElement;
using Invaders.View;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;

namespace Invaders.ViewModel
{
    /// <summary>
    /// Supports view model class
    /// </summary>
    static class InvadersHelper
    {
        private static Random _random = new Random();

        static public UIElement StarControlFactory(double scale)
        {
            // Draw star shape
            Array starShapes = Enum.GetValues(typeof(StarShape));
            StarShape randomShape = (StarShape)starShapes.GetValue(_random.Next(0, starShapes.Length));

            UIElement starControl; //ISSUE: Is this type good?

            switch (randomShape)
            {
                case StarShape.Rectangle:
                    starControl = new Rectangle()
                    {Fill = new SolidColorBrush(RandomColor()) };
                
                    break;
                case StarShape.Ellipse:
                    starControl = new Ellipse()
                    { Fill = new SolidColorBrush(RandomColor()) };
                    break;
                case StarShape.Star:
                    starControl = new Star()
                    { Fill = new SolidColorBrush(RandomColor()) };
                    break;
                default:
                    starControl = new Star()
                    { Fill = new SolidColorBrush(RandomColor()) };
                    break;
            }

            return starControl;
        }

        static public Rectangle ScanLineFactory(double y, double width, double scale)
        {
            return new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.White),
                Height = 2,
                Opacity = 1,
                MinWidth = width * scale,
                RenderTransformOrigin = new Windows.Foundation.Point(0, y),
            };
        }


        private static Color RandomColor()
        {
            // Get Colors type properties
            IEnumerable<PropertyInfo> colors = typeof(Colors).GetRuntimeProperties();
            // Draw color
            return (Color)colors.ElementAt(_random.Next(0, colors.Count())).GetValue(null);
        }
    }
}
