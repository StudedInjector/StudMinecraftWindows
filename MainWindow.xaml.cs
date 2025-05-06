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
        private DispatcherTimer _t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
        private bool _a, _f = true;
        private string wl { get { return $"{Left} {Top}"; } }

        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("--f") && bool.TryParse(args[Array.IndexOf(args, "--f") + 1], out bool fade)) _f = fade;
            if (args.Contains("--wl") && int.TryParse(args[Array.IndexOf(args, "--wl") + 1], out int x) && int.TryParse(args[Array.IndexOf(args, "--wl") + 2], out int y)) { Left = x; Top = y; }

            InitializeComponent();
            LocationChanged += delegate { Console.WriteLine($"Moved: {wl}"); };
            _t.Tick += P;
            Opacity = _f ? 0 : 1;
            if (_f) FI();
        }

        public static void Notify(string t, string m, string i) =>
            new ToastContentBuilder().AddText(t).AddText(m).AddInlineImage(new Uri(i)).SetToastScenario(ToastScenario.Reminder).Show();

        private void InjectButton_Click(object s, RoutedEventArgs e)
        {
            if (_a) return;
            _a = true;
            InjectButton.IsEnabled = false;
            AN(MainContent, 0);
            AN(ProgressContent, 1);
            _t.Start();
        }

        private bool _inj;

        private void P(object s, EventArgs e)
        {
            if (ProgressBar.Value == 0 && !_inj) { _inj = true; Task.Run(delegate { Injector.Inject(); }); }

            if (ProgressBar.Value < 100)
            {
                ProgressBar.Value += 2.2;
                StatusText.Text = "Downloading Horion.dll" + new string('.', (int)(ProgressBar.Value / 33 % 3 + 1));
                return;
            }

            _t.Stop();
            var r = new DoubleAnimation(300, 0, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut } };

            r.Completed += delegate
            {
                ProgressBar.Value = 0;
                StatusText.Text = "Done";
                _a = false;
                InjectButton.IsEnabled = true;
            };

            ProgressBar.BeginAnimation(WidthProperty, r);
            AN(MainContent, 1);
            AN(ProgressContent, 0);
            _a = false;
            InjectButton.IsEnabled = true;
            Task.Delay(1).Wait();
            Injector.Inject();
            Notify("Horion", "Injected", "C:\\Users\\TUF\\source\\repos\\DotNet\\DotNet\\stud.png");
            Environment.Exit(0xf);
        }

        private void AN(FrameworkElement e, double o) =>
            e.BeginAnimation(OpacityProperty, new DoubleAnimation(o, TimeSpan.FromMilliseconds(1000)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut } });

        private void Window_MouseLeftButtonDown(object s, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void CloseButton_Click(object s, RoutedEventArgs e) => FO();

        private void Window_Closing(object s, System.ComponentModel.CancelEventArgs e)
        {
            if (Opacity > 0) { e.Cancel = true; FO(); }
        }

        private void FO()
        {
            var a = new DoubleAnimation(0, TimeSpan.FromSeconds(0.714455)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut } };
            a.Completed += delegate { Environment.Exit(0); };
            BeginAnimation(OpacityProperty, a);
        }

        private void FI() =>
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.714455)) { EasingFunction = new QuinticEase { EasingMode = EasingMode.EaseInOut } });
    }
}