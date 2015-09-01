using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIElement = Windows.UI.Xaml.UIElement;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.System;
using System.ComponentModel;
using DispatcherTimer = Windows.UI.Xaml.DispatcherTimer;
using FrameworkElement = Windows.UI.Xaml.FrameworkElement;
using Invaders.View;
using Invaders.Model;

namespace Invaders.ViewModel
{
    /// <summary>
    /// Game view model
    /// </summary>
    class InvadersViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var propertyChangedEventHandler = PropertyChanged;

            // Elvis operator ( if( a != null) a.Invoke() )
            propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        /// <summary>
        /// Stores elements displayed on play area.
        /// </summary>
        private readonly ObservableCollection<FrameworkElement> _sprites = 
            new ObservableCollection<FrameworkElement>();
        /// <summary>
        /// Provides possibility to subscribe collection of sprites event CollectionChanged
        /// </summary>
        public INotifyCollectionChanged Sprites { get { return _sprites; } }

        /// <summary>
        /// Indicates that game is over
        /// </summary>
        public bool GameOver { get { return _model.GameOver; } }

        /// <summary>
        /// Representation of player lives
        /// </summary>
        private readonly ObservableCollection<object> _lives = new ObservableCollection<object>();

        /// <summary>
        /// Provides possibility to subscribe collection of lives event CollectionChanged
        /// </summary>
        public INotifyCollectionChanged Lives { get { return _lives; } }

        /// <summary>
        /// Indicates that game is paused
        /// </summary>
        public bool Paused { get; set; }
        private bool _lastPaused = true;

        /// <summary>
        /// Factor which allows to calculate size or location of controls from 400x300 
        /// coordinates to Canvas control coordinates
        /// </summary>
        public static double Scale { get; private set; }

        /// <summary>
        /// Game score
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// Game play area size
        /// </summary>
        public Size PlayAreaSize
        {
            set
            {
                Scale = value.Width / 405;
                _model.UpdateAllShipsAndStars();
                RecreateScanLines();
            }
        }

        private readonly InvadersModel _model = new InvadersModel();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private FrameworkElement _playerControl = null;
        private bool _playerFlashing = false;

        /// <summary>
        /// Connects invader data model with it's UI representation
        /// </summary>
        private readonly Dictionary<Invader, FrameworkElement> _invaders =
            new Dictionary<Invader, FrameworkElement>();

        /// <summary>
        /// Connects time of begin shot and shot invader
        /// </summary>
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders =
            new Dictionary<FrameworkElement, DateTime>();

        private readonly Dictionary<Shot, FrameworkElement> _shots =
            new Dictionary<Shot, FrameworkElement>();

        private readonly Dictionary<Point, FrameworkElement> _stars =
            new Dictionary<Point, FrameworkElement>();

        private readonly List<FrameworkElement> _scanLines =
            new List<FrameworkElement>();

        public InvadersViewModel()
        {
            Scale = 1;

            _model.ShipChanged += _model_ShipChanged;
            _model.ShotMoved += _model_ShotMoved;
            _model.StarChanged += _model_StarChanged;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += _timer_Tick;

            EndGame(); 
        }

        /// <summary>
        /// Starts game
        /// </summary>
        /// <remarks>
        /// Deletes all invaders ships and shots from sprite collection, starts timer and
        /// informs model about game start 
        /// </remarks>
        private void StartGame()
        {
            Paused = false;
            foreach (var invader in _invaders.Values)
                _sprites.Remove(invader);
            foreach (var shot in _shots.Values)
                _sprites.Remove(shot);
            _model.StartGame();
            OnPropertyChanged(nameof(GameOver));
            _timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndGame()
        {
            throw new NotImplementedException();
        }

        #region Model class events handlers

        private void _timer_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }

        private void _model_StarChanged(object sender, StarChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _model_ShotMoved(object sender, ShotMovedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _model_ShipChanged(object sender, ShipChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region User interaction services

        /// <summary>
        /// Contains data about time when left button was pressed or left swipe was registered.
        /// </summary>
        private DateTime? _leftAction = null;
        /// <summary>
        /// Contains data about time when right button was pressed or right swipe was registered.
        /// </summary>
        private DateTime? _rightAction = null;

        internal void KeyDown(VirtualKey virtualKey)
        {
            switch (virtualKey)
            {
                case VirtualKey.Space:
                    _model.FireShot();
                    break;
                case VirtualKey.Left:
                    _leftAction = DateTime.Now;
                    break;
                case VirtualKey.Right:
                    _rightAction = DateTime.Now;
                    break;
                default:
                    break;
            }
        }

        internal void KeyUp(VirtualKey virtualKey)
        {
            switch (virtualKey)
            {
                case VirtualKey.Left:
                    _leftAction = null;
                    break;
                case VirtualKey.Right:
                    _rightAction = null;
                    break;
                default:
                    break;
            }
        }

        internal void LeftGestureStarted()
        {
            _leftAction = DateTime.Now;
        }

        internal void RightGestureStarted()
        {
            _rightAction = DateTime.Now;
        }

        internal void GestureCompleted()
        {
            /// In original code there was a specific method for each direction
            /// of gesture. But this methods were called at the same time, on 
            /// manipulation completed event. I decided to make only one method 
            /// for servicing manipulation completed method.
            _leftAction = null;
            _rightAction = null;
        }

        internal void Tapped()
        {
            // TODO: Modify this method to avoid shot on the beginning of the game
            // ( when player clicks start button )
            _model.FireShot();
        }


        #endregion

        /// <summary>
        /// Creates simulated horizontal noise lines from old TV 
        /// </summary>
        private void RecreateScanLines()
        {
            // Remove scan lines from game screen
            foreach (FrameworkElement scanLIne in _scanLines)
            {
                if (_sprites.Contains(scanLIne))
                    _sprites.Remove(scanLIne);
            }
            _scanLines.Clear();

            for (int y = 0; y < 300; y+=2)
            {
                FrameworkElement scanLine = InvadersHelper.ScanLineFactory(y, 400, Scale);
            }
        }
    }
}
