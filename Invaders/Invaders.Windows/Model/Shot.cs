using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    class Shot
    {
        /// <summary>
        /// Speed of shot in px/sec
        /// </summary>
        public const double ShotPixelsPerSecond = 95;

        /// <summary>
        /// Shot location
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Shot size in pixels
        /// </summary>
        public static Size ShotSize { get { return new Size(2, 10); } }

        /// <summary>
        /// Shot movement direction
        /// </summary>
        private Direction _direction;
        public Direction Direction
        {
            get { return _direction; }
            private set { _direction = value; }
        }

        /// <summary>
        /// Contains informations about when shot moved last time
        /// </summary>
        private DateTime _lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;
            _lastMoved = DateTime.Now;
        }

        public void Move()
        {
            TimeSpan timeSinceLastMoved = DateTime.Now - _lastMoved;
            //Calculate distance from time (ms) and speed (px/s) in pixels   
            double distance = (ShotPixelsPerSecond / 1000) * timeSinceLastMoved.Milliseconds;
            if (Direction == Direction.Up)
                distance *= -1; // If shot is going upward then multiply distance by -1 to properly calculate it's coordinate 
            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
        
    }
}
