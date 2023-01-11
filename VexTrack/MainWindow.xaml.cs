using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using VexTrack.MVVM.ViewModel;

namespace VexTrack
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.StateChanged += Window_StateChanged;
			Window_StateChanged(this, null);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
		}

		public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WmGetminmaxinfo)
			{
				// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
				// including the task bar.
				var mmi = (Minmaxinfo)Marshal.PtrToStructure(lParam, typeof(Minmaxinfo));

				// Adjust the maximized size and position to fit the work area of the correct monitor
				var monitor = MonitorFromWindow(hwnd, MonitorDefaulttonearest);

				if (monitor != IntPtr.Zero)
				{
					var monitorInfo = new Monitorinfo();
					monitorInfo.cbSize = Marshal.SizeOf(typeof(Monitorinfo));
					GetMonitorInfo(monitor, ref monitorInfo);
					var rcWorkArea = monitorInfo.rcWork;
					var rcMonitorArea = monitorInfo.rcMonitor;
					mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
					mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
					mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
					mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
				}

				Marshal.StructureToPtr(mmi, lParam, true);
			}

			return IntPtr.Zero;
		}

		private const int WmGetminmaxinfo = 0x0024;

		private const uint MonitorDefaulttonearest = 0x00000002;

		[DllImport("user32.dll")]
		private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

		[DllImport("user32.dll")]
		private static extern bool GetMonitorInfo(IntPtr hMonitor, ref Monitorinfo lpmi);

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public Rect(int left, int top, int right, int bottom)
			{
				this.Left = left;
				this.Top = top;
				this.Right = right;
				this.Bottom = bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Monitorinfo
		{
			public int cbSize;
			public Rect rcMonitor;
			public Rect rcWork;
			public uint dwFlags;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public int X;
			public int Y;

			public Point(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Minmaxinfo
		{
			public Point ptReserved;
			public Point ptMaxSize;
			public Point ptMaxPosition;
			public Point ptMinTrackSize;
			public Point ptMaxTrackSize;
		}

		private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
			}
			else
			{
				this.WindowState = WindowState.Maximized;
			}
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			var mainVm = (MainViewModel)ViewModelManager.ViewModels["Main"];
			mainVm.Destroy();

			this.Close();
		}

		private void RefreshMaximizeRestoreButton()
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.maximizeButton.Visibility = Visibility.Collapsed;
				this.restoreButton.Visibility = Visibility.Visible;

				this.MainBorder.CornerRadius = new CornerRadius(0);
				this.ShadowBorder.CornerRadius = new CornerRadius(0);
				this.PopupBorder.CornerRadius = new CornerRadius(0);
				this.MainBorder.Margin = new Thickness(0);
				this.ShadowBorder.Margin = new Thickness(0);
				this.PopupBorder.Margin = new Thickness(0);
			}
			else if (this.WindowState == WindowState.Normal)
			{
				this.maximizeButton.Visibility = Visibility.Visible;
				this.restoreButton.Visibility = Visibility.Collapsed;

				this.MainBorder.CornerRadius = new CornerRadius(8);
				this.ShadowBorder.CornerRadius = new CornerRadius(8);
				this.PopupBorder.CornerRadius = new CornerRadius(8);
				this.MainBorder.Margin = new Thickness(16);
				this.ShadowBorder.Margin = new Thickness(16);
				this.PopupBorder.Margin = new Thickness(16);
			}
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			this.RefreshMaximizeRestoreButton();
		}

		private void PopupBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var vm = (MainViewModel)DataContext;
			vm.OnPopupBorderClick();
		}
	}
}
