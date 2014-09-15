using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoProTimelapse.Annotations;

namespace GoProTimelapse
{
    public interface IFileService
    {
        string SaveFile(string extension = null);
        string SelectFile(string extension = null);
        IEnumerable<string> SelectFiles(string extension = null);
        string SelectDirectory(string description = null);
    }
}
