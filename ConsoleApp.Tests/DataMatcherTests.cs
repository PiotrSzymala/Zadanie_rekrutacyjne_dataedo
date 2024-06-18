using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ConsoleApp.Models;

namespace ConsoleApp.Tests
{
    public class DataMatcherTests
    {
        private readonly DataMatcher _dataMatcher;
        private readonly Mock<ILogger<DataMatcher>> _mockLogger = new();

        public DataMatcherTests()
        {
            _dataMatcher = new DataMatcher(_mockLogger.Object);
        }

        [Fact]
        public void MatchAndUpdate_Should_UpdateTypeWhenGlossaryEntry()
        {
            // Arrange
            var dataSource = new List<DataSourceObject>();
            var importedObjects = new List<ImportedObject>
        {
            new() { Type = "GLOSSARY_ENTRY", Name = "Entry1" }
        };

            // Act
            _dataMatcher.MatchAndUpdate(dataSource, importedObjects);

            // Assert
            Assert.Equal("TERM", importedObjects[0].Type);
        }

        [Fact]
        public void MatchAndUpdate_Should_NotUpdateWhenNoMatchFound()
        {
            // Arrange
            var dataSource = new List<DataSourceObject>
        {
            new() { Type = "TERM", Name = "Entry1", Schema = "Schema1" }
        };
            var importedObjects = new List<ImportedObject>
        {
            new() { Type = "TERM", Name = "Entry2", Schema = "Schema2" }
        };

            // Act
            _dataMatcher.MatchAndUpdate(dataSource, importedObjects);

            // Assert
            Assert.Null(dataSource[0].Title);
        }

        [Fact]
        public void UpdateMatch_Should_CorrectlyUpdateFields()
        {
            // Arrange
            var match = new DataSourceObject();
            var importedObject = new ImportedObject { Title = "Updated Title", Description = "Updated Description", CustomField1 = "CF1" };

            // Act
            DataMatcher.UpdateMatch(match, importedObject);

            // Assert
            Assert.Equal("Updated Title", match.Title);
            Assert.Equal("Updated Description", match.Description);
            Assert.Equal("CF1", match.CustomField1);
        }
    }
}
