using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleApp.Interfaces;
using ConsoleApp.Models;
using ConsoleApp.Utilities;
using Microsoft.Extensions.Logging;
using NLog;

namespace ConsoleApp.Services
{
    public class DataImporter : IDataImporter
    {
        private readonly ILogger<DataImporter> _logger;

        public DataImporter(ILogger<DataImporter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Imports data from a specified CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>A list of <see cref="ImportedObject"/> representing the imported data.</returns>
        /// <exception cref="Exception">Thrown when there is an error processing the data.</exception>
        public IList<ImportedObject> Import(string filePath)
        {
            try
            {
                var importedObjects = new List<ImportedObject>();
                using (var streamReader = new StreamReader(filePath))
                {

                    var headerLine = streamReader.ReadLine();

                    var headerMap = ParseHeaders(headerLine);

                    string line;

                    var lineCounter = 1;


                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var values = line.Split(';');

                        var importedObject = ParseLine(values, headerMap);

                        var emptyProperties = GetEmptyProperties(importedObject);

                        if (emptyProperties.Any())
                            _logger.LogWarning($"Empty fields found in line {lineCounter}: {string.Join(", ", emptyProperties)}");

                        importedObjects.Add(importedObject);

                        lineCounter++;
                    }
                }

                return importedObjects;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }

        /// <summary>
        /// Parses the header line of the CSV file to create a map of header names to their respective indices.
        /// </summary>
        /// <param name="headerLine">The header line of the CSV file.</param>
        /// <returns>A dictionary mapping header names to their indices.</returns>
        private static Dictionary<string, int> ParseHeaders(string headerLine)
        {
            return headerLine.Split(';')
                .Select((header, index) => new { Header = header, Index = index })
                .ToDictionary(h => h.Header, h => h.Index);
        }

        /// <summary>
        /// Parses a line of values from the CSV file into an <see cref="ImportedObject"/>.
        /// </summary>
        /// <param name="values">The values from the CSV line.</param>
        /// <param name="headerMap">The map of header names to their indices.</param>
        /// <returns>An <see cref="ImportedObject"/> representing the data from the line.</returns>
        private static ImportedObject ParseLine(IReadOnlyList<string> values,
            IReadOnlyDictionary<string, int> headerMap)
        {
            return new ImportedObject
            {
                Type = GetValueSafely(values, GetIndexSafely(headerMap, "Type")),
                Name = GetValueSafely(values, GetIndexSafely(headerMap, "Name")),
                Schema = GetValueSafely(values, GetIndexSafely(headerMap, "Schema")),
                ParentName = GetValueSafely(values, GetIndexSafely(headerMap, "ParentName")),
                ParentType = GetValueSafely(values, GetIndexSafely(headerMap, "ParentType")),
                ParentSchema = GetValueSafely(values, GetIndexSafely(headerMap, "ParentSchema")),
                Title = GetValueSafely(values, GetIndexSafely(headerMap, "Title")),
                Description = GetValueSafely(values, GetIndexSafely(headerMap, "Description")),
                CustomField1 = GetValueSafely(values, GetIndexSafely(headerMap, "CustomField1")),
                CustomField2 = GetValueSafely(values, GetIndexSafely(headerMap, "CustomField2")),
                CustomField3 = GetValueSafely(values, GetIndexSafely(headerMap, "CustomField3")),
            };
        }

        /// <summary>
        /// Retrieves the index of a header key from the map, returning -1 if the key is not found.
        /// </summary>
        /// <param name="map">The map of header names to indices.</param>
        /// <param name="key">The header key to find.</param>
        /// <returns>The index of the header key, or -1 if the key is not found.</returns>
        private static int GetIndexSafely(IReadOnlyDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var safely))
            {
                return safely;
            }

            return -1;
        }

        /// <summary>
        /// Retrieves the value from the values list at the specified index, returning an empty string if the index is out of range.
        /// </summary>
        /// <param name="values">The list of values from the CSV line.</param>
        /// <param name="index">The index of the value to retrieve.</param>
        /// <returns>The value at the specified index, or an empty string if the index is out of range.</returns>
        private static string GetValueSafely(IReadOnlyList<string> values, int index)
        {
            return index >= 0 && index < values.Count ? values[index].Clear() : "";
        }

        /// <summary>
        /// Gets a list of property names that have empty values in the specified <see cref="ImportedObject"/>.
        /// </summary>
        /// <param name="importedObject">The <see cref="ImportedObject"/> to check.</param>
        /// <returns>A list of property names with empty values.</returns>
        private static List<string> GetEmptyProperties(ImportedObject importedObject)
        {
            return importedObject.GetType().GetProperties()
                .Where(prop => string.IsNullOrEmpty(prop.GetValue(importedObject) as string))
                .Select(prop => prop.Name)
                .ToList();
        }
    }
}