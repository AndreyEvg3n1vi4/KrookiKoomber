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
		_isFullScreen = Avalonia.Controls.WindowState.Maximized;
		_di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

		FillFileSystemItems();
		InitWatcher();
	}

	private void FillFileSystemItems()
	{
		FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

		SelectedItem = FileSystemItems.Length > 0 ?
			FileSystemItems[0] : null;
	}

	private void InitWatcher()
	{
		if (_watcher != null)
		{
			_watcher.EnableRaisingEvents = false;
			_watcher.Created -= OnWatcherEventsHandler;
			_watcher.Deleted -= OnWatcherEventsHandler;
			_watcher.Renamed -= OnWatcherEventsHandler;
			_watcher.Changed -= OnWatcherEventsHandler;
			_watcher.Error -= OnWatcherEventsHandler;
			_watcher.Dispose();
		}

		_watcher = new FileSystemWatcher(Di.FullName)
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

	private void ClearTimer()
	{
		if (_oneShotTimer is null)
			return;

		_oneShotTimer.Stop();
		_oneShotTimer.Elapsed -= TimerShot;
		_oneShotTimer.Dispose();
		_oneShotTimer = null;
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
		ClearTimer();
		Dispatcher.UIThread.Post(FillFileSystemItems);
	}

	[RelayCommand]
	private void GoTo(object? back = null)
	{
		if (back is null)
		{
			if (!(SelectedItem is DirectoryInfo di))
				return;

			Di = new DirectoryInfo(di.FullName);
		}
		else
		{
			if (Di.Parent is null)
				return;

			Di = Di.Parent;
		}

		ClearTimer();
		FillFileSystemItems();
		InitWatcher();
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
