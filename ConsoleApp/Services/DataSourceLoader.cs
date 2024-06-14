using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace ConsoleApp.Services
{
    internal class DataSourceLoader : IDataSourceLoader
    {
        public IList<DataSourceObject> Load(string dataSourcePath)
        {
            try
            {

                var dataSource = new List<DataSourceObject>();

                using (var parser = new TextFieldParser(dataSourcePath))
                {
                    parser.SetDelimiters(new string[] { ";" });

                    while (!parser.EndOfData)
                    {
                        string[] values = parser.ReadFields();
                        dataSource.Add(new DataSourceObject
                        {
                            Id = int.TryParse(values[0], out int result) ? result : 0,
                            Type = values[1],
                            Name = values[2],
                            Schema = values[3],
                            ParentId = !string.IsNullOrEmpty(values[4]) ? Convert.ToInt32(values[4]) : 0,
                            ParentType = values[5],
                            Title = values[6],
                            Description = values[7],
                            CustomField1 = values[8],
                            CustomField2 = values[9],
                            CustomField3 = values[10]
                        });
                    }
                }
                return dataSource;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
