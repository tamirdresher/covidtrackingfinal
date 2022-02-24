using System;
using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using FluentAssertions;
using covidtracking.Dtos;
using covidtracking.Utilities;

namespace covidtracking.UnitTests
{
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientsDB> patientsDbModelStub = new();
        private readonly Mock<IPotentialPatientsDB> potentialPatientsDbModelStub = new();
        private readonly Mock<IIsolatedDB> isolatedDbModelStub = new();
        private readonly Mock<IPatientRoutesDB> patientRoutesDbModelStub = new();
        private readonly Mock<ILabTestsDB> labTestsDbModelStub = new();
        private readonly Mock<IPatientsEncountersDB> patientEncountersDbModelStub = new();
        private readonly Mock<IStatisticsDb> statisticsDbModelStub = new();
        private readonly Mock<IInfectedDB> infectedDbModelStub = new();

        [Fact]
        public async Task GetPatientAsync_WithUnexistingId_ReturnsNotFound()
        {
            // Arrange
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>())).
                ReturnsAsync((Patient)null);

            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = await controller.GetPatientAsync("222");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPatientAsync_WithExistingId_ReturnsOk()
        {
            // Arrange
            Patient patient = CreatePatient1(true);
            LabTests labTest = CreateLabTestsForPatient1(true);
            PatientInformationDto expected = patient.PatientAsPatientInformationDto(labTest);
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>())).
            ReturnsAsync(patient);
            labTestsDbModelStub.Setup(model => model.GetLabTests(It.IsAny<string>())).
            ReturnsAsync(labTest);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = ((OkObjectResult)(await controller.GetPatientAsync("000000000")).Result).Value;

            // Assert
            Assert.IsType<PatientInformationDto>(result);
            result.Should().BeEquivalentTo(
                expected,
                options => options.ComparingByMembers<PatientInformationDto>()
            );
        }

        [Fact]
        public async Task GetPatientsAsync_AnyInput_ReturnsOk()
        {
            // Arrange
            Patient patient1 = CreatePatient1(true);
            Patient patient2 = CreatePatient2(true);
            List<Patient> input = new List<Patient>();
            input.Add(patient1);
            input.Add(patient2);
            List<GetPatientDto> expected = new List<GetPatientDto>();
            expected.Add(patient1.PatientAsGetPatientDto());
            expected.Add(patient2.PatientAsGetPatientDto());
            patientsDbModelStub.Setup(model => model.GetPatientsAsync()).
                ReturnsAsync(input);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = ((OkObjectResult)(await controller.GetPatientsAsync()).Result).Value;

            // Assert
            Assert.IsType<List<GetPatientDto>>(result);
            result.Should().BeEquivalentTo(
                expected,
                options => options.ComparingByMembers<GetPatientDto>()
            );
        }

        [Fact]
        public async Task CreatePatientAsync_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            Patient patient1 = CreatePatient1(true);
            patient1.govtId = "invalid";
            patientsDbModelStub.Setup(model => model.CheckValidPatientInput(It.IsAny<Patient>()))
                .Returns(false);

            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.CreatePatientAsync(new CreatePatientDto())).Result;
            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task CreatePatientAsync_ValidInput_ReturnsOk()
        {
            // Arrange
            patientsDbModelStub.Setup(model => model.CheckValidPatientInput(It.IsAny<Patient>()))
                .Returns(true);
            var createPatientDto = CreateCreatePatientDto1(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.CreatePatientAsync(createPatientDto)).Result;
            var returnedValue = ((OkObjectResult)((await controller.CreatePatientAsync(createPatientDto)).Result)).Value;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(createPatientDto.govtId, returnedValue);
        }

        [Fact]
        public async Task AddVisitToPatient_InvalidPersonIdInput_ReturnsNotFound()
        {
            // Arrange
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
                .ReturnsAsync((Patient)null);
            var createPatientDto = CreateCreatePatientDto1(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.AddVisitToPatientAsync("123456789", new CreateVisitDto())).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddVisitToPatient_InvalidVisitInputValidDate_ReturnsBadRequest()
        {
            // Arrange
            CreateVisitDto visit = new CreateVisitDto()
            {
                dateOfVisit = DateTime.Now,
                siteName = "site",
                siteAddress = new()
                {
                    city = "city",
                    street = "street",
                    houseNumber = 1,
                    appartmentNumber = 1
                }
            };
            patientRoutesDbModelStub.Setup(model => model.CheckValidVisitInput(It.IsAny<Visit>()))
                .Returns(false);
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
                .ReturnsAsync(CreatePatient1(true));
            var createPatientDto = CreateCreatePatientDto1(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.AddVisitToPatientAsync("123456789", visit)).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddVisitToPatient_ValidVisitInputInvalidDate_ReturnsBadRequest()
        {
            // Arrange
            CreateVisitDto invalidVisit = new CreateVisitDto()
            {
                dateOfVisit = DateTime.Now.AddYears(-1),
                siteName = "site",
                siteAddress = new()
                {
                    city = "city",
                    street = "street",
                    houseNumber = 1,
                    appartmentNumber = 1
                }
            };

            patientRoutesDbModelStub.Setup(model => model.CheckValidVisitInput(It.IsAny<Visit>()))
                .Returns(true);
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
                .ReturnsAsync(CreatePatient1(true));
            var createPatientDto = CreateCreatePatientDto1(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.AddVisitToPatientAsync("123456789", invalidVisit)).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddVisitToPatient_ValidInput_ReturnsOk()
        {
            // Arrange
            CreateVisitDto visit = new CreateVisitDto()
            {
                dateOfVisit = DateTime.Now,
                siteName = "site",
                siteAddress = new()
                {
                    city = "city",
                    street = "street",
                    houseNumber = 1,
                    appartmentNumber = 1
                }
            };
            patientRoutesDbModelStub.Setup(model => model.CheckValidVisitInput(It.IsAny<Visit>()))
                .Returns(true);
            patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
                .ReturnsAsync(CreatePatient1(true));
            statisticsDbModelStub.Setup(model => model.AddCityToDb(It.IsAny<string>()))
                .Returns(true);
            var createPatientDto = CreateCreatePatientDto1(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.AddVisitToPatientAsync("123456789", visit)).Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task MakeInterviewAsync_InvalidPersonIdInput_ReturnsNotFound()
        {
            // Arrange
            potentialPatientsDbModelStub.Setup(model => model.GetPotentialPatientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((PotentialPatient)null);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.MakeInterviewAsync("123456789", CreateCreatePatientDto1(false))).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task MakeInterviewAsync_InvalidPatientCreationInput_ReturnsBadRequest()
        {
            // Arrange
            Mock<Adapters> adaptersMock = new();
            potentialPatientsDbModelStub.Setup(model => model.GetPotentialPatientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreatePotentialPatient1());
            patientsDbModelStub.Setup(model => model.CheckValidPatientInput(It.IsAny<Patient>()))
                .Returns(false);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.MakeInterviewAsync("123456789", CreateCreatePatientDto1(false))).Result;

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task MakeInterviewAsync_ValidInput_ReturnsOk()
        {
            // Arrange
            Mock<Adapters> adaptersMock = new();
            potentialPatientsDbModelStub.Setup(model => model.GetPotentialPatientByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(CreatePotentialPatient1());
            patientsDbModelStub.Setup(model => model.CheckValidPatientInput(It.IsAny<Patient>()))
                .Returns(true);
            var controller = new PatientsController(patientsDbModelStub.Object, potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                                                    patientRoutesDbModelStub.Object, labTestsDbModelStub.Object, patientEncountersDbModelStub.Object,
                                                    statisticsDbModelStub.Object, infectedDbModelStub.Object);

            // Act
            var result = (await controller.MakeInterviewAsync("123456789", CreateCreatePatientDto1(false))).Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        /*  Entity creation utilities  */
        private Patient CreatePatient1(bool issick)
        {
            return new Patient(
                "000000000", "TestFnameOne", "TestLnameOne", new DateTime(2000, 04, 04), "050-1234567",
                "TestMail1@gmail.com", "TestCityOne", "TestStreetOne", 1, 1, issick, 1, null
            );
        }

        private LabTests CreateLabTestsForPatient1(bool ispositive)
        {
            LabTests labTests1 = new LabTests("000000000");
            labTests1.AddTestResult(new LabTestResult("01", "01", "000000000", new DateTime(2022, 01, 12), ispositive));
            return labTests1;
        }

        private CreatePatientDto CreateCreatePatientDto1(bool isSick)
        {
            return new CreatePatientDto()
            {
                govtId = "111111111",
                firstName = "fname",
                lastName = "lname",
                birthDate = new DateTime(2022, 01, 12),
                phoneNumber = "phone",
                email = "email",
                address = new()
                {
                    city = "city",
                    street = "street",
                    houseNumber = 1,
                    appartmentNumber = 1
                },
                isCovidPositive = isSick,
                infectedByPatientID = "222222222"
            };
        }

        private Patient CreatePatient2(bool issick)
        {
            return new Patient(
                "000000001", "TestFnameTwo", "TestLnameTwo", new DateTime(2000, 04, 04), "051-1234567",
                "TestMail2@gmail.com", "TestCityTwo", "TestStreetTwo", 2, 2, issick, 1, "111111111"
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

        private PotentialPatient CreatePotentialPatient1()
        {
            return new PotentialPatient("Fname", "Lname", "050-6879422");
        }
    }
}