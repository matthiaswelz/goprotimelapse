using System;
using System.Linq;

namespace journeyofcode.GoProTimelapse.ViewModels.Sorting
{
    public sealed class CreationDateSortOrder
        : FileSortOrder
    {
        public CreationDateSortOrder() 
            : base("Creation Date")
        {
        }

        public override int Compare(FileViewModel x, FileViewModel y)
        {
            return x.CreationDate.GetValueOrDefault(DateTime.MinValue).CompareTo(y.CreationDate.GetValueOrDefault(DateTime.MinValue));
        }
    }
}
