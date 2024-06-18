using ConsoleApp.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Models;

namespace ConsoleApp.Tests
{
    public class ParserTests
    {
        private readonly Mock<ILogger<Parser>> _loggerMock;
        private readonly Mock<IDataImporter> _dataImporterMock;
        private readonly Mock<IDataSourceLoader> _dataSourceLoaderMock;
        private readonly Mock<IDataMatcher> _dataMatcherMock;
        private readonly Mock<IDataPrinter> _dataPrinterMock;
        private readonly Parser _parser;

        public ParserTests()
        {
            _loggerMock = new Mock<ILogger<Parser>>();
            _dataImporterMock = new Mock<IDataImporter>();
            _dataSourceLoaderMock = new Mock<IDataSourceLoader>();
            _dataMatcherMock = new Mock<IDataMatcher>();
            _dataPrinterMock = new Mock<IDataPrinter>();

            _parser = new Parser(
                _loggerMock.Object,
                _dataImporterMock.Object,
                _dataSourceLoaderMock.Object,
                _dataMatcherMock.Object,
                _dataPrinterMock.Object);
        }

        [Fact]
        public void Do_ValidFiles_ProcessesCorrectly()
        {
            // Arrange
            var fileToImport = "import.csv";
            var dataSource = "source.csv";

            var importedData = new List<ImportedObject>
            {
                new() { Type = "type1", Name = "name1" },
                new() { Type = "type2", Name = "name2" }
            };

            var sourceData = new List<DataSourceObject>
            {
                new() { Type = "type1", Name = "name1" },
                new() { Type = "type3", Name = "name3" }
            };

            _dataImporterMock.Setup(x => x.Import(fileToImport)).Returns(importedData);
            _dataSourceLoaderMock.Setup(x => x.Load(dataSource)).Returns(sourceData);

            // Act
            _parser.Do(fileToImport, dataSource);

            // Assert
            _dataImporterMock.Verify(x => x.Import(fileToImport), Times.Once);
            _dataSourceLoaderMock.Verify(x => x.Load(dataSource), Times.Once);
            _dataMatcherMock.Verify(x => x.MatchAndUpdate(sourceData, importedData), Times.Once);
            _dataPrinterMock.Verify(x => x.Print(sourceData), Times.Once);
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Do_ExceptionThrown_LogsError()
        {
            // Arrange
            var fileToImport = "import.csv";
            var dataSource = "source.csv";
            var exception = new Exception("Test exception");

            _dataImporterMock.Setup(x => x.Import(fileToImport)).Throws(exception);

            // Act
            _parser.Do(fileToImport, dataSource);

            // Assert
            _dataImporterMock.Verify(x => x.Import(fileToImport), Times.Once);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error occurred while processing {fileToImport} and {dataSource}")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
