namespace MovieQuotes.UI.Services;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

public class FilesService : IFilesService
{
    private readonly Window _target;

    public FilesService(Window target)
    {
        _target = target;
    }

    public async Task<IStorageFile?> OpenFileAsync()
    {
        var files = await _target.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select Subtitle File",
            AllowMultiple = false,
            FileTypeFilter = [new("subtitle File") {
                Patterns = ["*.srt"],
            }]
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
}