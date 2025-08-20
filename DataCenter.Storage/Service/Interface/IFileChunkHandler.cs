using Data_Center.Configuration.Constants;
using DataCenter.Domain.Domain;
using Microsoft.AspNetCore.Http;

namespace StorageService.Service.Interface;

public interface IFileChunkHandler
{
    Task ChunkProducerAsync(IFormFile file, FolderType folderType, Guid fileId, string connectionString, CancellationToken cancellationToken = default);
}