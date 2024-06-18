using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Tests
{
    public class DataImporterTests
    {
        private readonly Mock<ILogger<DataImporter>> _loggerMock;
        private readonly DataImporter _dataImporter;

        public DataImporterTests()
        {
            _loggerMock = new Mock<ILogger<DataImporter>>();
            _dataImporter = new DataImporter(_loggerMock.Object);
        }

        [Fact]
        public void Import_ValidFile_ReturnsImportedObjects()
        {
            // Arrange
            const string filePath = "test.csv";
            const string fileContent = "Type;Name;Schema;ParentName;ParentType;ParentSchema;Title;Description;CustomField1;CustomField2;CustomField3\n" +
                                       "type1;name1;schema1;parentName1;parentType1;parentSchema1;title1;description1;custom1;custom2;custom3\n" +
                                       "type2;name2;schema2;parentName2;parentType2;parentSchema2;title2;description2;custom4;custom5;custom6";

            File.WriteAllText(filePath, fileContent);

            // Act
            var result = _dataImporter.Import(filePath);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("type1", result[0].Type);
            Assert.Equal("name1", result[0].Name);
            Assert.Equal("schema1", result[0].Schema);
            Assert.Equal("parentName1", result[0].ParentName);
            Assert.Equal("parentType1", result[0].ParentType);
            Assert.Equal("parentSchema1", result[0].ParentSchema);
            Assert.Equal("title1", result[0].Title);
            Assert.Equal("description1", result[0].Description);
            Assert.Equal("custom1", result[0].CustomField1);
            Assert.Equal("custom2", result[0].CustomField2);
            Assert.Equal("custom3", result[0].CustomField3);

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public void Import_InvalidFile_ThrowsException()
        {
            // Arrange
            const string filePath = "nonexistent.csv";

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() => _dataImporter.Import(filePath));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("There was an error processing the data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Import_FileWithEmptyFields_LogsWarning()
        {
            // Arrange
            const string filePath = "test_with_empty_fields.csv";
            const string fileContent = "Type;Name;Schema;ParentName;ParentType;ParentSchema;Title;Description;CustomField1;CustomField2;CustomField3\n" +
                                       "type1;name1;schema1;;;parentSchema1;title1;;custom1;;custom3";

            File.WriteAllText(filePath, fileContent);

            // Act
            var result = _dataImporter.Import(filePath);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Empty fields found in line 1")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            // Clean up
            File.Delete(filePath);
        }
    }
}
