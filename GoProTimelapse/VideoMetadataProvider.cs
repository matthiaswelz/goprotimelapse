using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Shell32;

namespace GoProTimelapse
{
    public sealed class VideoMetadataProvider
        : IDisposable
    {
        private static volatile VideoMetadataProvider _instance;

        public static VideoMetadataProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(VideoMetadataProvider))
                    {
                        if (_instance == null)
                            _instance = new VideoMetadataProvider();
                    }
                }

                return _instance;
            }
        }

        private Shell _shell;
        private readonly SingleThreadTaskScheduler _scheduler;

        public VideoMetadataProvider()
        {
            this._scheduler = new SingleThreadTaskScheduler(() => this._shell = new Shell());
        }

        public Task<TimeSpan> GetDuration(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                Folder dir = this._shell.NameSpace(Path.GetDirectoryName(path));
                FolderItem item = dir.ParseName(Path.GetFileName(path));

                String duration = dir.GetDetailsOf(item, 27);
                try
                {
                    return TimeSpan.Parse(duration);
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid timespan: " + duration + " on file " + path, e);
                }
            }, this._scheduler);
        }
        public DateTime GetCreationDate(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public Task<int> GetFPS(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                Folder dir = this._shell.NameSpace(Path.GetDirectoryName(path));
                FolderItem item = dir.ParseName(Path.GetFileName(path));
                
                String details = dir.GetDetailsOf(item, 300);
                try
                {
                    var fps = details.Substring(0, details.IndexOf(' ')).Trim().Trim('\x200E');
                    return Int32.Parse(fps);
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid FPS: " + details + " on file " + path, e);
                }
            }, this._scheduler);
        }

        public void Dispose()
        {
            this._scheduler.Dispose();
        }
    }
}