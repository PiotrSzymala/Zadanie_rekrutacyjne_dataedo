using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Models;

namespace ConsoleApp.Interfaces
{
    public interface IDataSourceLoader
    {
        IList<DataSourceObject> Load(string dataSourcePath);
    }
}
