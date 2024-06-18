using ConsoleApp.Interfaces;

namespace ConsoleApp
{
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualBasic.FileIO;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Parser
    {
        private readonly ILogger<Parser> _logger;

        private readonly IDataImporter _dataImporter;
        private readonly IDataSourceLoader _dataSourceLoader;
        private readonly IDataMatcher _dataMatcher;
        private readonly IDataPrinter _dataPrinter;

        public Parser(ILogger<Parser> logger, IDataImporter dataImporter, IDataSourceLoader dataSourceLoader, IDataMatcher dataMatcher, IDataPrinter dataPrinter)
        {
            _logger = logger;
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
                _logger.LogError(e, $"Error occurred while processing {fileToImport} and {dataSource}");
            }
        }
    }
}
