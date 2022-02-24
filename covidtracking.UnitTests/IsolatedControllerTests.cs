using System.Threading.Tasks;
using covidtracking.Controllers;
using covidtracking.Database;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace covidtracking.UnitTests{
public class IsolatedControllerTests
{   
    private readonly Mock<IPatientsDB> patientsDbModelStub = new();
    private readonly Mock<IPotentialPatientsDB> potentialPatientsDbModelStub = new();
    private readonly Mock<IIsolatedDB> isolatedDbModelStub = new();

    [Fact]
    public async Task GetIsolatedAsync_NoIsolatedInDB_ReturnsNotFound(){
        // Arrange
        isolatedDbModelStub.Setup(model => model.GetIsolatedIdsAsync())
            .ReturnsAsync((List<string>)null);

        var controller = new IsolatedController(isolatedDbModelStub.Object, potentialPatientsDbModelStub.Object,
                                                patientsDbModelStub.Object);

        // Act

        var result = (await controller.GetIsolatedAsync()).Result;

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetIsolatedAsync_IsolatedDBNotEmpty_ReturnsOk(){
        // Arrange
        isolatedDbModelStub.Setup(model => model.GetIsolatedIdsAsync())
            .ReturnsAsync(new List<string>());
        isolatedDbModelStub.Setup(model => model.GetIsolatedEncounteredIdsAsync())
            .ReturnsAsync(new List<string>());

        var controller = new IsolatedController(isolatedDbModelStub.Object, potentialPatientsDbModelStub.Object,
                                                patientsDbModelStub.Object);

        // Act

        var result = (await controller.GetIsolatedAsync()).Result;

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
}