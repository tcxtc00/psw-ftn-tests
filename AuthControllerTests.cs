using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Data;
using psw_ftn.Dtos;
using psw_ftn.Dtos.CheckUpDtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.CheckUpService;

namespace Psw.UnitTests;

public class AuthControllerTests
{
    private readonly Mock<IAuthRepository> repositoryStub = new();
    private readonly Random rand = new();

    private readonly Utils utils = new(); 

    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsRegisteredUser()
    {
        // Arrange
        UserDto newUser = utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com");
       
        var response = new ServiceResponse<UserDto>();
        response.Data = newUser;
        response.Success = true;

        RegisterUserDto registerUser = new RegisterUserDto()
        {
            FirstName = "Petar",
            LastName = "Petrovic",
            Role = RoleDto.Patient,
            Status = StatusDto.Active,
            Password = Guid.NewGuid().ToString(),
            Email = "pera@gmail.com"
        };

        repositoryStub.Setup(repo => repo.Register(It.IsAny<RegisterUserDto>()))
                .ReturnsAsync(response);

        var controller = new AuthController(repositoryStub.Object);

        // Act
        var responseController = await controller.Register(registerUser);

        var createdUser = (OkObjectResult)responseController.Result;

        // Assert
            registerUser.Should().BeEquivalentTo(
                createdUser,
                options => options.ComparingByMembers<UserDto>().ExcludingMissingMembers()
            );
    }

    [Fact]
    public async Task RegisterUser_WithInvalidData_ReturnsBadRequest()
    {
         // Arrange
        var response = new ServiceResponse<UserDto>();
        response.Success = false;

        RegisterUserDto registerUser = new RegisterUserDto()
        {
            FirstName = "Petar",
            LastName = "Petrovic",
            Role = RoleDto.Patient,
            Status = StatusDto.Active,
            Password = Guid.NewGuid().ToString(),
            Email = "peraEmail"
        };

        repositoryStub.Setup(repo => repo.Register(It.IsAny<RegisterUserDto>()))
                .ReturnsAsync(response);

        var controller = new AuthController(repositoryStub.Object);

        // Act
        var result = await controller.Register(registerUser);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

     [Fact]
    public async Task LoginUser_WithValidData_ReturnsLoggedUser()
    {
         // Arrange
        UserDto newUser = utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com");
       
        var response = new ServiceResponse<UserDto>();
        response.Data = newUser;
        response.Success = true;

        LoginUserDto loginUser = new LoginUserDto()
        {
            Email = "pera@gmail.com",
            Password = Guid.NewGuid().ToString(),
        };

        repositoryStub.Setup(repo => repo.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

        var controller = new AuthController(repositoryStub.Object);

        // Act
        var responseController = await controller.Login(loginUser);

        var createdUser = (OkObjectResult)responseController.Result;

        // Assert
            loginUser.Should().BeEquivalentTo(
                createdUser,
                options => options.ComparingByMembers<UserDto>().ExcludingMissingMembers()
            );
    }

    [Fact]
    public async Task LoginUser_WithInvalidData_ReturnsBadRequest()
    {
         // Arrange
        var response = new ServiceResponse<UserDto>();
        response.Success = false;

        LoginUserDto loginUser = new LoginUserDto()
        {
            Password = null,
            Email = "peraEmail"
        };

        repositoryStub.Setup(repo => repo.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

        var controller = new AuthController(repositoryStub.Object);

        // Act
        var result = await controller.Login(loginUser);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}