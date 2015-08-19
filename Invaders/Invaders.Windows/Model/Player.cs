using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    /// <summary>
    /// Representation of player in the game.
    /// </summary>
    class Player : Ship
    {
        /// <summary>
        /// Player size in pixels.
        /// </summary>
        static public readonly Size PlayerSize { get; private set; }

        /// <summary>
        /// Movement speed.
        /// </summary>
        private readonly double _speed = 10.0;

        public Player(Point location, Size size):base(location,size)
        {
            PlayerSize = Size;
        }

        /// <summary>
        /// Moves player ship to the left or right.
        /// </summary>
        /// <param name="direction">Movement direction.</param>
        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    double newX = Location.X - _speed;

                    if (( newX > PlayerSize.Width/2)) // Movement in the left direction only decreases x coordinate
                        Location = new Point(newX, Location.Y);
                    break;
                case Direction.Right:
                    double playAreaWidth = InvadersModel.PlayAreaSize.Width;
                    double newX = Location.X + _speed;

                    if (newX < playAreaWidth - PlayerSize.Width / 2) 
                        Location = new Point(newX, Location.Y);
                    break;
                default:
                    break;
            }
        }
    }
}
