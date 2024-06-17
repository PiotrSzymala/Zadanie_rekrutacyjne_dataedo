using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;

namespace ConsoleApp.Services
{
    public class DataImporter : IDataImporter
    {
        public IList<ImportedObject> Import(string filePath)
        {
            var importedObjects = new List<ImportedObject>();
            using (var streamReader = new StreamReader(filePath))
            {

                string headerLine = streamReader.ReadLine();

                var headerMap = ParseHeaders(headerLine);

                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    var values = line.Split(';');
                    var importedObject = ParseLine(values, headerMap);
                    importedObjects.Add(importedObject);
                }
            }

            return importedObjects;
        }

        private static Dictionary<string, int> ParseHeaders(string headerLine)
        {
            return headerLine.Split(';')
                .Select((header, index) => new { Header = header, Index = index })
                .ToDictionary(h => h.Header, h => h.Index);
        }

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

        private static int GetIndexSafely(IReadOnlyDictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out var safely))
            {
                return safely;
            }

            return -1;
        }

        private static string GetValueSafely(IReadOnlyList<string> values, int index)
        {
            return index >= 0 && index < values.Count ? values[index].Clear() : "";
        }
    }
}