using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using VexTrack.MVVM.ViewModel;

namespace VexTrack;

public partial class MainWindow
{
	public MainWindow()
	{
		InitializeComponent();

		StateChanged += Window_StateChanged;
		Window_StateChanged(this, null);
	}

	protected override void OnSourceInitialized(EventArgs e)
	{
		base.OnSourceInitialized(e);
		((HwndSource)PresentationSource.FromVisual(this))!.AddHook(HookProc);
	}

	// ReSharper disable once IdentifierTypo
	private static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == WmGetMinMaxInfo)
		{
			// We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
			// including the task bar.
			var mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo))!;

			// Adjust the maximized size and position to fit the work area of the correct monitor
			var monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);

			if (monitor != IntPtr.Zero)
			{
				var monitorInfo = new MonitorInfo
				{
					cbSize = Marshal.SizeOf(typeof(MonitorInfo))
				};
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

	private const int WmGetMinMaxInfo = 0x0024;

	private const uint MonitorDefaultToNearest = 0x00000002;

	[DllImport("user32.dll")]
	private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

	[DllImport("user32.dll")]
	// ReSharper disable once IdentifierTypo
	private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

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
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MonitorInfo
	{
		public int cbSize;
		public Rect rcMonitor;
		public Rect rcWork;
		private readonly uint dwFlags;
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Point
	{
		public int X;
		public int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MinMaxInfo
	{
		private readonly Point ptReserved;
		public Point ptMaxSize;
		public Point ptMaxPosition;
		private readonly Point ptMinTrackSize;
		private readonly Point ptMaxTrackSize;
	}

	private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized;
	}

	private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
	}

	private void OnCloseButtonClick(object sender, RoutedEventArgs e)
	{
		var mainVm = (MainViewModel)ViewModelManager.ViewModels[nameof(MainViewModel)];
		mainVm.Destroy();
		Close();
	}

	private void RefreshMaximizeRestoreButton()
	{
		switch (WindowState)
		{
			case WindowState.Maximized:
				MaximizeButton.Visibility = Visibility.Collapsed;
				RestoreButton.Visibility = Visibility.Visible;

				MainBorder.CornerRadius = new CornerRadius(0);
				ShadowBorder.CornerRadius = new CornerRadius(0);
				PopupBorder.CornerRadius = new CornerRadius(0);
				MainBorder.Margin = new Thickness(0);
				ShadowBorder.Margin = new Thickness(0);
				PopupBorder.Margin = new Thickness(0);
				break;
			case WindowState.Normal:
				MaximizeButton.Visibility = Visibility.Visible;
				RestoreButton.Visibility = Visibility.Collapsed;

				MainBorder.CornerRadius = new CornerRadius(8);
				ShadowBorder.CornerRadius = new CornerRadius(8);
				PopupBorder.CornerRadius = new CornerRadius(8);
				MainBorder.Margin = new Thickness(16);
				ShadowBorder.Margin = new Thickness(16);
				PopupBorder.Margin = new Thickness(16);
				break;
			case WindowState.Minimized:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Window_StateChanged(object sender, EventArgs e)
	{
		RefreshMaximizeRestoreButton();
	}

	private void PopupBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		var vm = (MainViewModel)DataContext;
		vm.OnPopupBorderClick();
	}
}