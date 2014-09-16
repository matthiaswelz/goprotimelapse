using System;
using System.IO;
using System.Threading.Tasks;
using GoProTimelapse.Extensions;
using Shell32;

namespace GoProTimelapse.Helpers
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
        

        public void Dispose()
        {
            this._scheduler.Dispose();
        }
    }
}