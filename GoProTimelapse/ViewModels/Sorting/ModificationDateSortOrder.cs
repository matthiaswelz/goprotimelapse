using System;
using System.Linq;

namespace journeyofcode.GoProTimelapse.ViewModels.Sorting
{
    public sealed class ModificationDateSortOrder
        : FileSortOrder
    {
        public ModificationDateSortOrder()
            : base("Modification Date")
        {
        }

        public override int Compare(FileViewModel x, FileViewModel y)
        {
            return x.ModificationDate.ValueOrDefault(DateTime.MinValue).CompareTo(y.ModificationDate.ValueOrDefault(DateTime.MinValue));
        }
    }
}
