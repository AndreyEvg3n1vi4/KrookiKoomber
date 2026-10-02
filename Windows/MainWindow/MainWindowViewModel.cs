namespace KrookiKoomer;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;

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

	public MainWindowViewModel()
	{
		_isFullScreen = Avalonia.Controls.WindowState.Maximized;
		_di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
		_fileSystemItems = _di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

		if (_fileSystemItems.Length > 0)
			_selectedItem = _fileSystemItems[0];
	}

	[RelayCommand]
	private void GoToDirectory()
	{
		if (SelectedItem is DirectoryInfo di)
		{
			Di = new DirectoryInfo(di.FullName);
			FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

			if (FileSystemItems.Length > 0)
			{
				SelectedItem = FileSystemItems[0];
			}
			else
			{
				SelectedItem = null;
			}
		}
	}

	[RelayCommand]
	private void GoToBack()
	{
		if (Di.Parent is null)
			return;

		Di = Di.Parent;
		FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);

		if (FileSystemItems.Length > 0)
		{
			SelectedItem = FileSystemItems[0];
		}
		else
		{
			SelectedItem = null;
		}
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
