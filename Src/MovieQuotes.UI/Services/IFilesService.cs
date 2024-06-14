﻿namespace MovieQuotes.UI.Services;

using Avalonia.Platform.Storage;
using System.Threading.Tasks;

public interface IFilesService
{
    public Task<IStorageFile?> OpenFileAsync(string title="select file");
    public Task<IStorageFile?> SaveFileAsync();
}