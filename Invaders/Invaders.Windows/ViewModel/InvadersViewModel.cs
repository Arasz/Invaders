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
    class InvadersViewModel
    {
        /// <summary>
        /// Stores elements displayed on play area.
        /// </summary>
        private readonly ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
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
        /// Connects invader data model with it representation
        /// </summary>
        private readonly Dictionary<Invader, FrameworkElement> _invaders =
            new Dictionary<Invader, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders =
            new Dictionary<FrameworkElement, DateTime>();

        public InvadersViewModel()
        {
        
        }

        internal void KeyDown(VirtualKey virtualKey)
        {
            throw new NotImplementedException();
        }

        internal void KeyUp(VirtualKey virtualKey)
        {
            throw new NotImplementedException();
        }

        internal void LeftGestureStarted()
        {
            throw new NotImplementedException();
        }

        internal void RightGestureStarted()
        {
            throw new NotImplementedException();
        }

        internal void GestureCompleted()
        {
            throw new NotImplementedException();
        }

        internal void Tapped()
        {
            throw new NotImplementedException();
        }

        internal void StartGame()
        {
            throw new NotImplementedException();
        }

        private void RecreateScanLines()
        {
            throw new NotImplementedException();
        }
    }
}
