namespace MovieQuotes.UI.Services;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public async Task<IStorageFolder?> OpenFolderAsync(string title = "select folder")
    {
        var result = await _target.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = title,

        });
        return result.Count >= 1 ? result[0] : null;
    }

    public async Task<IStorageFile?> OpenFileAsync(string title = "select file", IReadOnlyList<FilePickerFileType>? type = null)
    {
        var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = type,
        });

        return files.Count >= 1 ? files[0] : null;
    }

    public async Task<IStorageFile?> SaveFileAsync()
    {
        return await _target.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save Text File"
        });
    }

    public bool ExploreFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }
        //Clean up file path so it can be navigated OK
        filePath = Path.GetFullPath(filePath);
        System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
        return true;
    }


}