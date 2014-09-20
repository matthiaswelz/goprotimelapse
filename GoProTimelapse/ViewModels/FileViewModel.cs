using System;
using System.IO;
using System.Threading.Tasks;
using journeyofcode.GoProTimelapse.Helpers;
using journeyofcode.Threading;

namespace journeyofcode.GoProTimelapse.ViewModels
{
    public sealed class FileViewModel
        : ViewModelBase
    {
        public string FullPath { get; private set; }

        public string FileName
        {
            get { return Path.GetFileName(this.FullPath); }
        }

        public string DirectoryName
        {
            get { return Path.GetDirectoryName(this.FullPath); }
        }

        public TaskLazy<TimeSpan> Duration { get; private set; }
        public TaskLazy<DateTime> CreationDate { get; private set; }
        public TaskLazy<DateTime> ModificationDate { get; private set; }

        public bool AreMetadataReady
        {
            get { return this.MetadataTask.IsCompleted; }
        }
        public Task MetadataTask { get; private set; }

        public string DurationText
        {
            get { return this.Duration.ActionOrDefault(t => t.ToString(), "<calculating...>"); }
        }

        public FileViewModel(string path)
        {
            this.FullPath = path;

            this.Duration = VideoMetadataProvider.Instance.GetDuration(this.FullPath);
            this.CreationDate = Task.Run(() => File.GetCreationTimeUtc(this.FullPath));
            this.ModificationDate = Task.Run(() => File.GetLastWriteTimeUtc(this.FullPath));

            this.Duration.WhenValueAvailable(() => this.OnPropertyChanged("DurationText"));
            
            this.MetadataTask = Task.WhenAll(this.Duration, this.CreationDate, this.ModificationDate);
        }
    }
}