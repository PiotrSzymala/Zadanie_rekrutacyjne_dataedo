using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.Interfaces;

namespace ConsoleApp.Services
{
    internal class DataPrinter : IDataPrinter
    {
        public void Print(IList<DataSourceObject> dataSource)
        {
            foreach (var dataSourceObject in dataSource.OrderBy(x => x.Type))
            {
                PrintDataSourceObject(dataSourceObject, dataSource);
            }
            Console.ReadKey();
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

        private void PrintMainObject(DataSourceObject dataSourceObject)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{dataSourceObject.Type} '{dataSourceObject.Name} ({dataSourceObject.Title})'");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(dataSourceObject.Description);
            Console.ResetColor();
        }

        private void PrintChildren(DataSourceObject parent, IList<DataSourceObject> dataSource)
        {
            var childrenGroups = dataSource
                .Where(x => x.ParentId == parent.Id && x.ParentType == parent.Type)
                .GroupBy(x => x.Type);

            foreach (var childrenGroup in childrenGroups)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"\t{childrenGroup.Key}S ({childrenGroup.Count()}):");
                Console.ResetColor();

                foreach (var child in childrenGroup.OrderBy(x => x.Name))
                {
                    PrintSubChildren(child, dataSource);
                }
            }
        }

        private void PrintSubChildren(DataSourceObject child, IList<DataSourceObject> dataSource)
        {
            var subChildrenGroups = dataSource
                .Where(x => x.ParentId == child.Id && x.ParentType == child.Type)
                .GroupBy(x => x.Type);

            Console.WriteLine($"\t\t{child.Schema}.{child.Name} ({child.Title})");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\t\t{child.Description}");
            Console.ResetColor();

            foreach (var subChildrenGroup in subChildrenGroups)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"\t\t\t{subChildrenGroup.Key}S ({subChildrenGroup.Count()}):");
                Console.ResetColor();

                foreach (var subChild in subChildrenGroup.OrderBy(x => x.Name))
                {
                    Console.WriteLine($"\t\t\t\t{subChild.Name} ({subChild.Title})");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"\t\t\t\t{subChild.Description}");
                    Console.ResetColor();
                }
            }
        }
    }
}
