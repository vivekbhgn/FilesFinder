using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieFolderWatcher
{
    class Program
    {
        static List<string> directories = new List<string>();
        static List<string> dirCopy = new List<string>();
        static List<FileAttributes> listFiles = new List<FileAttributes>();

        static void Main(string[] args)
        {
            try
            {
                string[] drives = System.Environment.GetLogicalDrives();
                ParallelOptions p = new ParallelOptions();
                p.MaxDegreeOfParallelism = 3;

                foreach (var drive in drives)
                {
                    try
                    {
                        directories.Add(drive);
                        dirCopy.Add(drive);
                        //    var files = System.IO.Directory.GetFiles(drive);
                        //    Parallel.ForEach(files, p, file =>
                        //        {
                        //            try
                        //            {
                        //                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        //                listFiles.Add(new FileAttributes { Path = fi.FullName, CreatedDate = fi.CreationTime, LastModifiedDate = fi.LastWriteTime });
                        //            }
                        //            catch (UnauthorizedAccessException)
                        //            {

                        //            }


                        //        });
                    }
                    catch (UnauthorizedAccessException)
                    {

                    }

                }
                while (!(directories.Count == 0))
                {
                    directories.Clear();
                    directories.AddRange(dirCopy);
                    dirCopy.Clear();
                    foreach (var pathDir in directories)
                    {
                        ProcessDirectories(pathDir);
                    }
                }

                var musicFiles = listFiles.Where(l => l != null && l.FileType != null && l.FileType == (int)FileType.Music);
                var movieFiles = listFiles.Where(l => l != null && l.FileType != null && l.FileType == (int)FileType.Media);
                var docFiles = listFiles.Where(l => l != null && l.FileType != null && l.FileType == (int)FileType.Documents);
                var largeMovieFiles = movieFiles.Where(l => l != null && l.FileType == (int)FileType.Media && l.FileSize > 300);
            }
            catch (UnauthorizedAccessException)
            {

            }
        }


        private static void ProcessDirectories(string pathDir)
        {
            try
            {
                var folders = Directory.EnumerateDirectories(pathDir, "*", SearchOption.TopDirectoryOnly);
                dirCopy.AddRange(folders);
                try
                {
                    var files = System.IO.Directory.GetFiles(pathDir);
                    ParallelOptions p = new ParallelOptions();
                    p.MaxDegreeOfParallelism = 6;
                    Parallel.ForEach(files, p, file =>
                    {
                        try
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(file);
                            listFiles.Add(
                                new FileAttributes
                                {
                                    Path = fi.FullName,
                                    CreatedDate = fi.CreationTime,
                                    LastModifiedDate = fi.LastWriteTime,
                                    Extension = fi.Extension,
                                    FileType = GetFileType(fi.Extension),
                                    FileSize = fi.Length / 1000000
                                });
                        }
                        catch (UnauthorizedAccessException)
                        {

                        }


                    });
                }
                catch (UnauthorizedAccessException)
                {

                }
            }
            catch (UnauthorizedAccessException)
            {

            }
        }

        private static int GetFileType(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return (int)FileType.Other;
            }
            switch (extension)
            {
                case ".mp4":
                case ".mov":
                case ".mkv":
                case ".avi":
                case ".webm":
                    return (int)FileType.Media;
                case ".mp3":
                case ".flac":
                case ".ogg":
                    return (int)FileType.Music;
                case ".ppt":
                case ".doc":
                case ".docx":
                case ".xlsx":
                case ".xls":
                case ".xml":
                    return (int)FileType.Documents;
                default:
                    return (int)FileType.Other;

            }
        }




    }



    class FileAttributes
    {
        public string Path { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Extension { get; set; }
        public int FileType { get; set; }
        public long FileSize { get; set; }
    }
    enum FileType
    {
        Media = 1,
        Documents = 2,
        Pictures = 3,
        Music = 4,
        Other = 5
    }
}
