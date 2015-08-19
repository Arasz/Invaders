using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    /// <summary>
    /// Invader model
    /// </summary>
    class Invader:Ship
    {
        private static readonly double _speed = 10; 

        private static readonly Size _invaderSize = new Size(15,25);
        /// <summary>
        /// Returns default invader size
        /// </summary>
        public static Size InvaderSize { get { return _invaderSize; } }

        /// <summary>
        /// Invader type
        /// </summary>
        public InvaderType InvaderType {get; private set;}

        /// <summary>
        /// Score collected by player for shooting invader
        /// </summary>
        public int Score { get; private set; }


        public Invader(InvaderType invaderType, int score, Point location, Size size):base(location, size)
        {
            InvaderType = invaderType;
            Score = score;
        }

        /// <summary>
        /// Moves invader ships in given direction, excluding top
        /// </summary>
        /// <param name="direction"> Move direction </param>
        public override void Move(Direction direction)
        {
            double newCoordinate;
            switch (direction)
            {
                case Direction.Left:
                    newCoordinate = Location.X - _speed;
                    if (newCoordinate > Size.Width / 2)
                        Location = new Point(newCoordinate, Location.Y);
                    break;
                case Direction.Right:
                    newCoordinate = Location.X + _speed;
                    if (newCoordinate < (InvadersModel.PlayAreaSize.Width - Size.Width/2))
                        Location = new Point(newCoordinate, Location.Y);
                    break;
                case Direction.Up:
                    newCoordinate = Location.Y + _speed;
                    if (newCoordinate > (InvadersModel.PlayAreaSize.Height - Size.Height / 2))
                        Location = new Point(Location.X, newCoordinate);
                    break;
                case Direction.Down:
                    newCoordinate = Location.Y - _speed;
                    if (newCoordinate > Size.Height / 2)
                        Location = new Point(Location.X, newCoordinate);
                    break;
                default:
                    break;
            }
        }
    }
}
