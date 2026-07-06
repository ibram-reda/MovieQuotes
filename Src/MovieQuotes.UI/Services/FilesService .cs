namespace MovieQuotes.UI.Services;

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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

        if (OperatingSystem.IsWindows())
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            //Clean up file path so it can be navigated OK
            filePath = Path.GetFullPath(filePath);
            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
        }
        else if (OperatingSystem.IsLinux())
        {
            // Linux
            // Common file managers: nautilus (GNOME), dolphin (KDE), thunar (Xfce), nemo (Cinnamon), caja (MATE)

            string command = "nautilus"; // Example, you might need to check which one is installed

            Process? p;
            try
            {
                p = Process.Start(new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = $"\"{filePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start file manager: {ex.Message}");
                 return false;
            }
            if (p == null)
            {
                Console.WriteLine("Failed to start file manager.");
                return false;
            } 
            if (!p.WaitForExit(5000)) // Wait for 5 seconds
            {
                Console.WriteLine("File manager did not exit in time.");
                return false;
            }
            if (p.ExitCode != 0)
            {
                Console.WriteLine($"File manager exited with code {p.ExitCode}.");
                Console.WriteLine($"Error details: {p.StandardError.ReadToEnd()}");
                return false;
            }
        }
        else
        {
            Console.WriteLine("Unsupported operating system.");
        }

        return true;

    }
}