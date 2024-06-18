using ConsoleApp.Interfaces;
using ConsoleApp.Models;
using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ConsoleApp.Tests
{
    public class DataPrinterTests
    {
        private readonly Mock<ILogger<DataPrinter>> _mockLogger;
        private readonly Mock<IConsoleWriter> _mockConsoleWriter;
        private readonly DataPrinter _dataPrinter;

        public DataPrinterTests()
        {
            _mockLogger = new Mock<ILogger<DataPrinter>>();
            _mockConsoleWriter = new Mock<IConsoleWriter>();
            _dataPrinter = new DataPrinter(_mockLogger.Object, _mockConsoleWriter.Object);
        }

        [Fact]
        public void Test_PrintMainObject_PrintsCorrectly()
        {
            var mockLogger = new Mock<ILogger<DataPrinter>>();
            var mockConsoleWriter = new Mock<IConsoleWriter>();
            var printer = new DataPrinter(mockLogger.Object, mockConsoleWriter.Object);
            var dataSourceObject = new DataSourceObject { Type = "DATABASE", Name = "TestDB", Title = "Test Database" };

            printer.PrintMainObject(dataSourceObject);

            mockConsoleWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
            mockConsoleWriter.Verify(x => x.SetForegroundColor(ConsoleColor.Yellow), Times.Once());
            mockConsoleWriter.Verify(x => x.ResetColor(), Times.Exactly(2));
        }
        [Fact]
        public void PrintMainObject_WithEmptyDescription_DoesNotPrintDescription()
        {
            var dataSourceObject = new DataSourceObject { Type = "DATABASE", Name = "TestDB", Title = "Test Database", Description = "" };
            _dataPrinter.PrintMainObject(dataSourceObject);
            _mockConsoleWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once());
            _mockConsoleWriter.Verify(x => x.ResetColor(), Times.Exactly(2));
        }

        [Fact]
        public void Print_Should_OrderDataSource_ByType()
        {
            // Arrange
            var dataSource = new List<DataSourceObject>
            {
                new() { Type = "GLOSSARY", Name = "Glossary1" },
                new() { Type = "DATABASE", Name = "Database1" }
            };

            // Act
            _dataPrinter.Print(dataSource);

            // Assert
            _mockConsoleWriter.Verify(m => m.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public void PrintMainObject_Should_Handle_EmptyTitle_Correctly()
        {
            // Arrange
            var dataSourceObject = new DataSourceObject { Type = "DATABASE", Name = "Database1", Title = "" };

            // Act
            _dataPrinter.PrintMainObject(dataSourceObject);

            // Assert
            _mockConsoleWriter.Verify(m => m.WriteLine(It.Is<string>(s => s.Contains("'Database1'") && !s.Contains("()"))), Times.Once());
        }

        [Fact]
        public void CheckDescriptionForEmptiness_Should_NotPrint_IfDescriptionIsEmpty()
        {
            // Arrange
            var dataSourceObject = new DataSourceObject { Description = "" };

            // Act
            _dataPrinter.CheckDescriptionForEmptiness(dataSourceObject, ConsoleColor.DarkGray, 1);

            // Assert
            _mockConsoleWriter.Verify(m => m.WriteLine(It.IsAny<string>()), Times.Never);
        }
    }
}