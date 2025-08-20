using DataCenter.Domain.Domain;
using DataCenter.Domain.Dto;

namespace StorageService.Service.Interface;

public interface IStorageService
{
    Task SaveChunkAsync(FileChunk chunk, CancellationToken cancellationToken);
    Task ReassembleAsync(Guid fileId, int bufferSize, int totalChunks, string chunksFolder, string finalPath, CancellationToken cancellationToken);

    Task CleanUpAsync(Guid fileId, int totalChunks, string chunksFolder, CancellationToken cancellationToken);
}