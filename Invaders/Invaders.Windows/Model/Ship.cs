using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    /// <summary>
    /// Abstract ship representation.
    /// </summary>
    class Ship
    {
        /// <summary>
        /// Ship location.
        /// </summary>
        public Point Location { get; protected set; }
        /// <summary>
        /// Ship size.
        /// </summary>
        public Size Size { get; protected set; }
        /// <summary>
        /// Ship area.
        /// </summary>
        public Rect Area { get { return new Rect(Location, Size); } }

        public Ship(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        /// <summary>
        /// Moves ship in the given direction.
        /// </summary>
        /// <param name="direction"> Direction of movement</param>
        public abstract void Move(Direction direction);
        
    }
}
