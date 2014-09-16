using System;
using System.IO;
using System.Linq;

namespace journeyofcode.GoProTimelapse.ViewModels.Sorting
{
    public sealed class GoProFileNameSorting
        : FileSortOrder
    {
        public GoProFileNameSorting()
            : base("GoPro File name")
        {
        }

        public override int Compare(FileViewModel x, FileViewModel y)
        {
            var a = this.ExtractNumber(x.FileName);
            var b = this.ExtractNumber(y.FileName);

            if (a.Item1 == b.Item1)
                return a.Item2.CompareTo(b.Item2);

            return a.Item1.CompareTo(b.Item1);
        }

        private Tuple<int, int> ExtractNumber(string nameWithExtension)
        {
            var name = Path.GetFileNameWithoutExtension(nameWithExtension);
            if (name.Length != 8)
                return new Tuple<int, int>(0, 0);

            var part1 = name.Substring(4, 4);
            var part2 = name.Substring(2, 2);

            if (String.Equals("PR", part2, StringComparison.InvariantCultureIgnoreCase))
                part2 = "00";

            int item1;
            int item2;
            if (!Int32.TryParse(part1, out item1) || !Int32.TryParse(part2, out item2))
                return new Tuple<int, int>(0, 0);

            return new Tuple<int, int>(item1, item2);
        }
    }
}
