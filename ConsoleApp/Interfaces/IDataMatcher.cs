using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    public interface IDataMatcher
    {
        void MatchAndUpdate(IList<DataSourceObject> dataSource, IList<ImportedObject> importedObjects);
    }
}
