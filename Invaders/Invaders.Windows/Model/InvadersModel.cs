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
        /// Number of invaders in one wave
        /// </summary>
        private const int _invadersAmount = 66;

        /// <summary>
        /// Time between invaders movement.
        /// </summary>
        private readonly TimeSpan _invadersMoveInterval = TimeSpan.FromMilliseconds(500);

        internal void UpdateAllShipsAndStars()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Player location at the beginning of new game (in the center of play area bottom)
        /// </summary>
        private readonly Point _playerInitialLocation = new Point(150, Player.PlayerSize.Height / 2); //HACK: Redundant variable?

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
            // Clear game objects
            GameOver = false;
            foreach (Invader invader in _invaders)
                OnShipChanged(invader, true); //TODO: Check if killed parameter really should be true
            _invaders.Clear();

            foreach (Shot invadersShot in _invaderShots)
                OnShotMoved(invadersShot, true);
            _invaderShots.Clear();

            foreach (Shot playerShot in _playerShots)
                OnShotMoved(playerShot, true);
            _playerShots.Clear();

            foreach (var star in _stars)
                OnStarChanged(star, true);
            _stars.Clear();
            
            // Create new game objects
            AddNewStars(InitialStarCount);
            _player = new Player(_playerInitialLocation, Player.PlayerSize);
            Lives = 2;
            Wave = 0;
            NextWave();
        }
        /// <summary>
        /// Ends game
        /// </summary>
        public void EndGame()
        {
            GameOver = true;
        }

        /// <summary>
        /// Fire shot in the invaders
        /// </summary>
        public void FireShot()
        {
            //Check if there is place for next shot
            if (MaximumPlayerShots <= _playerShots.Count)
                return;
            else
            {
                // Add new shot
                Point shotLocation = new Point(_player.Location.X, _player.Location.Y + Player.PlayerSize.Height/2); 
                Shot shot = new Shot(shotLocation, Direction.Up);
                _playerShots.Add(shot);
                OnShotMoved(shot, false);
            }
        }

        /// <summary>
        /// Moves player ship
        /// </summary>
        public void MovePlayer(Direction direction)
        {
            if (PlayerDying)
                return;
            else
            {
                _player.Move(direction);
                OnShipChanged(_player, false);
            }
        }

        /// <summary>
        /// Randomly adds or removes one star
        /// </summary>
        public void Twinkle()
        {
            var starsCounter = _stars.Count();
            // Amount of stars must be in (15% of initial value, 150% of initial value) interval
            if ((starsCounter+1<(1.5*InitialStarCount))&&(starsCounter-1>(0.15*InitialStarCount)))
            {
                if (_random.Next(0, 2) == 0)
                    _stars.RemoveAt(_random.Next(0, starsCounter));
                else
                    _stars.Add(RandomPointFactory());
            }
        }

        /// <summary>
        /// Core game method. Updates state of the game.
        /// </summary>
        public void Update()
        {
            _lastUpdated = DateTime.Now;

            Twinkle(); // Stars always twinkle 
            if(!GameOver)
            {
                // Check if there is any invader ship. If not, create new wave
                if(!_invaders.Any())
                    NextWave();

                // Check if player is alive
                if(!PlayerDying)
                {
                    MoveInvaders();

                    // Move all shots
                    Rect playAreaRect = new Rect(new Point(0, 0), PlayAreaSize);

                    foreach (var shot in _playerShots)
                    {
                        shot.Move();
                        if (!playAreaRect.Contains(shot.Location))
                            _playerShots.Remove(shot);
                        OnShotMoved(shot, false);
                    }
                    foreach (var shot in _invaderShots)
                    {
                        shot.Move();
                        if (!playAreaRect.Contains(shot.Location))
                            _invaderShots.Remove(shot);
                        OnShotMoved(shot, false);
                    }

                    ReturnFire();

                    // Collisions check 
                    // At first check for shots which struck into invader ship

                    //We are using ToList() to make collection copy. We're iterating through copy
                    // and we're using remove on the original collection. This prevents the occurrence
                    // of an exception.
                    foreach (Shot playerShot in _playerShots.ToList()) 
                    {
                        foreach (Invader invader in _invaders.ToList()) 
                        {
                            if (CheckForInvaderCollision(playerShot, invader))
                            {
                                _invaders.Remove(invader);
                                OnShipChanged(invader, true);
                                _playerShots.Remove(playerShot);
                                OnShotMoved(playerShot, true);
                                break;
                            }
                        }
                    }
                    // Check if any enemy shot struck player ship
                    foreach (Shot enemyShot in _invaderShots.ToList())
                    {
                        if (CheckForPlayerCollision(enemyShot))
                        {
                            _playerDied = DateTime.Now;
                            OnShipChanged(_player, true);
                            _invaderShots.Remove(enemyShot);
                            OnShotMoved(enemyShot, true);
                            break;
                        }
                    }
                }
            }
        }


        #region Private helper methods
        // TODO: ADD FEW METHODS TO HELPER CLASS

        /// <summary>
        /// Moves invaders.
        /// </summary>
        /// <remarks>
        /// Ships are moving from edge to edge of a play area. 
        /// If they are at the edge, they're moving down. 
        /// This method should use Move() of each ship. There's also _lastUpdated field
        /// which can be used to accelerate movement of invaders when their numbers in formation
        /// will decrease.
        /// </remarks>
        private void MoveInvaders()
        {
            //TODO: Improve this method
            //ISSUE: Potential issue 

            // Check if elapsed enough time between invaders move 

            TimeSpan intervalBetweenMoves = DateTime.Now - _lastUpdated;
            if (intervalBetweenMoves <= _invadersMoveInterval)
                return;


            // Select invaders that are near the play area edge
            int epsilon = 20; // Distance equal to doubled one time movement of invader
            var invadersOnEdge =
                from invader in _invaders
                where (invader.Location.X >= PlayAreaSize.Width - epsilon) || (invader.Location.X <= epsilon)
                //where !(new Rect(new Point(0, 0), PlayAreaSize).Contains(invader.Location))
                select invader;
            
            if(invadersOnEdge!=null && invadersOnEdge.Any()&&(!_justMovedDown)) 
            {
                // If there is any invader out of the play area move down
                _justMovedDown = true;
                foreach (Invader invader in _invaders)
                {
                    invader.Move(Direction.Down);
                    OnShipChanged(invader, false);
                }
            }
            else
            {
                _justMovedDown = false;
                foreach (Invader invader in _invaders)
                {
                    invader.Move(_invaderDirection);
                    OnShipChanged(invader, false);
                }
            }

            // Change invader direction
            if (_justMovedDown && _invaderDirection == Direction.Left)
                _invaderDirection = Direction.Right;
            else if (_justMovedDown && _invaderDirection == Direction.Right)
                _invaderDirection = Direction.Left;

            // Check if there is any invader near the bottom play area edge
            var invadersAtBottom =
                from invader in _invaders
                where invader.Location.Y >= PlayAreaSize.Height
                select invader;
            if(invadersAtBottom != null && invadersAtBottom.Any())
            {
                EndGame();
            }


        }

        /// <summary>
        /// Invaders are shooting at player. There can be max Wave+1 shots in play area;
        /// </summary>
        private void ReturnFire()
        {
            if (_invaders.Count <= 0)
                return;

            int possibleShots = Wave + 1;
            int shotsInvadersCanShot = _invaderShots.Count - possibleShots;

            if (shotsInvadersCanShot > 0 && !(_random.Next(10)< 10 - Wave))
            {
                Point shotLocation;
                Shot shot;

                // Select invaders from last row  ( only this invaders can shot)

                var groupedByYPosition =
                    from invader in _invaders
                    group invader by invader.Location.Y
                        into invadersGroups
                        orderby invadersGroups.Key
                        select invadersGroups;
                var bottomInvaders = groupedByYPosition.Last();

                for (int i = 0; i < possibleShots; i++)
                {
                    Invader invader = bottomInvaders.ElementAt(_random.Next(0, bottomInvaders.Count()));
                    shotLocation = new Point(invader.Location.X, invader.Location.Y + invader.Size.Height / 2);
                    shot = new Shot(shotLocation, Direction.Down);
                    _invaderShots.Add(shot);
                    OnShotMoved(_invaderShots.Last(), false);
                } 
            }
        }

        /// <summary>
        /// Checks if shot collides with player
        /// </summary>
        /// <param name="enemyShot"></param>
        /// <returns>True if there is collision</returns>
        private bool CheckForPlayerCollision(Shot enemyShot)
        {
            return CollisionHelper.CheckCollision(_player.Area, enemyShot.Location);
        }

        /// <summary>
        /// Checks if shot collides with given invader
        /// </summary>
        /// <param name="playerShot"></param>
        /// <param name="invader"></param>
        /// <returns>True if there is collision</returns>
        private bool CheckForInvaderCollision(Shot playerShot, Invader invader)
        {
            return CollisionHelper.CheckCollision(invader.Area, playerShot.Location);
        }

        /// <summary>
        /// Fills up invaders collection and calls ShipChanged event for each invader
        /// </summary>
        private void NextWave()
        {
            // Distance between invaders should be equal to 1.4 invader width/height.
            Wave++;
            int invadersInRowCount = 11;
            double dx = Invader.InvaderSize.Width / 2;
            double dy = Invader.InvaderSize.Height;

            // Initial coordinates of firs invader;
            double x =(PlayAreaSize.Width / 2) - (6 * dx); 
            double y = dy;

            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Spaceship, 50, new Point((x + i * dx), y), Invader.InvaderSize));
            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Satellite, 40, new Point((x + i * dx), y+dy), Invader.InvaderSize));
            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Saucer, 30, new Point((x + i * dx), y + 2* dy), Invader.InvaderSize));
            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Bug, 20, new Point((x + i * dx), y + 4* dy), Invader.InvaderSize));
            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Bug, 20, new Point((x + i * dx), y + 6* dy), Invader.InvaderSize));
            for (int i = 0; i < invadersInRowCount; i++)
                _invaders.Add(new Invader(InvaderType.Star, 10, new Point((x + i * dx), y + 8* dy), Invader.InvaderSize));

            foreach (Invader invader in _invaders)
                OnShipChanged(invader, false);
        }

        /// <summary>
        /// Adds new stars to the stars collection
        /// </summary>
        /// <param name="starsAmount">Stars number</param>
        private void AddNewStars(int starsAmount)
        {
            for (int i = 0; i < starsAmount; i++)
            {
                _stars.Add(RandomPointFactory());
                OnStarChanged(_stars.Last(), false);
            }
        }

        /// <summary>
        /// Creates random point inside game play area
        /// </summary>
        /// <returns>Point inside game area</returns>
        private Point RandomPointFactory() //TODO: Move to the helper class
        {
            int epsilon = 0; // Defines minimum distance from play area "frame"
            double x = _random.Next(epsilon, (int)PlayAreaSize.Width - epsilon);
            double y = _random.Next(epsilon, (int)PlayAreaSize.Height - epsilon);
            return new Point(x, y);
        }
        #endregion

    }
}
