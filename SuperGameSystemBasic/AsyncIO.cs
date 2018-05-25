using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace SuperGameSystemBasic
{
    public static class AsyncIO
    {
        public static bool DoesFileExistAsync(StorageFolder folder, string fileName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFileAsync(fileName);
                    return f != null;
                }
                catch
                {
                    return false;
                }
            }).Result;
        }

        public static StorageFolder GetFolderAsync(StorageFolder folder, string folderName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFolderAsync(folderName);
                    return f;
                }
                catch
                {
                    return null;
                }
            }).Result;
        }

        public static StorageFile GetFileAsync(StorageFolder folder, string fileName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFileAsync(fileName);
                    return f;
                }
                catch
                {
                    return null;
                }
            }).Result;
        }

        public static StorageFolder GetParentAsync(StorageFolder folder)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetParentAsync();
                    return f;
                }
                catch
                {
                    return null;
                }
            }).Result;
        }

        public static List<StorageFolder> GetFoldersAsync(StorageFolder folder)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFoldersAsync();
                    return f;
                }
                catch
                {
                    return new List<StorageFolder>();
                }
            }).Result.ToList();
        }

        public static bool DoesFolderExistAsync(StorageFolder folder, string folderName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var f = await folder.GetFolderAsync(folderName);
                    return f != null;
                }
                catch
                {
                    return false;
                }
            }).Result;
        }

        public static IList<StorageFile> GetFilesAsync(StorageFolder folder)
        {
            var files = Task.Run(async () =>
            {
                try
                {
                    return await folder.GetFilesAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }).Result;
            var ret = new List<StorageFile>();
            if (files != null) ret.AddRange(files);
            return ret;
        }

        public static IList<StorageFile> GetFilesAsync(StorageFolder folder, QueryOptions options)
        {
            var files = Task.Run(async () =>
            {
                try
                {
                    return await folder.CreateFileQueryWithOptions(options).GetFilesAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }).Result;
            var ret = new List<StorageFile>();
            if (files != null) ret.AddRange(files);
            return ret;
        }

        public static StorageFolder CreateFolderAsync(StorageFolder folder, string folderName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                }
                catch
                {
                    return null;
                }
            }).Result;
        }

        public static StorageFile CreateFileAsync(StorageFolder folder, string fileName)
        {
            return Task.Run(async () =>
            {
                try
                {
                    return await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                }
                catch
                {
                    return null;
                }
            }).Result;
        }


        public static string ReadTextFileAsync(IStorageFile file)
        {
            return Task.Run(async () => await FileIO.ReadTextAsync(file)).Result;
        }

        public static void WriteStreamToFile(IStorageFile file, Stream stream)
        {
            Task.Run(async () =>
            {
                using (var ostream = await file.OpenStreamForWriteAsync())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(ostream);
                }
            });
        }

        public static Stream GetContentStream(string path)
        {
            return Task.Run(async () =>
            {
                var file = await Package.Current.InstalledLocation.GetFileAsync(path);
                return (await file.OpenReadAsync()).AsStreamForRead();
            }).Result;
        }
    }
}