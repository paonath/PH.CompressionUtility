# PH.CompressionUtility

# Documentation: Using `ZipUtilityExtensions`

The `ZipUtilityExtensions` class provides extension methods for creating ZIP archives from collections of files or directories. It supports asynchronous operations and allows customization of compression levels.

## Features

1. **Create ZIP from Files**:
   - Converts a collection of `FileInfo` objects into a ZIP archive as a byte array or stream.
   - Skips non-existent files.

2. **Create ZIP from Directories**:
   - Converts a collection of `DirectoryInfo` objects into a ZIP archive as a stream.
   - Recursively includes all files and subdirectories.

3. **Customizable Compression Levels**:
   - Supports `CompressionLevel.Optimal` by default, but can be customized.

---

## Methods

### `ToZipAsync`

Creates a ZIP archive from a collection of files and returns it as a byte array.

```csharp
public static Task<byte[]> ToZipAsync(
    this IEnumerable<FileInfo> files,
    CancellationToken token,
    CompressionLevel level = CompressionLevel.Optimal
)
```

### `ToZipStreamAsync`

Creates a ZIP archive from a collection of files or directories and returns it as a stream.

```csharp
public static Task<Stream> ToZipStreamAsync(
    this IEnumerable<FileInfo> files,
    CancellationToken token,
    CompressionLevel level = CompressionLevel.Optimal
)

public static Task<Stream> ToZipStreamAsync(
    this IEnumerable<DirectoryInfo> directories,
    CancellationToken token,
    CompressionLevel level = CompressionLevel.Optimal
)
```

---

## Usage Examples

### Example 1: Create ZIP from Files (Byte Array)

```csharp
var file1 = new FileInfo("file1.txt");
var file2 = new FileInfo("file2.txt");
await File.WriteAllTextAsync(file1.FullName, "Content of file 1");
await File.WriteAllTextAsync(file2.FullName, "Content of file 2");

var files = new List<FileInfo> { file1, file2 };
var token = CancellationToken.None;

byte[] zipBytes = await files.ToZipAsync(token);

File.WriteAllBytes("output.zip", zipBytes);
```

---

### Example 2: Create ZIP from Files (Stream)

```csharp
var file1 = new FileInfo("file1.txt");
await File.WriteAllTextAsync(file1.FullName, "Content of file 1");

var files = new List<FileInfo> { file1 };
var token = CancellationToken.None;

using var zipStream = await files.ToZipStreamAsync(token);
using var fileStream = new FileStream("output.zip", FileMode.Create, FileAccess.Write);

await zipStream.CopyToAsync(fileStream);
```

---

### Example 3: Create ZIP from Directories

```csharp
var dir = new DirectoryInfo("testdir");
dir.Create();
await File.WriteAllTextAsync(Path.Combine(dir.FullName, "file1.txt"), "Content of file 1");

var directories = new List<DirectoryInfo> { dir };
var token = CancellationToken.None;

using var zipStream = await directories.ToZipStreamAsync(token);
using var fileStream = new FileStream("output.zip", FileMode.Create, FileAccess.Write);

await zipStream.CopyToAsync(fileStream);
```

---

## Tests

The `ZipUtilityExtensionsTest` class provides comprehensive test coverage for the utility. Below are some key test cases:

1. **Empty File Collection**:
   - Ensures that an empty collection returns an empty ZIP archive.

2. **Single File**:
   - Verifies that a single file is correctly added to the ZIP archive.

3. **Multiple Files**:
   - Confirms that multiple files are included in the ZIP archive.

4. **Empty Directory Collection**:
   - Ensures that an empty directory collection returns an empty ZIP archive.

5. **Directory with Files**:
   - Verifies that all files in a directory (including subdirectories) are included in the ZIP archive.

---

## Conclusion

The `ZipUtilityExtensions` class is a powerful utility for creating ZIP archives from files and directories. Its asynchronous methods and support for customizable compression levels make it suitable for a wide range of applications. The provided tests ensure reliability and correctness.