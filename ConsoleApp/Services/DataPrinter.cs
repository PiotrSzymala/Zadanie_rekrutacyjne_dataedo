using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApp.Interfaces;
using ConsoleApp.Models;
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

        /// <summary>
        /// Prints the data source objects to the console.
        /// </summary>
        /// <param name="dataSource">The list of data source objects to print.</param>
        public void Print(IList<DataSourceObject> dataSource)
        {
            try
            {
                foreach (var dataSourceObject in dataSource.OrderBy(x => x.Type))
                {
                    PrintDataSourceObject(dataSourceObject, dataSource);
                }

                _consoleWriter.ReadKey();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error processing the data: ");
                throw;
            }
        }

        /// <summary>
        /// Prints a data source object and its children to the console.
        /// </summary>
        /// <param name="dataSourceObject">The data source object to print.</param>
        /// <param name="dataSource">The list of all data source objects.</param>
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

        /// <summary>
        /// Prints the main object to the console.
        /// </summary>
        /// <param name="dataSourceObject">The data source object to print.</param>
        public void PrintMainObject(DataSourceObject dataSourceObject)
        {
            var displayText = string.IsNullOrEmpty(dataSourceObject.Title)
                ? $"{dataSourceObject.Type} '{dataSourceObject.Name}'"
                : $"{dataSourceObject.Type} '{dataSourceObject.Name} ({dataSourceObject.Title})'";

            PrintWithColor(displayText, ConsoleColor.Yellow, 0);
            CheckDescriptionForEmptiness(dataSourceObject, ConsoleColor.DarkYellow, 0);
        }

        /// <summary>
        /// Prints the children of a parent object to the console.
        /// </summary>
        /// <param name="parent">The parent object whose children to print.</param>
        /// <param name="dataSource">The list of all data source objects.</param>
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

        /// <summary>
        /// Groups the children of a parent object by type.
        /// </summary>
        /// <param name="parent">The parent object whose children to group.</param>
        /// <param name="dataSource">The list of all data source objects.</param>
        /// <returns>A collection of grouped children.</returns>
        private static IEnumerable<IGrouping<string, DataSourceObject>> GetChildrenGroups(DataSourceObject parent, IList<DataSourceObject> dataSource)
        {
            return dataSource
                .Where(x => x.ParentId == parent.Id && x.ParentType == parent.Type)
                .GroupBy(x => x.Type);
        }

        /// <summary>
        /// Prints the sub-children of a child object to the console.
        /// </summary>
        /// <param name="child">The child object whose sub-children to print.</param>
        /// <param name="dataSource">The list of all data source objects.</param>
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

        /// <summary>
        /// Groups the sub-children of a child object by type.
        /// </summary>
        /// <param name="child">The child object whose sub-children to group.</param>
        /// <param name="dataSource">The list of all data source objects.</param>
        /// <returns>A collection of grouped sub-children.</returns>
        private IEnumerable<IGrouping<string, DataSourceObject>> GetSubChildrenGroups(DataSourceObject child, IList<DataSourceObject> dataSource)
        {
            return dataSource
                .Where(x => x.ParentId == child.Id && x.ParentType == child.Type)
                .GroupBy(x => x.Type);
        }

        /// <summary>
        /// Builds the display text for a child node.
        /// </summary>
        /// <param name="child">The child object to build the display text for.</param>
        /// <returns>A <see cref="StringBuilder"/> containing the display text.</returns>
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

        /// <summary>
        /// Prints a node with its description to the console.
        /// </summary>
        /// <param name="displayText">The display text of the node.</param>
        /// <param name="level">The indentation level of the node.</param>
        /// <param name="obj">The data source object representing the node.</param>
        private void PrintNodeWithDescription(string displayText, int level, DataSourceObject obj)
        {
            PrintWithColor(displayText, ConsoleColor.Gray, level);
            CheckDescriptionForEmptiness(obj, ConsoleColor.DarkGray, level);
        }

        /// <summary>
        /// Builds the display text for a data source object.
        /// </summary>
        /// <param name="obj">The data source object to build the display text for.</param>
        /// <returns>The display text.</returns>
        private string BuildDisplayText(DataSourceObject obj)
        {
            return string.IsNullOrEmpty(obj.Title) ? obj.Name : $"{obj.Name} ({obj.Title})";
        }

        /// <summary>
        /// Prints a message with a specified color and indentation level to the console.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="color">The color to use for the message.</param>
        /// <param name="indentationLevel">The indentation level of the message.</param>
        private void PrintWithColor(string message, ConsoleColor color, int indentationLevel)
        {
            var indentation = new string('\t', indentationLevel);

            _consoleWriter.SetForegroundColor(color);
            _consoleWriter.WriteLine($"{indentation}{message}");
            _consoleWriter.ResetColor();
        }

        /// <summary>
        /// Checks if the description of a data source object is empty and prints it with a specified color and indentation level to the console.
        /// </summary>
        /// <param name="dataSourceObject">The data source object to check.</param>
        /// <param name="color">The color to use for the description.</param>
        /// <param name="indentationLevel">The indentation level of the description.</param>
        public void CheckDescriptionForEmptiness(DataSourceObject dataSourceObject, ConsoleColor color, int indentationLevel)
        {
            if (!string.IsNullOrEmpty(dataSourceObject.Description))
                PrintWithColor(dataSourceObject.Description, color, indentationLevel);

            _consoleWriter.ResetColor();
        }
    }
}
