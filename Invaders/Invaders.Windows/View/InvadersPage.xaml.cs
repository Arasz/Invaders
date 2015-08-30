using Invaders.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Invaders.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InvadersPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Determines if command about is added to options menu
        /// </summary>
        static bool aboutCommandAdded = false;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public InvadersPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            if(!aboutCommandAdded)
            {
                SettingsPane.GetForCurrentView().CommandsRequested += InvadersPage_CommandsRequested;
                aboutCommandAdded = true;
            }
        }

        /// <summary>
        /// Services event called in settings pane - CommandsRequested
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InvadersPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // Create new delegate type object
            UICommandInvokedHandler invokedHandler = AboutInvokedHandler;
            // Create new settings command
            SettingsCommand aboutCommand = new SettingsCommand("About", "About", invokedHandler);
            // Add new command
            args.Request.ApplicationCommands.Add(aboutCommand);
        }

        /// <summary>
        /// Called when about command clicked
        /// </summary>
        /// <param name="command"></param>
        private void AboutInvokedHandler(IUICommand command)
        {
            invadersViewModel.Paused = true;
            aboutPopup.IsOpen = true;
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            aboutPopup.IsOpen = false;
            invadersViewModel.Paused = false;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            aboutPopup.IsOpen = false;
            invadersViewModel.StartGame();
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="Common.SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="Common.NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int gameTitleHeight = 160;
            UpdatePlayAreaSize(new Size(e.NewSize.Width, e.NewSize.Height - gameTitleHeight));
        }

        private void pageRoot_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Translation.X < -1)
                invadersViewModel.LeftGestureStarted();
            else if (e.Delta.Translation.X > 1)
                invadersViewModel.RightGestureStarted();
        }

        private void pageRoot_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            invadersViewModel.GestureCompleted();
        }

        private void pageRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            invadersViewModel.Tapped();
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayAreaSize(playArea.RenderSize);
        }

        /// <summary>
        /// Key up event handler
        /// </summary>
        /// <param name="sender">Window in which key was released </param>
        /// <param name="args"></param>
        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            invadersViewModel.KeyDown(args.VirtualKey);
        }
        /// <summary>
        /// Key down event handler
        /// </summary>
        /// <param name="sender">Window in which key was released </param>
        /// <param name="args"></param>
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            invadersViewModel.KeyUp(args.VirtualKey);
        }

        /// <summary>
        /// Updates play are size to conserve 4:3 ratio
        /// </summary>
        /// <param name="newPlayAreaSize">New size</param>
        private void UpdatePlayAreaSize(Size newPlayAreaSize)
        {
            double targetWidth, targetHeight;

            if (newPlayAreaSize.Width > newPlayAreaSize.Height)
            {
                targetHeight = newPlayAreaSize.Height;
                targetWidth = newPlayAreaSize.Height * 4 / 3;

                double leftRightMargin = (newPlayAreaSize.Width - targetWidth) / 2;
                playArea.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
            }
            else
            {
                targetHeight = newPlayAreaSize.Width * 3 / 4;
                targetWidth = newPlayAreaSize.Width;
                double topBottomMargin = (newPlayAreaSize.Height - targetHeight) / 2;
                playArea.Margin = new Thickness(0, topBottomMargin, 0, topBottomMargin);
            }
            playArea.Width = targetWidth;
            playArea.Height = targetHeight;
            invadersViewModel.PlayAreaSize = playArea.RenderSize;
        }
    }
}
