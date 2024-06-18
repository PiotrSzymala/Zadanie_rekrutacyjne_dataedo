using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;
using ConsoleApp.Models;
using Microsoft.Extensions.Logging;
using NLog;

namespace ConsoleApp.Services
{
    public class DataSourceLoader : IDataSourceLoader
    {
        private readonly ILogger<DataSourceLoader> _logger;
        private readonly IFileReader _fileReader;

        public DataSourceLoader(ILogger<DataSourceLoader> logger, IFileReader fileReader)
        {
            _logger = logger;
            _fileReader = fileReader;
        }

        /// <summary>
        /// Loads data source objects from the specified file path.
        /// </summary>
        /// <param name="dataSourcePath">The path to the data source file.</param>
        /// <returns>A list of <see cref="DataSourceObject"/> representing the loaded data.</returns>
        /// <exception cref="Exception">Thrown when there is an error processing the data.</exception>
        public IList<DataSourceObject> Load(string dataSourcePath)
        {
            try
            {
                var dataSource = new List<DataSourceObject>();
                var lines = _fileReader.ReadAllLines(dataSourcePath);

                foreach (var line in lines.Skip(1))
                {
                    var values = _fileReader.Split(line, ';');
                   
                    var dataSourceObject = CreateDataSourceObject(values);
                  
                    dataSource.Add(dataSourceObject);
                }
                return dataSource;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }

        /// <summary>
        /// Creates a data source object from a list of values.
        /// </summary>
        /// <param name="values">The values to create the data source object from.</param>
        /// <returns>A <see cref="DataSourceObject"/> created from the values.</returns>
        /// <exception cref="ArgumentException">Thrown when the data is malformed or cannot be converted.</exception>
        private static DataSourceObject CreateDataSourceObject(IReadOnlyList<string> values)
        {
            if(values.Count <11)
                throw new ArgumentException("Malformed data: Not enough fields");

            var dataSourceObject = new DataSourceObject
            {
                Id = int.TryParse(values[0], out var id) ? id : throw new ArgumentException($"Cannot convert id: {values[0]}"),
                Type = values[1],
                Name = values[2],
                Schema = values[3],
                ParentId = TryParseParentId(values[4]),
                ParentType = values[5],
                Title = values[6],
                Description = values[7],
                CustomField1 = values[8],
                CustomField2 = values[9],
                CustomField3 = values[10]
            };

            return dataSourceObject;
        }

        /// <summary>
        /// Tries to parse the parent ID from a string.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed parent ID, or 0 if the parsing fails.</returns>
        private static int TryParseParentId(string value)
        {
            return int.TryParse(value, out var parentId) ? parentId : 0;
        }
    }
}
