using System.Collections;
using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using covidtracking.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using covidtracking.Dtos;

namespace covidtracking.UnitTests{
public class PatientEncountersControllerTests
{   
    private readonly Mock<IPatientsDB> patientsDbModelStub = new();
    private readonly Mock<IPotentialPatientsDB> potentialPatientsDbModelStub = new();
    private readonly Mock<IIsolatedDB> isolatedDbModelStub = new();
    private readonly Mock<ILabTestsDB> labTestsDbModelStub = new();
    private readonly Mock<IPatientsEncountersDB> patientEncountersDbModelStub = new();
    private readonly Mock<IStatisticsDb> statisticsDbModelStub = new();

    [Fact]
    public async Task GetEncountersAsync_InvalidPatientId_ReturnsNotFound(){
        // Arrange
        patientEncountersDbModelStub.Setup(model => model.GetPatientEncountersAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientEncounter) null);
        
        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                            potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                            labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = (await controller.GetEncountersAsync("0000")).Result;
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetEncountersAsync_ValidPatientId_ReturnsOk(){
        // Arrange
        patientEncountersDbModelStub.Setup(model => model.GetPatientEncountersAsync(It.IsAny<string>()))
            .ReturnsAsync(new PatientEncounter("000000000"));
        
        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                            potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                            labTestsDbModelStub.Object, statisticsDbModelStub.Object);
                            
        // Act
        var result = (await controller.GetEncountersAsync("0000")).Result;
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetPotentialPatientsAsync_NoPotentialEncounters_ReturnsNotFound(){
        // Arrange
        potentialPatientsDbModelStub.Setup(model => model.CreateHashSet())
            .Returns(new HashSet<string>());
        patientsDbModelStub.Setup(model => model.CreateHashTable())
            .ReturnsAsync(new Hashtable());
        patientEncountersDbModelStub.Setup(model => model.GetPotentialPatientsEncounters(It.IsAny<HashSet<string>>(),
        It.IsAny<Hashtable>())).Returns((IEnumerable<PotentialPatientsEncounterDto>)null);

        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                           potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                           labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = controller.GetPotentialPatientsAsync().Result;

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetPotentialPatientsAsync_PotentialEncountersExist_ReturnsOk(){
        // Arrange
        potentialPatientsDbModelStub.Setup(model => model.CreateHashSet())
            .Returns(new HashSet<string>());
        patientsDbModelStub.Setup(model => model.CreateHashTable())
            .ReturnsAsync(new Hashtable());
        patientEncountersDbModelStub.Setup(model => model.GetPotentialPatientsEncounters(It.IsAny<HashSet<string>>(),
        It.IsAny<Hashtable>())).Returns(new List<PotentialPatientsEncounterDto>());

        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                           potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                           labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = controller.GetPotentialPatientsAsync().Result;

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddEncounteredAsync_InvalidId_ReturnsNotFound(){
        // Arrange
        patientEncountersDbModelStub.Setup(model => model.GetPatientEncountersAsync(It.IsAny<string>()))
            .ReturnsAsync((PatientEncounter) null);

        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                           potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                           labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = controller.AddEncounteredAsync("000000000", CreatePotentialPatientDto1()).Result;

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddEncounteredAsync_ValidIdInvalidPotentialPatientInput_ReturnsBadRequest(){
        // Arrange
        patientEncountersDbModelStub.Setup(model => model.GetPatientEncountersAsync(It.IsAny<string>()))
            .ReturnsAsync(new PatientEncounter("123321123"));
        potentialPatientsDbModelStub.Setup(model => model.CheckValidPotentialPatientInput(It.IsAny<PotentialPatient>()))
            .Returns(false);

        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                           potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                           labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = controller.AddEncounteredAsync("000000000", CreatePotentialPatientDto1()).Result;

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddEncounteredAsync_ValidInput_ReturnsOk(){
        // Arrange
        patientEncountersDbModelStub.Setup(model => model.GetPatientEncountersAsync(It.IsAny<string>()))
            .ReturnsAsync(new PatientEncounter("123321123"));
        potentialPatientsDbModelStub.Setup(model => model.CheckValidPotentialPatientInput(It.IsAny<PotentialPatient>()))
            .Returns(true);

        var controller = new PatientEncountersController(patientEncountersDbModelStub.Object, patientsDbModelStub.Object,
                           potentialPatientsDbModelStub.Object, isolatedDbModelStub.Object,
                           labTestsDbModelStub.Object, statisticsDbModelStub.Object);

        // Act
        var result = controller.AddEncounteredAsync("000000000", CreatePotentialPatientDto1()).Result;

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }


    private CreatePotentialPatientDto CreatePotentialPatientDto1(){
        return new CreatePotentialPatientDto(){
            firstName = "fName",
            lastName = "lName",
            phoneNumber = "051-8376445"
        };
    }
}
}