using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    /// <summary>
    /// Invaders game model. 
    /// </summary>
    class InvadersModel
    {
        #region Events
        /// <summary>
        /// Informs about any change in the ships state
        /// </summary>
        public event EventHandler<ShipChangedEventArgs> ShipChanged;
        public void OnShipChanged(Ship shipUpdated, bool killed)
        {
            var shipChanged = ShipChanged;
            if (shipChanged != null)
                shipChanged(this, new ShipChangedEventArgs(shipUpdated, killed));
        }

        /// <summary>
        /// Informs about shot move
        /// </summary>
        public event EventHandler<ShotMovedEventArgs> ShotMoved;
        public void OnShotMoved(Shot shot, bool disappeared)
        {
            var shotMoved = ShotMoved;
            if (shotMoved != null)
                shotMoved(this, new ShotMovedEventArgs(shot, disappeared));
        }

        /// <summary>
        /// Informs about change in the stars state
        /// </summary>
        public event EventHandler<StarChangedEventArgs> StarChanged;
        public void OnStarChanged(Point star, bool disappeared)
        {
            var starChanged = StarChanged;
            if (starChanged != null)
                starChanged(this, new StarChangedEventArgs(star, disappeared));
        }
        #endregion

        private static readonly Size _playAreaSize = new Size(300,400);
        /// <summary>
        /// Size of game play area
        /// </summary>
        public static Size PlayAreaSize { get{ return _playAreaSize;} }

        /// <summary>
        /// Amount of player shots that can be present in the game area
        /// </summary>
        public const int MaximumPlayerShots = 3;

        /// <summary>
        /// Initial amount of background stars
        /// </summary>
        public const int InitialStarCount = 50;

        /// <summary>
        /// Random number generator
        /// </summary>
        private readonly Random _random = new Random();

        /// <summary>
        /// Current wave number
        /// </summary>
        public int Wave { get; private set; }
        /// <summary>
        /// Player score
        /// </summary>
        public int Score { get; private set; }
        /// <summary>
        /// Player lives 
        /// </summary>
        public int Lives { get; private set; }

        /// <summary>
        /// End of the game state indicator
        /// </summary>
        public bool GameOver { get; private set; }

        /// <summary>
        /// Information about time when player died. 
        /// </summary>
        private DateTime? _playerDied = null;
        /// <summary>
        /// Player dying state indicator. 
        /// </summary>
        /// <remarks>
        /// Used to stop the game for 2.5 second when player is dying.
        /// </remarks>
        public bool PlayerDying { get { return _playerDied.HasValue; } }

        #region GameObjects
        private Player _player;
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();
        #endregion

        /// <summary>
        /// Invaders movement direction
        /// </summary>
        private Direction _invaderDirection = Direction.Left;
        /// <summary>
        /// Invaders moved down state indicator
        /// </summary>
        private bool _justMovedDown = false;

        /// <summary>
        /// Last update time
        /// </summary>
        private DateTime _lastUpdated = DateTime.MinValue;

        public InvadersModel()
        {
            EndGame();
        }

        /// <summary>
        /// Starts new game
        /// </summary>
        public void StartGame()
        {
            GameOver = false;
            _invaders.Clear();
            _invaderShots.Clear();
            _playerShots.Clear();
        }

        /// <summary>
        /// Ends game
        /// </summary>
        public void EndGame()
        {
            GameOver = true;
        }

    }
}
