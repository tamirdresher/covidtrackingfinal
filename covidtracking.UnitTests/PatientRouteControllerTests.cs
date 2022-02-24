using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace covidtracking.UnitTests
{
    public class PatientRouteControllerTests
    {
        private readonly Mock<IPatientsDB> patientsDbModelStub = new();
        private readonly Mock<IIsolatedDB> isolatedDbModelStub = new();
        private readonly Mock<IPatientRoutesDB> patientRoutesDbModelStub = new();

        [Fact]
        public async Task GetPatientRouteAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            patientRoutesDbModelStub.Setup(model => model.GetPatientRouteByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((PatientRoute)null);

            var controller = new PatientRouteController(patientRoutesDbModelStub.Object, patientsDbModelStub.Object,
                                                        isolatedDbModelStub.Object);

            // Act
            var result = (await controller.GetPatientRouteAsync("0abcd")).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPatientRouteAsync_ValidExistingId_ReturnsOk()
        {
            // Arrange
            string id = "111111111";
            patientRoutesDbModelStub.Setup(model => model.GetPatientRouteByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new PatientRoute(id));

            var controller = new PatientRouteController(patientRoutesDbModelStub.Object, patientsDbModelStub.Object,
                                                        isolatedDbModelStub.Object);

            // Act
            var result = (await controller.GetPatientRouteAsync(id)).Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

    }
}