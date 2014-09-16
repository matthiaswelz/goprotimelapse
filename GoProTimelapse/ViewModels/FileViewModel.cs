using System;
using System.IO;
using System.Threading.Tasks;
using journeyofcode.GoProTimelapse.Helpers;

namespace journeyofcode.GoProTimelapse.ViewModels
{
    public sealed class FileViewModel
        : ViewModelBase
    {
        private TimeSpan? _duration;
        private int? _fps;
        private Task _task;
        private DateTime? _creationDate;
        private DateTime? _modificationDate;
        public string FullPath { get; private set; }

        public string FileName
        {
            get { return Path.GetFileName(this.FullPath); }
        }

        public string DirectoryName
        {
            get { return Path.GetDirectoryName(this.FullPath); }
        }

        public TimeSpan? Duration
        {
            get { return this._duration; }
            private set
            {
                if (value.Equals(this._duration)) return;
                this._duration = value;
                
                this.OnPropertyChanged();
                this.OnPropertyChanged("DurationText");
            }
        }

        public string DurationText
        {
            get { return this.Duration.HasValue ? this.Duration.ToString() : "<calculating...>"; }
        }

        public DateTime? CreationDate
        {
            get { return this._creationDate; }
            private set
            {
                if (value.Equals(this._creationDate)) return;
                this._creationDate = value;
                this.OnPropertyChanged();
            }
        }

        public DateTime? ModificationDate
        {
            get { return this._modificationDate; }
            set
            {
                if (value.Equals(this._modificationDate)) return;
                this._modificationDate = value;
                this.OnPropertyChanged();
            }
        }

        public bool HasLoadedMetadata
        {
            get { return this.Duration.HasValue; }
        }

        public FileViewModel(string path)
        {
            this.FullPath = path;
            this.Initialize();
        }

        public void EnsureMetadata()
        {
            this._task.Wait();
        }

        private async void Initialize()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                return new {
                    CreationDate = File.GetCreationTimeUtc(this.FullPath),
                    Duration = await VideoMetadataProvider.Instance.GetDuration(this.FullPath),
                    ModificationDate = File.GetLastWriteTimeUtc(this.FullPath)
                };
            }).Unwrap();
            this._task = task;
            var data = await task;

            this.Duration = data.Duration;
            this.CreationDate = data.CreationDate;
            this.ModificationDate = data.ModificationDate;
        }
    }
}