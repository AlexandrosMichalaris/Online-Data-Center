using System.Runtime.CompilerServices;

namespace StorageService.Extensions;

public static class StreamExtension
{
    public static async IAsyncEnumerable<(byte[] Chunk, int Index)> ReadChunksAsync(
        this Stream stream,
        int bufferSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var buffer = new byte[bufferSize];
        int index = 0;

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            // Return bytes and index of chunk
            yield return (buffer.Take(bytesRead).ToArray(), index);
            index++;
        }
    }
}