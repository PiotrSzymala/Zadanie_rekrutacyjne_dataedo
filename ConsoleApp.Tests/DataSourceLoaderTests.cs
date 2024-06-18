using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Interfaces;
using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsoleApp.Tests
{
    public class DataSourceLoaderTests
    {
        private readonly DataSourceLoader _loader;
        private readonly Mock<IFileReader> _mockFileReader;
        private readonly Mock<ILogger<DataSourceLoader>> _mockLogger;

        public DataSourceLoaderTests()
        {
            _mockLogger = new Mock<ILogger<DataSourceLoader>>();
            _mockFileReader = new Mock<IFileReader>();
            _loader = new DataSourceLoader(_mockLogger.Object, _mockFileReader.Object);
        }

        [Fact]
        public void Load_HandlesMalformedData()
        {
            // Arrange
            const string dummyPath = "dummy_path";
            
            var lines = new[] {
                "Id;Type;Name;Schema;ParentId;ParentType;Title;Description;CustomField1;CustomField2;CustomField3",
                "1;DATABASE;MainDB" 
            };
            _mockFileReader.Setup(m => m.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _mockFileReader.Setup(m => m.Split(It.IsAny<string>(), ';')).Returns<string, char>((line, delimiter) => line.Split(delimiter));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _loader.Load(dummyPath));
            Assert.Contains("malformed data", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Load_HandlesFileReadException()
        {
            // Arrange
            const string dummyPath = "dummy_path";

            var expectedException = new IOException();
            _mockFileReader.Setup(m => m.ReadAllLines(It.IsAny<string>())).Throws(expectedException);

            // Act & Assert
            var exception = Assert.Throws<IOException>(() => _loader.Load(dummyPath));
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("There was an error processing the data")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void Load_HandlesEmptyFile()
        {
            // Arrange
            const string dummyPath = "empty_file.csv";
           
            var lines = Array.Empty<string>();
            
            _mockFileReader.Setup(m => m.ReadAllLines(It.IsAny<string>())).Returns(lines);

            // Act
            var result = _loader.Load(dummyPath);

            // Assert
            Assert.Empty(result);
        }
    }
}
