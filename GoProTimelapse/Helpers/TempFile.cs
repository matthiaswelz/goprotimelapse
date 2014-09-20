using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace journeyofcode.GoProTimelapse.Helpers
{
    public sealed class TempFile
        : CriticalFinalizerObject, IDisposable
    {
        public string Path { get; private set; }

        public TempFile(string extension = null)
        {
            this.Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName() + "." + (extension ?? "tmp"));
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
            catch (Exception ex)
            {
                Trace.Fail(ex.ToString());
            }
        }
    }
}
