using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Dtos;
using psw_ftn.Dtos.PharmacyDtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.PharmacyService;

namespace Psw.UnitTests;

public class PharmacyControllerTests
{
    private readonly Mock<IPharmacyService> repositoryStub = new();
    private readonly Random rand = new();
    private readonly Utils utils = new(); 

    [Fact]
    public async Task PostRecipe_WithValidData_ReturnsPostedRecipe()
    {
        // Arrange
        RecipeDto recipe = new RecipeDto()
        {
            DoctorName = "Branimir Nestorovic",
            Medicine = "Brufen",
            Therapy = "1x dnevno"
        };
       
        var response = new ServiceResponse<RecipeDto>();
        response.Data = recipe;
        response.Success = true;

        repositoryStub.Setup(repo => repo.PostRecipe(It.IsAny<RecipeDto>()))
                .Returns(response);

        var controller = new PharmacyController(repositoryStub.Object);

        // Act
        var responseController = controller.PostRecipe(recipe);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task PostRecipe_WithInvalidData_ReturnsBadRequest()
    {
         // Arrange
        var response = new ServiceResponse<RecipeDto>();
        response.Success = false;

        RecipeDto recipe = new RecipeDto();

        repositoryStub.Setup(repo => repo.PostRecipe(It.IsAny<RecipeDto>()))
                .Returns(response);

        var controller = new PharmacyController(repositoryStub.Object);

        // Act
        var result = controller.PostRecipe(recipe);

        // Assert
        Assert.IsType<ObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetMedicine_WithValidData_ReturnsPostedRecipe()
    {
        // Arrange
        MedicineResponseDto medicine = new MedicineResponseDto()
        {
           name = "Ksanaks",
           quantity = 10,
           supplies = 130,
           errorMsg = "Success"
        };
       
        var response = new ServiceResponse<MedicineResponseDto>();
        response.Data = medicine;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetMedicine(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(response);

        var controller = new PharmacyController(repositoryStub.Object);

        // Act
        var responseController = await controller.GetMedicine("Ksanaks", 10);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task GetMedicine_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var response = new ServiceResponse<MedicineResponseDto>();
        response.Success = false;

        RecipeDto recipe = new RecipeDto();

         repositoryStub.Setup(repo => repo.GetMedicine(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(response);

        var controller = new PharmacyController(repositoryStub.Object);

        // Act
        var result = await controller.GetMedicine("", 100);

        // Assert
        Assert.IsType<ObjectResult>(result.Result);
    }
}