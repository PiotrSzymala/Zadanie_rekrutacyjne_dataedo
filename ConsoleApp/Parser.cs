using ConsoleApp.Interfaces;

namespace ConsoleApp
{
    using Microsoft.VisualBasic.FileIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Parser
    {
        private readonly IDataImporter _dataImporter;
        private readonly IDataSourceLoader _dataSourceLoader;
        private readonly IDataMatcher _dataMatcher;
        private readonly IDataPrinter _dataPrinter;

        public Parser(IDataImporter dataImporter, IDataSourceLoader dataSourceLoader, IDataMatcher dataMatcher, IDataPrinter dataPrinter)
        {
            _dataImporter = dataImporter;
            _dataSourceLoader = dataSourceLoader;
            _dataMatcher = dataMatcher;
            _dataPrinter = dataPrinter;
        }

        public void Do(string fileToImport, string dataSource)
        {
            try
            {
                var importedData = _dataImporter.Import(fileToImport);
                var sourceData = _dataSourceLoader.Load(dataSource);
                _dataMatcher.MatchAndUpdate(sourceData, importedData);
                _dataPrinter.Print(sourceData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
