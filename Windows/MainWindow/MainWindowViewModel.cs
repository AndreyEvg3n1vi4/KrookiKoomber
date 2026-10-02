namespace KrookiKoomer;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;

internal partial class MainWindowViewModel : ObservableObject
{
	[ObservableProperty]
	private DirectoryInfo _di;
	[ObservableProperty]
	private FileSystemInfo[] _fileSystemItems;
	[ObservableProperty]
	private FileSystemInfo? _selectedItem;

	public MainWindowViewModel()
	{
		_di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
		_fileSystemItems = _di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
	}

	[RelayCommand]
	private void GoToDirectory()
	{
		if (SelectedItem is DirectoryInfo di)
		{
			Di = new DirectoryInfo(di.FullName);
			FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
			SelectedItem = null;
		}
	}

	[RelayCommand]
	private void GoToBack()
	{
		if (Di.Parent is null)
			return;

		Di = Di.Parent;
		FileSystemItems = Di.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
		SelectedItem = null;
	}
}
