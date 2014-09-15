using System;
using System.Collections.Generic;
using System.Linq;

namespace GoProTimelapse
{
    public interface IFileService
    {
        string SaveFile(string extension = null);
        IEnumerable<string> SelectFiles(string extension = null);
        string SelectDirectory(string description = null);
    }
}
