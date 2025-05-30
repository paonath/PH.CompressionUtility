using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PH.CompressionUtility
{
    /// <summary>
    /// Provides extension methods for creating ZIP archives from collections of files or directories.
    /// </summary>
    public static class ZipUtilityExtensions
    {
        /// <summary>
        /// Creates a ZIP archive from the specified collection of <see cref="FileInfo"/> objects and returns it as a byte array.
        /// </summary>
        /// <param name="files">The collection of <see cref="FileInfo"/> objects to include in the ZIP archive. Only existing files will be added.</param>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="level">The compression level to use when creating the ZIP archive. Defaults to <see cref="CompressionLevel.Optimal"/>.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a byte array representing the ZIP archive.
        /// If no valid files are provided, an empty byte array is returned.
        /// </returns>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="token"/>.</exception>
        public static async Task<byte[]> ToZipAsync(this IEnumerable<FileInfo> files, CancellationToken token, CompressionLevel level = CompressionLevel.Optimal)
        {
            token.ThrowIfCancellationRequested();
            var toAdd = files.Where(f => f.Exists).ToArray();
            if (!toAdd.Any())
            {
                return Array.Empty<byte>();
            }
            using (var memoryStream = new MemoryStream())
            {
                var r = await ToZipStreamAsync(toAdd, token, level);
                r.Position = 0; // Reset the position to the beginning of the stream
                await r.CopyToAsync(memoryStream);
                r.Dispose();
                memoryStream.Position = 0; 
                return memoryStream.ToArray();
            }
        }
        
        /// <summary>
        /// Creates a ZIP archive as a stream from the specified collection of files.
        /// </summary>
        /// <param name="files">
        /// A collection of <see cref="FileInfo"/> objects representing the files to include in the ZIP archive.
        /// Only existing files will be added to the archive.
        /// </param>
        /// <param name="token">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="level">
        /// The compression level to use when creating the ZIP archive. The default is <see cref="CompressionLevel.Optimal"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains a <see cref="Stream"/>
        /// representing the ZIP archive. If no valid files are provided, an empty stream is returned.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Thrown if the operation is canceled via the provided <paramref name="token"/>.
        /// </exception>
        public static async Task<Stream> ToZipStreamAsync(this IEnumerable<FileInfo> files, CancellationToken token,
                                                          CompressionLevel level = CompressionLevel.Optimal)
        {
            token.ThrowIfCancellationRequested();
            var toAdd        = files.Where(f => f.Exists).ToArray();
            if (toAdd.Length == 0)
            {
                return new MemoryStream();
            }

            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in toAdd)
                {
                    token.ThrowIfCancellationRequested();
                    if (file.Exists)
                    {
                        zipArchive.CreateEntryFromFile(file.FullName, file.Name, level);
                        await Task.Delay(0, token);
                    }
                }
            }

            return memoryStream;
        }

        /// <summary>
        /// Creates a ZIP archive stream containing the contents of the specified directories.
        /// </summary>
        /// <param name="directories">
        /// A collection of <see cref="DirectoryInfo"/> objects representing the directories to include in the ZIP archive.
        /// Only directories that exist will be included.
        /// </param>
        /// <param name="token">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="level">
        /// The compression level to use when creating the ZIP archive. Defaults to <see cref="CompressionLevel.Optimal"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Stream}"/> representing the asynchronous operation. The result contains a <see cref="Stream"/> 
        /// of the created ZIP archive.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Thrown if the operation is canceled via the provided <paramref name="token"/>.
        /// </exception>
        /// <remarks>
        /// This method filters out directories that do not exist before creating the ZIP archive.
        /// The returned stream must be disposed of by the caller to release resources.
        /// </remarks>
        public static async Task<Stream> ToZipStreamAsync(this IEnumerable<DirectoryInfo> directories, 
                                                          CancellationToken token,
                                                                 CompressionLevel level = CompressionLevel.Optimal)
        {
            var toAdd = directories.Where(d => d.Exists).ToArray();
            if (toAdd.Length == 0)
            {
                return new MemoryStream();
            }

            var memoryStream = new MemoryStream();
            using (var zipArchive =
                   new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var directoryInfo in toAdd)
                {
                    token.ThrowIfCancellationRequested();
                  await  zipArchive.AddFolderToZipArchive(directoryInfo, token, "", level);
                }
            }

            
            return memoryStream;
        }

        /// <summary>
        /// Adds the contents of a specified directory, including its subdirectories and files, to a <see cref="ZipArchive"/>.
        /// </summary>
        /// <param name="archive">The <see cref="ZipArchive"/> to which the directory contents will be added.</param>
        /// <param name="directory">The <see cref="DirectoryInfo"/> representing the directory to add to the archive.</param>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <param name="fullPath">
        /// An optional base path within the archive where the directory contents will be added. 
        /// If not specified, the directory's name will be used as the base path.
        /// </param>
        /// <param name="level">The compression level to use when adding files to the archive.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the <paramref name="token"/>.</exception>
        internal static async Task AddFolderToZipArchive(this ZipArchive archive, DirectoryInfo directory, CancellationToken token, string fullPath = "",
                                                         CompressionLevel level = CompressionLevel.Optimal)
        {
            var files = directory.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Exists).ToArray();
            
            if (files.Length > 0)
            {
                

                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    string filePath = $"{directory.Name}/{file.Name}";
                    if (!string.IsNullOrWhiteSpace(fullPath))
                    {
                        filePath = $"{fullPath}/{directory.Name}/{file.Name}";
                    }
                    
                    archive.CreateEntryFromFile(file.FullName,filePath, level);
                }
            }

            
            var dirs = directory.GetDirectories("*", SearchOption.AllDirectories).Where(x => x.Exists).ToArray();
            if (dirs.Length > 0)
            {
                fullPath = string.IsNullOrEmpty(fullPath) ? $"{directory.Name}" : $"{fullPath}/{directory.Name}";
                foreach (var dir in dirs)
                {
                    token.ThrowIfCancellationRequested();
                    
                    await AddFolderToZipArchive(archive, dir, token, fullPath, level);
                }
            }
        }
        
        
    }
}