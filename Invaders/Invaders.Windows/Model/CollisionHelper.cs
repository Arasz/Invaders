using System;
using Windows.Foundation;

namespace Invaders.Model
{
    internal static class CollisionHelper
    {
        internal static bool CheckCollision(Rect area, Point location)
        {
            return area.Contains(location);
        }

        internal static bool CheckCollision(Rect rect1, Rect rect2)
        {
            rect1.Intersect(rect2);
            return rect1.Height > 0 && rect1.Width > 0 ? true : false;
        }
    }
}