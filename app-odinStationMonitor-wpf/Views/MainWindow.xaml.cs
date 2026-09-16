using TacoStationMonitor.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace TacoStationMonitor.Views
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(
            IntPtr hdc,
            IntPtr lprcClip,
            MonitorEnumProc lpfnEnum,
            IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(
            IntPtr hMonitor,
            ref MONITORINFO lpmi);

        private delegate bool MonitorEnumProc(
            IntPtr hMonitor,
            IntPtr hdcMonitor,
            ref RECT lprcMonitor,
            IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        private readonly IConfiguration _configuration;

        public MainWindow(DashboardViewModel viewModel, IConfiguration configuration)
        {
            InitializeComponent();

            DataContext = viewModel;
            _configuration = configuration;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MoveToSecondMonitor();
        }

        private void MoveToSecondMonitor()
        {
            int targetMonitor = 0;

            string? monitorValue =
                _configuration["DashboardSettings:TargetMonitor"];

            if (int.TryParse(monitorValue, out int configuredMonitor))
            {
                targetMonitor = configuredMonitor;
            }

            IntPtr targetMonitorHandle = IntPtr.Zero;
            int monitorCount = 0;

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr data) =>
                {
                    monitorCount++;

                    if (monitorCount == targetMonitor)
                    {
                        targetMonitorHandle = hMonitor;
                    }

                    return true;
                },
                IntPtr.Zero);

            if (targetMonitorHandle == IntPtr.Zero)
            {
                WindowState = WindowState.Maximized;
                return;
            }

            var monitorInfo = new MONITORINFO
            {
                cbSize = Marshal.SizeOf<MONITORINFO>()
            };

            if (!GetMonitorInfo(targetMonitorHandle, ref monitorInfo))
                return;

            var monitorRect = monitorInfo.rcMonitor;

            WindowState = WindowState.Normal;

            Left = monitorRect.Left;
            Top = monitorRect.Top;

            Width = monitorRect.Right - monitorRect.Left;
            Height = monitorRect.Bottom - monitorRect.Top;

            WindowState = WindowState.Maximized;
        }
    }
}

//using System.Windows;
//using System.Windows.Interop;
//using System.Runtime.InteropServices;
//using DashboardSolution.ViewModels;
//using System.Text;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
////using forms = System.Windows.Forms;
//using System;

//namespace DashboardSolution.Views
//{
//    public partial class MainWindow : Window
//    {
//        private DashboardViewModel vm = new DashboardViewModel();       

//        [DllImport("user32.dll")]
//        private static extern IntPtr MonitorFromWindow(
//            IntPtr hwnd,
//            uint dwFlags);

//        [DllImport("user32.dll")]
//        private static extern bool GetMonitorInfo(
//            IntPtr hMonitor,
//            ref MONITORINFO lpmi);

//        private const uint MONITOR_DEFAULTTONEAREST = 2;

//        [StructLayout(LayoutKind.Sequential)]
//        private struct RECT
//        {
//            public int Left;
//            public int Top;
//            public int Right;
//            public int Bottom;
//        }

//        [StructLayout(LayoutKind.Sequential)]
//        private struct MONITORINFO
//        {
//            public int cbSize;
//            public RECT rcMonitor;
//            public RECT rcWork;
//            public uint dwFlags;
//        }

//        public MainWindow()
//        {
//            InitializeComponent();

//            DataContext = vm;

//            Loaded += MainWindow_Loaded;
//        }

//        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
//        {
//            MoveToSecondMonitor();
//        }

//        private void MoveToSecondMonitor()
//        {
//            var presentationSource = PresentationSource.FromVisual(this);

//            if (presentationSource == null)
//                return;

//            var hwnd = new WindowInteropHelper(this).Handle;

//            // Get the monitor currently containing the window
//            var currentMonitor = MonitorFromWindow(
//                hwnd,
//                MONITOR_DEFAULTTONEAREST);

//            var monitorInfo = new MONITORINFO
//            {
//                cbSize = Marshal.SizeOf<MONITORINFO>()
//            };

//            if (!GetMonitorInfo(currentMonitor, ref monitorInfo))
//                return;

//            // Get all monitors through WPF
//            var allScreens = System.Windows.SystemParameters
//                .VirtualScreenWidth;

//            // Move to the second monitor using Win32 monitor enumeration
//            MoveWindowToSecondMonitor(hwnd);
//        }

//        private void MoveWindowToSecondMonitor(IntPtr hwnd)
//        {
//            var monitors = new System.Collections.Generic.List<IntPtr>();

//            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
//                {
//                    monitors.Add(hMonitor);
//                    return true;
//                },
//                IntPtr.Zero);

//            // Only one monitor
//            if (monitors.Count < 2)
//            {
//                WindowState = WindowState.Maximized;
//                return;
//            }

//            // Second monitor
//            var secondMonitor = monitors[1];

//            var monitorInfo = new MONITORINFO
//            {
//                cbSize = Marshal.SizeOf<MONITORINFO>()
//            };

//            if (!GetMonitorInfo(secondMonitor, ref monitorInfo))
//                return;

//            var rect = monitorInfo.rcMonitor;

//            var source = PresentationSource.FromVisual(this);

//            double scaleX = 1.0;
//            double scaleY = 1.0;

//            if (source?.CompositionTarget != null)
//            {
//                scaleX = source.CompositionTarget.TransformFromDevice.M11;
//                scaleY = source.CompositionTarget.TransformFromDevice.M22;
//            }

//            WindowState = WindowState.Normal;

//            Left = rect.Left * scaleX;
//            Top = rect.Top * scaleY;
//            Width = (rect.Right - rect.Left) * scaleX;
//            Height = (rect.Bottom - rect.Top) * scaleY;

//            WindowState = WindowState.Maximized;
//        }

//        [DllImport("user32.dll")]
//        private static extern bool EnumDisplayMonitors(
//            IntPtr hdc,
//            IntPtr lprcClip,
//            MonitorEnumProc lpfnEnum,
//            IntPtr dwData);

//        private delegate bool MonitorEnumProc(
//            IntPtr hMonitor,
//            IntPtr hdcMonitor,
//            ref RECT lprcMonitor,
//            IntPtr dwData);

//        //public MainWindow()
//        //{
//        //    InitializeComponent();

//        //    DataContext = vm;
//        //    Loaded += MainWindow_Loaded;
//        //}

//        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
//        //{
//        //    MoveToSecondScreen();
//        //}

//        //private void MoveToSecondScreen()
//        //{
//        //    var screens = forms.Screen.AllScreens;

//        //    // If only one monitor is connected, keep the application on the primary screen
//        //    if (screens.Length < 2)
//        //    {
//        //        WindowStartupLocation = WindowStartupLocation.CenterScreen;
//        //        WindowState = WindowState.Maximized;
//        //        return;
//        //    }

//        //    // Get the second monitor
//        //    var secondScreen = screens.FirstOrDefault(screen => !screen.Primary);

//        //    if (secondScreen == null)
//        //        return;

//        //    // Convert screen coordinates from pixels to WPF device-independent units
//        //    var source = PresentationSource.FromVisual(this);

//        //    double dpiX = 1.0;
//        //    double dpiY = 1.0;

//        //    if (source?.CompositionTarget != null)
//        //    {
//        //        dpiX = source.CompositionTarget.TransformFromDevice.M11;
//        //        dpiY = source.CompositionTarget.TransformFromDevice.M22;
//        //    }

//        //    var workingArea = secondScreen.WorkingArea;

//        //    Left = workingArea.Left * dpiX;
//        //    Top = workingArea.Top * dpiY;

//        //    Width = workingArea.Width * dpiX;
//        //    Height = workingArea.Height * dpiY;

//        //    WindowState = WindowState.Maximized;
//        //}
//    }
//}
