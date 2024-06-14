using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;

namespace ConsoleApp.Services
{
    internal class DataImporter : IDataImporter
    {
        public IList<ImportedObject> Import(string filePath)
        {
           var importedObjects = new List<ImportedObject>();
           using (var streamReader = new StreamReader(filePath))
           {
               string line;

               while ((line = streamReader.ReadLine()) != null )
               {
                   importedObjects.Add(ParseLine(line));
               }
           }

           return importedObjects;
        }

        private ImportedObject ParseLine(string line)
        {
            var values = line.Split(';');
            return new ImportedObject
            {
                Type = GetValueSafely(values, 0),
                Name = GetValueSafely(values, 1),
                Schema = GetValueSafely(values, 2),
                ParentName = GetValueSafely(values, 3),
                ParentType = GetValueSafely(values, 4),
                ParentSchema = GetValueSafely(values, 5),
                Title = GetValueSafely(values, 6),
                Description = GetValueSafely(values, 7),
                CustomField1 = GetValueSafely(values, 8),
                CustomField2 = GetValueSafely(values, 9),
                CustomField3 = GetValueSafely(values, 10)
            };
        }

        private string GetValueSafely(string[] values, int index)
        {
            return index < values.Length ? values[index] : null;
        }
    }
}
