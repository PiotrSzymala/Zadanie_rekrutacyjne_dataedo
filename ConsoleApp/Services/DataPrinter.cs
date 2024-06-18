using System;
using System.Collections.Generic;
using System.Linq;
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
                    break;
            }
        }

        private  void PrintMainObject(DataSourceObject dataSourceObject)
        {
            PrintWithColor($"{dataSourceObject.Type} '{dataSourceObject.Name} ({dataSourceObject.Title})'", ConsoleColor.Yellow,0);
            CheckDescriptionForEmptiness(dataSourceObject, ConsoleColor.DarkYellow, 0);
        }

        private  void PrintChildren(DataSourceObject parent, IList<DataSourceObject> dataSource)
        {
            var childrenGroups = dataSource
                .Where(x => x.ParentId == parent.Id && x.ParentType == parent.Type)
                .GroupBy(x => x.Type);

            foreach (var childrenGroup in childrenGroups)
            {
                PrintWithColor($"{childrenGroup.Key}S ({childrenGroup.Count()}):", ConsoleColor.DarkCyan, 1);
                foreach (var child in childrenGroup.OrderBy(x => x.Name))
                {
                    PrintSubChildren(child, dataSource);
                }
            }
        }

        private  void PrintSubChildren(DataSourceObject child, IList<DataSourceObject> dataSource)
        {
            var subChildrenGroups = dataSource
                .Where(x => x.ParentId == child.Id && x.ParentType == child.Type)
                .GroupBy(x => x.Type);

            PrintWithColor($"{child.Schema}.{child.Name} ({child.Title})",ConsoleColor.White,2);
            CheckDescriptionForEmptiness(child, ConsoleColor.DarkGray, 2);

            foreach (var subChildrenGroup in subChildrenGroups)
            {
                PrintWithColor($"{subChildrenGroup.Key}S ({subChildrenGroup.Count()}):", ConsoleColor.DarkCyan, 3);

                foreach (var subChild in subChildrenGroup.OrderBy(x => x.Name))
                {
                    PrintWithColor($"{subChild.Name} ({subChild.Title})", ConsoleColor.White, 4);
                    CheckDescriptionForEmptiness(subChild, ConsoleColor.DarkGray, 4);
                }
            }
        }

        private  void CheckDescriptionForEmptiness(DataSourceObject dataSourceObject, ConsoleColor color, int indentationLevel)
        {
            if (!string.IsNullOrEmpty(dataSourceObject.Description))
                PrintWithColor(dataSourceObject.Description, color,indentationLevel);

            _consoleWriter.ResetColor();
        }

        private  void PrintWithColor(string message, ConsoleColor color, int indentationLevel)
        {
            var indentation = new string('\t', indentationLevel);
            
            _consoleWriter.SetForegroundColor(color);
            _consoleWriter.WriteLine($"{indentation}{message}");
            _consoleWriter.ResetColor();
        }
    }
}
