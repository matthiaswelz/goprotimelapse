using System;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using IOPath = System.IO.Path;

namespace GoProTimelapse
{
    public sealed class TempFile
        : CriticalFinalizerObject, IDisposable
    {
        public string Path { get; private set; }

        public TempFile(string extension = null)
        {
            this.Path = IOPath.Combine(IOPath.GetTempPath(), IOPath.GetRandomFileName() + "." + (extension ?? "tmp"));
        }

        ~TempFile()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool managed)
        {
            try
            {
                if (File.Exists(this.Path))
                    File.Delete(this.Path);

                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {

            }
        }
    }
}
