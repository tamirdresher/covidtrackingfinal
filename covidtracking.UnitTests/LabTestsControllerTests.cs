using System;
using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using covidtracking.Dtos;

namespace covidtracking.UnitTests{
public class LabTestsControllerTests
{   
    private readonly Mock<IPatientsDB> patientsDbModelStub = new();
    private readonly Mock<IPotentialPatientsDB> potentialPatientsDbModelStub = new();
    private readonly Mock<IIsolatedDB> isolatedDbModelStub = new();
    private readonly Mock<ILabTestsDB> labTestsDbModelStub = new();
    private readonly Mock<IStatisticsDb> statisticsDbModelStub = new();
    private readonly Mock<IInfectedDB> infectedDbModelStub = new();

    [Fact]
    public async Task CreateTestAsync_InvalidTestPatientId_ReturnsBadRequestAndMessage(){
        // Arrange
        patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
            .ReturnsAsync((Patient) null);
        var controller = new LabTestsController(labTestsDbModelStub.Object, potentialPatientsDbModelStub.Object,
                                                patientsDbModelStub.Object, isolatedDbModelStub.Object, 
                                                statisticsDbModelStub.Object, infectedDbModelStub.Object);

        // Act
        var result = (await controller.CreateTestAsync(CreateLabTestDto1(true))).Result;
        var badRequestResult = ((BadRequestObjectResult) (await controller.CreateTestAsync(CreateLabTestDto1(true))).Result);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.True("Id doesn't exist in our database" == badRequestResult.Value);
    }

    [Fact]
    public async Task CreateTestAsync_ValidLabTestInput_ReturnsOk(){
        // Arrange
        patientsDbModelStub.Setup(model => model.GetPatientAsync(It.IsAny<string>()))
            .ReturnsAsync(CreatePatient1(true));
        var controller = new LabTestsController(labTestsDbModelStub.Object, potentialPatientsDbModelStub.Object,
                                                patientsDbModelStub.Object, isolatedDbModelStub.Object, 
                                                statisticsDbModelStub.Object, infectedDbModelStub.Object);

        // Act
        var result = (await controller.CreateTestAsync(CreateLabTestDto1(true))).Result;

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    private CreateLabTestDto CreateLabTestDto1(bool issick){
        return new CreateLabTestDto{
            labId = "00",
            testId = "01",
            patientId = "111111111",
            testDate = new DateTime(2021,01, 01),
            isCovidPositive = issick
        };
    }

    private Patient CreatePatient1(bool issick){
        return new Patient(
            "000000000", "TestFnameOne", "TestLnameOne", new DateTime(2000, 04, 04), "050-1234567",
            "TestMail1@gmail.com", "TestCityOne", "TestStreetOne", 1, 1, issick, 1,  null
        );
    }
}
}