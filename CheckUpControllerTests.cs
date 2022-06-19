using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Dtos.CheckUpDtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.CheckUpService;

namespace Psw.UnitTests;

public class CheckUpControllerTests
{
    private readonly Mock<ICheckUpService> repositoryStub = new();
    private readonly Random rand = new();

    [Fact]
    public async Task GetPatientCheckUps_WithUnexistingCheckUps_ReturnsNotFound()
    {
        // Arrange
        var response = new ServiceResponse<List<CheckUpDto>>();
        response.Success = false;
        
        repositoryStub.Setup(repo => repo.GetPatientCheckUps(It.IsAny<FilterCheckUpDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var result = await controller.GetPatientCheckUps(FilterCheckUpDto.HistoryCheckUps);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllPatients_WithExistingPatients_ReturnsAllPatients()
    {
        // Arrange
        var expectedPatients = new List<UserDto>
        {
            CreateRandomPatient(),
            CreateRandomPatient()
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedPatients;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetAllPatients())
                .ReturnsAsync(response);
        // Act
        var controller = new CheckUpController(repositoryStub.Object);

        var responseFromService = await controller.GetAllPatients();

        var actualPatients = (OkObjectResult)responseFromService.Result;

        // Assert
        actualPatients.Value.Should().BeEquivalentTo(response);
    }

     [Fact]
    public async Task GetAllDoctors_WithExistingDoctors_ReturnsAllDoctors()
    {
        // Arrange
        var expectedDoctors = new List<UserDto>
        {
            CreateRandomDoctor(),
            CreateRandomDoctor()
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedDoctors;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetAllDoctors())
                .ReturnsAsync(response);
        // Act
        var controller = new CheckUpController(repositoryStub.Object);

        var responseFromService = await controller.GetAllDoctors();

        var actualDoctors = (OkObjectResult)responseFromService.Result;

        // Assert
        actualDoctors.Value.Should().BeEquivalentTo(response);
    }


     private UserDto CreateRandomPatient()
        {
            return new()
            {
                UserId= rand.Next(1000),
                Role = RoleDto.Patient,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AccessToken = Guid.NewGuid().ToString(),
                Status = 1,
                Email = Guid.NewGuid().ToString(), 
               // CreatedDate = DateTimeOffset.UtcNow
            };
        }

        private UserDto CreateRandomDoctor()
        {
            return new()
            {
                UserId= rand.Next(1000),
                Role = RoleDto.Doctor,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AccessToken = Guid.NewGuid().ToString(),
                Status = 1,
                Expertise = DrExpertiseDto.Generalist.ToString(),
                Email = Guid.NewGuid().ToString(), 
               // CreatedDate = DateTimeOffset.UtcNow
            };
        }
}