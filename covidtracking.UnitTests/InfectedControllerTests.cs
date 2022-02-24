using System;
using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using covidtracking.Dtos;

namespace covidtracking.UnitTests
{
    public class InfectedControllerTests
    {
        private readonly Mock<IPatientsDB> patientsDbModelStub = new();
        private readonly Mock<IInfectedDB> infectedDbModelStub = new();

        [Fact]
        public async Task GetPatientsSince_InvalidDateFormat_ReturnsBadRequest()
        {
            // Arrange
            var controller = new InfectedController(infectedDbModelStub.Object, patientsDbModelStub.Object);

            // Act
            var result = (await controller.GetPatientsSince("123a")).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetPatientsSince_InvalidDateFormat_ReturnsNotFound()
        {
            // Arrange
            infectedDbModelStub.Setup(model => model.GetInfectedSince(It.IsAny<DateTime>()))
                .Returns(new HashSet<string>());
            patientsDbModelStub.Setup(model => model.GetPatientsWithIdsAsync(It.IsAny<HashSet<string>>()))
                .ReturnsAsync(new List<Patient>());
            var controller = new InfectedController(infectedDbModelStub.Object, patientsDbModelStub.Object);

            // Act
            var result = (await controller.GetPatientsSince("2000, 04, 04")).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPatientsSince_ValidDateInputExistingInfectionsSeince_ReturnsOk()
        {
            // Arrange
            List<Patient> expected = new List<Patient>();
            expected.Add(CreatePatient1(true));
            infectedDbModelStub.Setup(model => model.GetInfectedSince(It.IsAny<DateTime>()))
                .Returns(new HashSet<string>());
            patientsDbModelStub.Setup(model => model.GetPatientsWithIdsAsync(It.IsAny<HashSet<string>>()))
                .ReturnsAsync(expected);
            var controller = new InfectedController(infectedDbModelStub.Object, patientsDbModelStub.Object);

            // Act
            var result = (await controller.GetPatientsSince("2000, 04, 04")).Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        /*  Entity creation utilities  */
        private Patient CreatePatient1(bool issick)
        {
            return new Patient(
                "000000000", "TestFnameOne", "TestLnameOne", new DateTime(2000, 04, 04), "050-1234567",
                "TestMail1@gmail.com", "TestCityOne", "TestStreetOne", 1, 1, issick, 2, null
            );
        }

        private CreateVisitDto CreateVisitDto1()
        {
            return new CreateVisitDto
            {
                dateOfVisit = new DateTime(2022, 02, 02),
                siteName = "Visit Site",
                siteAddress = new()
                {
                    city = "city",
                    street = "street",
                    houseNumber = 1,
                    appartmentNumber = 1
                }
            };
        }

    }
}


