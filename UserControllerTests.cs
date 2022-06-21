using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Dtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.UserService;

namespace Psw.UnitTests;

public class UserControllerTests
{
    private readonly Mock<IUserService> repositoryStub = new();
    private readonly Random rand = new();
    private readonly Utils utils = new(); 

    [Fact]
    public async Task GetMallicious_WithExistingUsers_ReturnsMaliciousUsers()
    {
        // Arrange
        var expectedUsers = new List<GetUserDto>
        {
            new GetUserDto()
            {
                FirstName = "Pera",
                LastName = "Petrovic",
                Email = "pera@gmail.com",
                UserId = 1,
                Status = 2
            },

            new GetUserDto()
            {
                FirstName = "Zoran",
                LastName = "Petrovic",
                Email = "zoran@gmail.com",
                Status = 2,
                UserId = 2
            }
        };

        var response = new ServiceResponse<List<GetUserDto>>();
        response.Data = expectedUsers;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetMaliciousUsers())
                .ReturnsAsync(response);

        var controller = new UserController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetMallicious();

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<GetUserDto>());
    }

    [Fact]
    public async Task GetBlocked_WithExistingUsers_ReturnsBlockedUsers()
    {
        // Arrange
        var expectedUsers = new List<GetUserDto>
        {
            new GetUserDto()
            {
                FirstName = "Pera",
                LastName = "Petrovic",
                Email = "pera@gmail.com",
                UserId = 1,
                Status = 0
            },

            new GetUserDto()
            {
                FirstName = "Zoran",
                LastName = "Petrovic",
                Email = "zoran@gmail.com",
                Status = 2,
                UserId = 2
            }
        };

        var response = new ServiceResponse<List<GetUserDto>>();
        response.Data = expectedUsers;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetBlockedUsers())
                .ReturnsAsync(response);

        var controller = new UserController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetBlocked();

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<GetUserDto>());
    }

    [Fact]
    public async Task ChangeUserStatus_WithValidData_ReturnsChangedUser()
    {
        // Arrange
        var user = new GetUserDto()
        {
            FirstName = "Zoran",
            LastName = "Petrovic",
            Email = "zoran@gmail.com",
            Status = 2,
            UserId = 1
        };

        var response = new ServiceResponse<GetUserDto>();
        response.Data = user;
        response.Success = true;

        repositoryStub.Setup(repo => repo.ChangeUserStatus(It.IsAny<StatusDto>(), It.IsAny<int>()))
                .ReturnsAsync(response);

        var controller = new UserController(repositoryStub.Object);

        // Act
        var responseController = await controller.ChangeUserStatus(StatusDto.Active, 1);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task ChangeUserStatus_WithInvalidData_ReturnsNotFound()
    {
         // Arrange
        var response = new ServiceResponse<GetUserDto>();
        response.Success = false;

        GetUserDto user = new GetUserDto();

        repositoryStub.Setup(repo => repo.ChangeUserStatus(It.IsAny<StatusDto>(), It.IsAny<int>()))
                .ReturnsAsync(response);

        var controller = new UserController(repositoryStub.Object);

        // Act
        var result = await controller.ChangeUserStatus(StatusDto.Active, 99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}