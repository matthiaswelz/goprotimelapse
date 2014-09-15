using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace GoProTimelapse
{
    public class MainViewModel
        : ViewModelBase
    {
        private readonly IFileService _fileService = new WpfFileService();

        private bool _isBusy;
        private string _taskText;
        private string _outputFile;
        private string _targetFpsText;
        private string _targetDurationText;
        private FileViewModel _selectedFile;
        private readonly ObservableCollection<FileViewModel> _files;

        public ICommand AddFileCommand { get; private set; }
        public ICommand AddDirectoryCommand { get; private set; }
        public ICommand AddDirectoryTreeCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand SetOutputFileCommand { get; private set; }
        public ICommand StartCommand { get; private set; }

        public bool IsBusy
        {
            get { return this._isBusy; }
            private set
            {
                if (value.Equals(this._isBusy)) return;
                this._isBusy = value;
                this.OnPropertyChanged();
            }
        }

        public string TaskText
        {
            get { return this._taskText ?? "Nothing to do"; }
            private set
            {
                if (value == this._taskText) return;
                this._taskText = value;
                this.OnPropertyChanged();

                this.IsBusy = value != null;
            }
        }

        public string OutputFile
        {
            get { return this._outputFile; }
            set
            {
                if (value == this._outputFile) return;
                this._outputFile = value;
                this.OnPropertyChanged();
            }
        }

        public string TargetFPSText
        {
            get { return this._targetFpsText; }
            set
            {
                if (value == this._targetFpsText) return;
                this._targetFpsText = value;
                this.OnPropertyChanged();
            }
        }

        public int? TargetFPS
        {
            get
            {
                int result;
                if (!Int32.TryParse(this.TargetFPSText, out result))
                    return null;

                return result;
            }
        }

        public string TargetDurationText
        {
            get { return this._targetDurationText; }
            set
            {
                if (value == this._targetDurationText) return;
                this._targetDurationText = value;
                this.OnPropertyChanged();
            }
        }

        public TimeSpan? TargetDuration
        {
            get
            {
                TimeSpan result;
                if (!TimeSpan.TryParse(this.TargetDurationText, out result))
                    return null;

                return result;
            }
        }

        public ReadOnlyObservableCollection<FileViewModel> Files { get; private set; }

        public FileViewModel SelectedFile
        {
            get { return this._selectedFile; }
            set
            {
                if (value == this._selectedFile) return;
                this._selectedFile = value;
                this.OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            this._files = new ObservableCollection<FileViewModel>();
            this.Files = new ReadOnlyObservableCollection<FileViewModel>(this._files);

            this.TargetFPSText = "30";
            this.TargetDurationText = "00:01:00";

            this.InitializeCommands();
        }


        private void InitializeCommands()
        {
            this.AddFileCommand = new DelegateCommand(this.AddFile, this.CanDo);
            this.AddDirectoryCommand = new DelegateCommand(this.AddDirectory, this.CanDo);
            this.AddDirectoryTreeCommand = new DelegateCommand(this.AddDirectoryTree, this.CanDo);
            this.RemoveCommand = new DelegateCommand(this.Remove, this.CanRemove);
            this.ClearCommand = new DelegateCommand(this.Clear, this.CanClear);
            this.SetOutputFileCommand = new DelegateCommand(this.SetOutputFile, this.CanDo);
            this.StartCommand = new DelegateCommand(this.Start, this.CanStart);
        }


        private bool CanDo()
        {
            return !this.IsBusy;
        }
        private bool CanStart()
        {
            return !this.IsBusy && this.Files.Count > 0;
        }

        private bool CanClear()
        {
            return this.Files.Count > 0;
        }

        private bool CanRemove()
        {
            return this.SelectedFile != null;
        }

        private async void Start()
        {
            using (this.DoTask("Working..."))
            {
                this.TaskText = "Locating ffmpeg";

                var ffmpegPath = await Task.Factory.StartNew(() => new[] { "ffmpeg.exe", @"C:\ffmpeg\ffmpeg.exe", @"C:\Program Files\ffmpeg\bin\ffmpeg.exe" }.FirstOrDefault(File.Exists));
                if (ffmpegPath == null)
                {
                    MessageBox.Show("Could not locate ffmpeg.exe. Please make sure that ffmpeg.exe is within %PATH%.");
                    return;
                }

                this.TaskText = "Determining video metadata";

                await Task.Factory.StartNew(() => Task.WaitAll(this._files.Select(file => file.EnsureResults()).ToArray()));

                this.TaskText = "Calculating duration";

                if (!this.TargetFPS.HasValue || !this.TargetDuration.HasValue)
                {
                    MessageBox.Show("Invalid FPS / Duration.");
                    return;
                }

                var targetFps = this.TargetFPS;
                var targetDuration = this.TargetDuration.Value;
                var files = this._files.ToArray();
                var outputFile = this.OutputFile;

                var totalDuration = await Task.Factory.StartNew(() => files.Select(v => v.Duration.Value).Aggregate((a, b) => a.Add(b)));
                var factor = (totalDuration.TotalSeconds / targetDuration.TotalSeconds);

                this.TaskText = String.Format("Generating timelapse - {0} videos ({1}), Factor: {2:0.####}", files.Length, totalDuration, factor);

                await Task.Factory.StartNew(() =>
                {
                    using (var tempFile = new TempFile("txt"))
                    {
                        var lines = files
                            .OrderBy(file => file.CreationDate)
                            .Select(f => String.Format("file '{0}'", f.FullPath))
                            .ToArray();
                        File.WriteAllLines(tempFile.Path, lines);

                        var psi = new ProcessStartInfo
                        {
                            FileName = ffmpegPath,
                            Arguments = String.Format(CultureInfo.InvariantCulture, "-f concat -i \"{0}\" -filter:v setpts=1/{1:0.####}*PTS -r {2} -an \"{3}\"", tempFile.Path, factor, targetFps, outputFile),
                            WorkingDirectory = Environment.CurrentDirectory,
                            UseShellExecute = false,
                        };

                        var process = Process.Start(psi);
                        process.PriorityClass = ProcessPriorityClass.BelowNormal;
                        process.WaitForExit();
                    }
                });
            }
        }

        private void SetOutputFile()
        {
            var file = this._fileService.SaveFile("mp4");
            if (file == null)
                return;

            this.OutputFile = file;
        }

        private void Clear()
        {
            this._files.Clear();
            this.SelectedFile = null;
        }

        private void Remove()
        {
            this._files.Remove(this.SelectedFile);
            this.SelectedFile = null;
        }

        private void AddDirectoryTree()
        {
            this.AddDirectory(SearchOption.AllDirectories);
        }
        private void AddDirectory()
        {
            this.AddDirectory(SearchOption.TopDirectoryOnly);
        }

        private async void AddDirectory(SearchOption option)
        {
            using (this.DoTask("Adding directory"))
            {
                var root = this._fileService.SelectDirectory();
                if (root == null)
                    return;

                var files = await DoInBackground(() => Directory.EnumerateFiles(root, "*.mp4", option).Select(file => new FileViewModel(file)).ToArray());
                if (files != null)
                    this._files.AddRange(files);
            }
        }

        private void AddFile()
        {
            var files = this._fileService.SelectFiles("mp4");
            if (files == null)
                return;

            this._files.AddRange(files.Select(f => new FileViewModel(f)));
        }

        private IDisposable DoTask(string text)
        {
            this.TaskText = text;

            return new Disposer(() => this.TaskText = null);
        }

        private Task<T> DoInBackground<T>(Func<T> func)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An exception occured: " + ex);
                    return default(T);
                }
            });
        }
    }
}
