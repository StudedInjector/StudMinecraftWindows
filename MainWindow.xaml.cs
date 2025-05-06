using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DotNet
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _progressTimer;
        private bool _isAnimating;
        private bool _fadeEnabled = true;
        private string windowLocation => $"{Left} {Top}";

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

        private void InjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimating) return;
            _isAnimating = true;
            InjectButton.IsEnabled = false;
            AnimateElement(MainContent, 0);
            AnimateElement(ProgressContent, 1);
            _progressTimer.Start();
        }
        private bool _hasInjected = false; // Track if injection has started

        private void UpdateProgress(object sender, EventArgs e)
        {
            if (ProgressBar.Value == 0 && !_hasInjected)
            {
                // Start injecting only once when progress begins
                _hasInjected = true;
                Task.Run(() => Injector.Inject());
            }


            if (ProgressBar.Value < 100)
            {
                ProgressBar.Value += 2.2;
                StatusText.Text = "downloading dll" + new string('.', (int)(ProgressBar.Value / 33 % 3 + 1));
                return;
            }

            _progressTimer.Stop();
            var reverseAnim = new DoubleAnimation
            {
                From = 300,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut }
            };

            reverseAnim.Completed += (s, _) =>
            {
                ProgressBar.Value = 0;
                StatusText.Text = "Injected!";
                _isAnimating = false;
                InjectButton.IsEnabled = true;
            };

            ProgressBar.BeginAnimation(WidthProperty, reverseAnim);
            AnimateElement(MainContent, 1);
            AnimateElement(ProgressContent, 0);
            _isAnimating = false;
            InjectButton.IsEnabled = true;
            Task.Delay(1).Wait();
            // animation, notification not stolen...
            Injector.Inject();
            ShowNotification("Horion x Stud", "Injected", "C:\\Users\\TUF\\source\\repos\\DotNet\\DotNet\\stud.png");
            Environment.ExitCode = 0xf;
            Environment.Exit(0xf);
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