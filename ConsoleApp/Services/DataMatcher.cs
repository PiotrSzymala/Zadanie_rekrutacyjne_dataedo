using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.Logging;
using NLog;

namespace ConsoleApp.Services
{
    public class DataMatcher : IDataMatcher
    {
        private readonly ILogger<DataMatcher> _logger;

        public DataMatcher(ILogger<DataMatcher> logger)
        {
            _logger = logger;
        }

        public void MatchAndUpdate(IList<DataSourceObject> dataSource, IList<ImportedObject> importedObjects)
        {
            try
            {
                foreach (var importedObject in importedObjects.Where(x => x.Type == "GLOSSARY_ENTRY"))
                {
                    importedObject.Type = "TERM";
                }

                foreach (var importedObject in importedObjects.Where(x => x.Type == "TERM"))
                {
                    var match = dataSource.FirstOrDefault(x =>
                        x.Type == importedObject.Type &&
                        x.Name == importedObject.Name &&
                        x.Schema == importedObject.Schema);

                    if (match is null)
                        continue;

                    if (match.ParentId > 0 && !string.IsNullOrEmpty(importedObject.ParentType))
                    {
                        var parent = dataSource.FirstOrDefault(x =>
                            x.Id == match.ParentId &&
                            x.Type == match.ParentType);

                        if (parent?.Name != importedObject.ParentName
                            || parent?.Schema != importedObject.ParentSchema
                            || parent?.Type != importedObject.ParentType)
                        {
                            continue;
                        }
                    }

                    UpdateMatch(match, importedObject);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }
        private static void UpdateMatch(DataSourceObject match, ImportedObject importedObject)
        {
            match.Title = importedObject.Title;
            match.Description = importedObject.Description;
            match.CustomField1 = importedObject.CustomField1;
            match.CustomField2 = importedObject.CustomField2;
            match.CustomField3 = importedObject.CustomField3;
        }
    }
}
