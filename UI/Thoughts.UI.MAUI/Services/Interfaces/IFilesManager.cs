﻿using Thoughts.UI.MAUI.ViewModels;

namespace Thoughts.UI.MAUI.Services.Interfaces
{
    public interface IFilesManager
    {
        Task<bool> UploadLimitSizeFileAsync(FileResult file, CancellationToken token = default);

        Task<bool> UploadAnyFileAsync(FileResult file, CancellationToken token = default);

        Task<IEnumerable<FileViewModel>> GetFilesAsync(int page = default, CancellationToken token = default);

        IEnumerable<FileViewModel> GetFiles(int page = default);

        bool UploadLimitSizeFile(FileResult file);

        bool UploadAnyFile(FileResult file);
    }
}