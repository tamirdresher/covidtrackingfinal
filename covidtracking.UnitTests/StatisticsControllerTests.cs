using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;


namespace covidtracking.UnitTests
{
    public class StatisticsControllerTests
    {
        private readonly Mock<IStatisticsDb> statisticsDbModelStub = new();

        [Fact]
        public void GetCurrentStatistics_UninitializedStatistics_ReturnsOkNewStatistics()
        {
            // Arrange
            statisticsDbModelStub.Setup(model => model.GetCurrentStatistics()).Returns((Statistics)null);

            var controller = new StatisticsController(statisticsDbModelStub.Object);
            // Act
            var result = controller.GetCurrentStatistics().Result;
            var statisticResult = ((OkObjectResult)controller.GetCurrentStatistics().Result).Value;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            statisticResult.Should().BeEquivalentTo(
                new Statistics(),
                options => options.ComparingByMembers<Statistics>()
            );
        }

        [Fact]
        public void GetCurrentStatistics_IninitializedStatistics_ReturnsOkExpectedStatistics()
        {
            // Arrange
            Statistics expected = new Statistics();
            expected.infected = 1;
            statisticsDbModelStub.Setup(model => model.GetCurrentStatistics()).Returns(expected);

            var controller = new StatisticsController(statisticsDbModelStub.Object);
            // Act
            var result = controller.GetCurrentStatistics().Result;
            var statisticResult = ((OkObjectResult)controller.GetCurrentStatistics().Result).Value;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            statisticResult.Should().BeEquivalentTo(
                expected,
                options => options.ComparingByMembers<Statistics>()
            );
        }
    }
}