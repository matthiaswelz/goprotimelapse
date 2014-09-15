using System;
using System.Collections.Generic;

namespace GoProTimelapse.ViewModels.Sorting
{
    public abstract class FileSortOrder
        : IComparer<FileViewModel>
    {
        public string Name { get; private set; }

        protected FileSortOrder(string name)
        {
            this.Name = name;
        }

        public abstract int Compare(FileViewModel x, FileViewModel y);

        public override string ToString()
        {
            return this.Name;
        }
    }
}