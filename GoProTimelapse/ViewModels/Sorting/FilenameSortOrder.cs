using System;
using System.Linq;

namespace journeyofcode.GoProTimelapse.ViewModels.Sorting
{
    public sealed class FilenameSortOrder
        : FileSortOrder
    {
        public FilenameSortOrder() : base("File name")
        {
        }

        public override int Compare(FileViewModel x, FileViewModel y)
        {
            return String.Compare(x.FileName, y.FileName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
