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
    public class DataMatcher : IDataMatcher
    {
        private readonly ILogger<DataMatcher> _logger;
        
        private const string GlossaryEntry = "GLOSSARY_ENTRY";
        private const string Term = "TERM";

        public DataMatcher(ILogger<DataMatcher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Matches and updates data source objects with imported objects.
        /// </summary>
        /// <param name="dataSource">The data source objects to be updated.</param>
        /// <param name="importedObjects">The imported objects to match and update from.</param>
        /// <exception cref="Exception">Thrown when there is an error processing the data.</exception>
        public void MatchAndUpdate(IList<DataSourceObject> dataSource, IList<ImportedObject> importedObjects)
        {
            try
            {
                foreach (var importedObject in importedObjects)
                {
                    if (importedObject.Type == GlossaryEntry)
                        importedObject.Type = Term;

                    MatchAndProcess(importedObject, dataSource);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }

        /// <summary>
        /// Matches and processes the imported object against the data source.
        /// </summary>
        /// <param name="importedObject">The imported object to match and process.</param>
        /// <param name="dataSource">The data source objects to match against.</param>
        private void MatchAndProcess(ImportedObject importedObject, IList<DataSourceObject> dataSource)
        {
            var match = dataSource.FirstOrDefault(x =>
                x.Type == importedObject.Type &&
                x.Name == importedObject.Name &&
                x.Schema == importedObject.Schema);

            if (match == null)
                return;

            if (!IsValidParent(importedObject, match, dataSource))
                return;

            UpdateMatch(match, importedObject);
        }

        /// <summary>
        /// Checks if the parent of the imported object is valid.
        /// </summary>
        /// <param name="importedObject">The imported object being checked.</param>
        /// <param name="match">The matched data source object.</param>
        /// <param name="dataSource">The data source objects to check against.</param>
        /// <returns><c>true</c> if the parent is valid; otherwise, <c>false</c>.</returns>
        private bool IsValidParent(ImportedObject importedObject, DataSourceObject match, IList<DataSourceObject> dataSource)
        {
            if (match.ParentId <= 0 || string.IsNullOrEmpty(importedObject.ParentType))
                return true;

            var parent = dataSource.FirstOrDefault(x =>
                x.Id == match.ParentId &&
                x.Type == match.ParentType);

            var isValidParent = parent != null &&
                               parent.Name == importedObject.ParentName &&
                               parent.Schema == importedObject.ParentSchema &&
                               parent.Type == importedObject.ParentType;

            return isValidParent;
        }

        /// <summary>
        /// Updates the matched data source object with values from the imported object.
        /// </summary>
        /// <param name="match">The matched data source object to update.</param>
        /// <param name="importedObject">The imported object containing the new values.</param>
        public static void UpdateMatch(DataSourceObject match, ImportedObject importedObject)
        {
            match.Title = importedObject.Title;
            match.Description = importedObject.Description;
            match.CustomField1 = importedObject.CustomField1;
            match.CustomField2 = importedObject.CustomField2;
            match.CustomField3 = importedObject.CustomField3;
        }
    }
}
