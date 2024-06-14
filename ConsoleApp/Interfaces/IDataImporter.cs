using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    public interface IDataImporter
    {
        IList<ImportedObject> Import(string filePath);
    }
}
