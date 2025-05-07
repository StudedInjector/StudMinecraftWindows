using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using StudInjector;
using cr1tcal3.lib;
using Windows.Web.Http;
namespace DotNet
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _progressTimer; private bool _isAnimating; private bool _fadeEnabled = true; private string windowLocation => $"{Left} {Top}";
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args.Contains("--f"))
            {
                int index = Array.IndexOf(args, "--f");
                if (index + 1 < args.Length && bool.TryParse(args[index + 1], out bool fadeArg))
                    _fadeEnabled = fadeArg;
            }

            if (args.Contains("--wl"))
            {
                int index = Array.IndexOf(args, "--wl");
                if (index + 2 < args.Length && int.TryParse(args[index + 1], out int x) && int.TryParse(args[index + 2], out int y))
                {
                    Left = x;
                    Top = y;
                }
            }

            InitializeComponent();
            cr1tcal3.lib.main.dc();
            LocationChanged += UpdateWindowLocation;

            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
            _progressTimer.Tick += UpdateProgress;

            if (!_fadeEnabled) Opacity = 1;
            else BeginFadeIn();
        }

        private void UpdateWindowLocation(object sender, EventArgs e)
        {
            Console.WriteLine($"Window moved: {windowLocation}");
        }

        public static void ShowNotification(string title, string message, string imagePath)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .AddInlineImage(new Uri(imagePath)) // Adds image
                .SetToastScenario(ToastScenario.Reminder) // Ensures normal behavior
                .Show();
        }

        private static string grstr()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF); // Unique seed
            return new string(Enumerable.Range(0, 2000).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
        }

        private async void InjectButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            if (_isAnimating) return;
            _isAnimating = true;
            InjectButton.IsEnabled = false;
            AnimateElement(MainContent, 0);
            AnimateElement(ProgressContent, 1);
            _progressTimer.Start();
            
        }

        private async void UpdateProgress(object sender, EventArgs e)
        {
            if (ProgressBar.Value < 100)
            {
                this.Title = grstr(); // GetWindowTitle method blocker
                this.ToolTip = grstr();
                ProgressBar.Value += 2.2;
                StatusText.Text = "Downloading DLL" + new string('.', (int)(ProgressBar.Value / 33 % 3 + 1));
                return;
            }

            _progressTimer.Stop();

            // Finally::
            var reverseAnim = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut }
            };

            ProgressBar.BeginAnimation(WidthProperty, reverseAnim);
            StatusText.Text = "Injecting...";

            try
            {
                bool success = await Injector.Inject();
                AnimateElement(MainContent, 1);
                AnimateElement(ProgressContent, 0);
                StatusText.Text = success ? "Success!" : "Failed";
                if (success) Environment.Exit(0);
            }
            finally
            {
                _isAnimating = false;
                InjectButton.IsEnabled = true;
            }
        }

        private void AnimateElement(FrameworkElement element, double targetOpacity)
        {
            var opacityAnim = new DoubleAnimation(targetOpacity, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut }
            };
            element.BeginAnimation(OpacityProperty, opacityAnim);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            BeginFadeOut();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Opacity > 0)
            {
                e.Cancel = true;
                BeginFadeOut();
            }
        }

        private void BeginFadeOut()
        {
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.714455))
            {
                EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut }
            };
            anim.Completed += (s, _) => Environment.Exit(0);
            BeginAnimation(OpacityProperty, anim);
        }

        private void BeginFadeIn()
        {
            var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.714455))
            {
                EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut }
            };
            BeginAnimation(OpacityProperty, fadeAnim);
        }
    }
}