using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Invaders.View
{
    /// <summary>
    /// Representation of animated image
    /// </summary>
    public sealed partial class AnimatedImage : UserControl
    {
        private Storyboard _invaderShotStoryboard;
        private Storyboard _flashStoryboard;

        public AnimatedImage()
        {
            this.InitializeComponent();
        }

        public AnimatedImage(IEnumerable<string> imageNames, TimeSpan interval)
        {
            CreateInvaderShotStoryboard();
            CreateFlashingAnimation();
            StartAnimation(imageNames, interval);
        }

        /// <summary>
        /// Starts new animation from given images. 
        /// </summary>
        /// <param name="imageNames">Names of images in assets folder</param>
        /// <param name="interval">Interval between animation frames</param>
        public void StartAnimation(IEnumerable<string> imageNames, TimeSpan interval)
        {
            Storyboard storyboard = new Storyboard();
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();

            // We're going to animate image inside our control.
            Storyboard.SetTarget(animation, image);
            // Animation relies on changing value of property source
            Storyboard.SetTargetProperty(animation, nameof(image.Source));

            TimeSpan currentInterval = TimeSpan.FromMilliseconds(0);
            foreach (string imageName in imageNames)
            {
                // We're creating individual frames from given images
                ObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
                keyFrame.Value = CreateImageFromAssets(imageName);
                keyFrame.KeyTime = currentInterval;
                animation.KeyFrames.Add(keyFrame);
                currentInterval = currentInterval.Add(interval);
            }

            // We're configuring our storyboard which will play animations
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.Children.Add(animation);
            storyboard.Begin();

        }

        private static BitmapImage CreateImageFromAssets(string imageFileName)
        {
            return new BitmapImage(new Uri(string.Format("ms-appx://Assets/{0}", imageFileName)));
        }

        /// <summary>
        /// Creates storyboard which contains shot invaders animation 
        /// </summary>
        private void CreateInvaderShotStoryboard()
        {
            _invaderShotStoryboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();

            Storyboard.SetTarget(animation, image);
            Storyboard.SetTargetProperty(animation, nameof(image.Opacity));

            animation.From = 1;
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(2);

            _invaderShotStoryboard.Children.Add(animation);
        }

        private void CreateFlashingAnimation()
        {
            _flashStoryboard = new Storyboard();
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, image);
            Storyboard.SetTargetProperty(animation, nameof(image.Visibility));

            ObjectKeyFrame visibleFrame = new DiscreteObjectKeyFrame()
            {
                Value = Windows.UI.Xaml.Visibility.Visible,
                KeyTime = TimeSpan.FromMilliseconds(0),
            };
            ObjectKeyFrame invisibleFrame = new DiscreteObjectKeyFrame()
            {
                Value = Windows.UI.Xaml.Visibility.Collapsed,
                KeyTime = TimeSpan.FromMilliseconds(500),
            };

            animation.KeyFrames.Add(visibleFrame);
            animation.KeyFrames.Add(invisibleFrame);

            _flashStoryboard.Children.Add(animation);
            _flashStoryboard.AutoReverse = true;
            _flashStoryboard.RepeatBehavior = RepeatBehavior.Forever;
        }

        public void InvaderShot()
        {
            _invaderShotStoryboard.Begin();
        }

        public void StartFlashing()
        {
            _flashStoryboard.Begin();
        }

        public void StopFlashing()
        {
            _flashStoryboard.Stop();
        }
    }
}
