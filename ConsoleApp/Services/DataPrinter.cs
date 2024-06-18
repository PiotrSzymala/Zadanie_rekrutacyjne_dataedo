using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.Logging;
using NLog;

namespace ConsoleApp.Services
{
    public class DataPrinter : IDataPrinter
    {
        private readonly ILogger<DataPrinter> _logger;
        private readonly IConsoleWriter _consoleWriter;
        public DataPrinter(ILogger<DataPrinter> logger, IConsoleWriter consoleWriter)
        {
            _logger = logger;
            _consoleWriter = consoleWriter;
        }

        public void Print(IList<DataSourceObject> dataSource)
        {
            try
            {
                foreach (var dataSourceObject in dataSource.OrderBy(x => x.Type))
                {
                    PrintDataSourceObject(dataSourceObject, dataSource);
                }

                Console.ReadKey();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }

        private void PrintDataSourceObject(DataSourceObject dataSourceObject, IList<DataSourceObject> dataSource)
        {
            switch (dataSourceObject.Type)
            {
                case "DATABASE":
                case "GLOSSARY":
                case "DOMAIN":
                    PrintMainObject(dataSourceObject);
                    PrintChildren(dataSourceObject, dataSource);
                    Console.WriteLine();
                    break;
            }
        }

        private void PrintMainObject(DataSourceObject dataSourceObject)
        {
            var displayText = string.IsNullOrEmpty(dataSourceObject.Title)
                ? $"{dataSourceObject.Type} '{dataSourceObject.Name}'"
                : $"{dataSourceObject.Type} '{dataSourceObject.Name} ({dataSourceObject.Title})'";

            PrintWithColor(displayText, ConsoleColor.Yellow, 0);
            CheckDescriptionForEmptiness(dataSourceObject, ConsoleColor.DarkYellow, 0);
        }

        private void PrintChildren(DataSourceObject parent, IList<DataSourceObject> dataSource)
        {
            var childrenGroups = GetChildrenGroups(parent, dataSource);

            foreach (var childrenGroup in childrenGroups)
            {
                PrintWithColor($"{childrenGroup.Key}S ({childrenGroup.Count()}):", ConsoleColor.DarkCyan, 1);
                foreach (var child in childrenGroup.OrderBy(x => x.Name))
                {
                    PrintSubChildren(child, dataSource);
                }
            }
        }

        private static IEnumerable<IGrouping<string, DataSourceObject>> GetChildrenGroups(DataSourceObject parent, IList<DataSourceObject> dataSource)
        {
            return dataSource
                .Where(x => x.ParentId == parent.Id && x.ParentType == parent.Type)
                .GroupBy(x => x.Type);
        }

        private void PrintSubChildren(DataSourceObject child, IList<DataSourceObject> dataSource)
        {
            var subChildrenGroups = GetSubChildrenGroups(child, dataSource);

            PrintNodeWithDescription(BuildChildNode(child).ToString(), 2, child);

            foreach (var subChildrenGroup in subChildrenGroups)
            {
                PrintWithColor($"{subChildrenGroup.Key}S ({subChildrenGroup.Count()}):", ConsoleColor.DarkCyan, 3);

                foreach (var subChild in subChildrenGroup.OrderBy(x => x.Name))
                {
                    PrintNodeWithDescription(BuildDisplayText(subChild), 4, subChild);
                }
            }
        }

        private IEnumerable<IGrouping<string, DataSourceObject>> GetSubChildrenGroups(DataSourceObject child, IList<DataSourceObject> dataSource)
        {
            return dataSource
                .Where(x => x.ParentId == child.Id && x.ParentType == child.Type)
                .GroupBy(x => x.Type);
        }

        private StringBuilder BuildChildNode(DataSourceObject child)
        {
            var childNodeStringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(child.Schema))
                childNodeStringBuilder.Append($"{child.Schema}.");

            childNodeStringBuilder.Append(child.Name);

            if (!string.IsNullOrEmpty(child.Title))
                childNodeStringBuilder.Append($" ({child.Title})");

            return childNodeStringBuilder;
        }

        private void PrintNodeWithDescription(string displayText, int level, DataSourceObject obj)
        {
            PrintWithColor(displayText, ConsoleColor.Gray, level);
            CheckDescriptionForEmptiness(obj, ConsoleColor.DarkGray, level);
        }

        private string BuildDisplayText(DataSourceObject obj)
        {
            return string.IsNullOrEmpty(obj.Title) ? obj.Name : $"{obj.Name} ({obj.Title})";
        }

        private void PrintWithColor(string message, ConsoleColor color, int indentationLevel)
        {
            var indentation = new string('\t', indentationLevel);

            _consoleWriter.SetForegroundColor(color);
            _consoleWriter.WriteLine($"{indentation}{message}");
            _consoleWriter.ResetColor();
        }

        private void CheckDescriptionForEmptiness(DataSourceObject dataSourceObject, ConsoleColor color, int indentationLevel)
        {
            if (!string.IsNullOrEmpty(dataSourceObject.Description))
                PrintWithColor(dataSourceObject.Description, color, indentationLevel);

            _consoleWriter.ResetColor();
        }
    }
}
