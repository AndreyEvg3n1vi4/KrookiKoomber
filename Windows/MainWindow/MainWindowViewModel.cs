namespace KrookiKoomer;

using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

internal partial class MainWindowViewModel : ObservableObject
{
	[ObservableProperty]
	private Avalonia.Controls.WindowState _isFullScreen;
	[ObservableProperty]
	private DirectoryInfo _di;
	[ObservableProperty]
	private FileSystemInfo[] _fileSystemItems;
	[ObservableProperty]
	private FileSystemInfo? _selectedItem;
	private FileSystemWatcher? _watcher;
	private System.Timers.Timer? _oneShotTimer;

	public MainWindowViewModel()
	{
		//Debug.WriteLine($"UI Thread: {Thread.CurrentThread.ManagedThreadId}");

		_isFullScreen = Avalonia.Controls.WindowState.Maximized;
		_di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
		_fileSystemItems = _di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

		if (_fileSystemItems.Length > 0)
			_selectedItem = _fileSystemItems[0];

		_watcher = new FileSystemWatcher(_di.FullName)
		{
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
			IncludeSubdirectories = false,
			InternalBufferSize = 64 * 1024,
		};

		_watcher.Created += OnWatcherEventsHandler;
		_watcher.Deleted += OnWatcherEventsHandler;
		_watcher.Renamed += OnWatcherEventsHandler;
		_watcher.Changed += OnWatcherEventsHandler;
		_watcher.Error += OnWatcherEventsHandler;
		_watcher.EnableRaisingEvents = true;
	}

	private void FillFileSystemItems()
	{
		FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

		SelectedItem = FileSystemItems.Length > 0 ?
			FileSystemItems[0] : null;
	}

	private void OnWatcherEventsHandler(object sender, EventArgs e)
	{
		if (_oneShotTimer != null)
			return;

		_oneShotTimer = new System.Timers.Timer(TimeSpan.FromSeconds(3));

		_oneShotTimer.Elapsed += TimerShot;
		_oneShotTimer.AutoReset = false;
		_oneShotTimer.Start();
	}

	private void TimerShot(object? sender, EventArgs e)
	{
		_oneShotTimer!.Stop();
		_oneShotTimer.Elapsed -= TimerShot;
		_oneShotTimer.Dispose();
		_oneShotTimer = null;

		Dispatcher.UIThread.Post(FillFileSystemItems);
	}

	[RelayCommand]
	private void GoToDirectory()
	{
		if (!(SelectedItem is DirectoryInfo di))
			return;

		Di = new DirectoryInfo(di.FullName);
		FillFileSystemItems();
	}

	[RelayCommand]
	private void GoToBack()
	{
		if (Di.Parent is null)
			return;

		Di = Di.Parent;

		FillFileSystemItems();
	}

	[RelayCommand]
	private void SwitchFullScreen()
	{
		if (IsFullScreen != Avalonia.Controls.WindowState.FullScreen)
		{
			IsFullScreen = Avalonia.Controls.WindowState.FullScreen;
		}
		else
		{
			IsFullScreen = Avalonia.Controls.WindowState.Maximized;
		}
	}
}
