namespace PH.CompressionUtility.Test
{
    public class ZipUtilityExtensionsTest
    {
        CancellationToken token = CancellationToken.None;

        [Fact]
        public async Task ToZipAsyncEtyEnumerationWillReturnEmptyByteArrayTest()
        {
            FileInfo[] empty = [];
            
            var r = await empty.ToZipAsync(token);
            Assert.Empty(r);

        }

        [Fact]
        public async Task ToZipAsyncSingleFileTest()
        {
            var file = new FileInfo("test.txt");
            await File.WriteAllTextAsync(file.FullName, "This is a test file.", token);

            var files = new List<FileInfo> { file };
            var r     = await files.ToZipAsync(token);

            Assert.NotEmpty(r);
            Assert.True(r.Length > 0);

            // Clean up
            if (file.Exists)
            {
                file.Delete();
            }
        }
        [Fact]
        public async Task ToZipAsyncMultipleFilesTest()
        {
            var file1 = new FileInfo("test1.txt");
            var file2 = new FileInfo("test2.txt");
            await File.WriteAllTextAsync(file1.FullName, "This is a test file 1.", token);
            await File.WriteAllTextAsync(file2.FullName, "This is a test file 2.", token);

            var files = new List<FileInfo> { file1, file2 };
            var r     = await files.ToZipAsync(token);

            Assert.NotEmpty(r);
            Assert.True(r.Length > 0);

            // Clean up
            if (file1.Exists)
            {
                file1.Delete();
            }

            if (file2.Exists)
            {
                file2.Delete();
            }
        }
        
        [Fact]
        public async Task ToZipStreamAsyncEtyEnumerationWillReturnEmptyStreamTest()
        {
            FileInfo[] empty = [];
            var            r     = await empty.ToZipStreamAsync(token);
            Assert.NotNull(r);
            Assert.Equal(0, r.Length);
        }
        
        [Fact]
        public async Task ToZipStreamAsyncSingleFileTest()
        {
            var file = new FileInfo("test.txt");
            await File.WriteAllTextAsync(file.FullName, "This is a test file.", token);

            var files = new List<FileInfo> { file };
            var r     = await files.ToZipStreamAsync(token);

            Assert.NotNull(r);
            Assert.True(r.Length > 0);

            // Clean up
            if (file.Exists)
            {
                file.Delete();
            }
        }

        [Fact]
        public async Task ToZipStreamAsyncMultipleFilesTest()
        {
            var file1 = new FileInfo("test1.txt");
            var file2 = new FileInfo("test2.txt");
            await File.WriteAllTextAsync(file1.FullName, "This is a test file 1.");
            await File.WriteAllTextAsync(file2.FullName, "This is a test file 2.");

            var files = new List<FileInfo> { file1, file2 };
            var r     = await files.ToZipStreamAsync(token);

            Assert.NotNull(r);
            Assert.True(r.Length > 0);

            // Clean up
            if (file1.Exists)
            {
                file1.Delete();
            }

            if (file2.Exists)
            {
                file2.Delete();
            }
        }
        
        [Fact]
        public async Task ToZipAsyncDirectoryTest()
        {
            var dir = new DirectoryInfo("testdir");
            dir.Create();
            var file1 = new FileInfo(Path.Combine(dir.FullName, "test1.txt"));
            var file2 = new FileInfo(Path.Combine(dir.FullName, "test2.txt"));
            await File.WriteAllTextAsync(file1.FullName, "This is a test file 1.", token);
            await File.WriteAllTextAsync(file2.FullName, "This is a test file 2.", token);
            
            var dir2  = new DirectoryInfo("testdir/testdir2");
            dir2.Create();
            
            var file3 = new FileInfo(Path.Combine(dir2.FullName, "test3.txt"));
            await File.WriteAllTextAsync(file3.FullName, "This is a test file 2.", token);
            
            
            var dirs  = new List<DirectoryInfo> { dir };
            var r     = await dirs.ToZipStreamAsync(token);

            Assert.NotNull(r);
            Assert.True(r.Length > 0);

            // Clean up
            if (file1.Exists)
            {
                file1.Delete();
            }

            if (file2.Exists)
            {
                file2.Delete();
            }

            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }
        
        
        
        /*
        private async Task<DirectoryInfo> CreateATreeAsync()
        {
            for
        }*/
    }
}
