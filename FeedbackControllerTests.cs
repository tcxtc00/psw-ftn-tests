using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Dtos;
using psw_ftn.Dtos.FeedbackDtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.FeedbackService;

namespace Psw.UnitTests;

public class FeedbackControllerTests
{
    private readonly Mock<IFeedbackService> repositoryStub = new();
    private readonly Random rand = new();
    private readonly Utils utils = new(); 

    [Fact]
    public async Task AddFeedback_WithValidValues_ReturnsAddedFeedback()
    {
        // Arrange
        GetFeedbackDto getFeedbackDto = new GetFeedbackDto()
        {
            Grade = GradeDto.Excellent,
            Comment = "Excellent",
            FeedbackId = 1,
            Incognito = false,
            IsForDisplay = true,
            Patient = utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com")
        }; 
       
        var response = new ServiceResponse<GetFeedbackDto>();
        response.Data = getFeedbackDto;
        response.Success = true;

        AddFeedbackDto feedbackDto = new AddFeedbackDto()
        {
            Grade = GradeDto.Excellent,
            Comment = "Excellent",
            incognito = false,
            isForDisplay = true
        };

        repositoryStub.Setup(repo => repo.AddFeedback(It.IsAny<AddFeedbackDto>()))
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);

        // Act
        var responseController = await controller.AddFeedback(feedbackDto);

        var addedFeedback = (OkObjectResult)responseController.Result;

        // Assert
            getFeedbackDto.Should().BeEquivalentTo(
                addedFeedback,
                options => options.ComparingByMembers<GetFeedbackDto>().ExcludingMissingMembers()
            );
    }

    [Fact]
    public async Task AddFeedback_WithInvalidValues_ReturnsNotFound()
    {
        // Arrange
        var response = new ServiceResponse<GetFeedbackDto>();
            
        AddFeedbackDto feedbackDto = new AddFeedbackDto();

        repositoryStub.Setup(repo => repo.AddFeedback(It.IsAny<AddFeedbackDto>()))
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);

        // Act
        var result = await controller.AddFeedback(feedbackDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllFeedbacks_WithExistingFeedbacks_ReturnsAllFeedbacks()
    {
        // Arrange
        var expectedFeedbacks = new List<GetFeedbackDto>
        {
            utils.CreateGetFeedbackDto(GradeDto.VeryBad, "Comment", 1, true, false, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com")),
            utils.CreateGetFeedbackDto(GradeDto.VeryGood, "Comment", 2, false, true, utils.CreatePatient(2, "Mitar", "Miric", 1, "mitar@gmail.com")),
        };

        var response = new ServiceResponse<List<GetFeedbackDto>>();
        response.Data = expectedFeedbacks;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetAllFeedbacks())
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetAllFeedbacks();

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<GetFeedbackDto>());
    }

    [Fact]
    public async Task GetAllFeedbacks_WithUnexistingCheckUps_ReturnsNotFound()
    {
        // Arrange
        var response = new ServiceResponse<List<GetFeedbackDto>>();
        response.Success = false;
        
        repositoryStub.Setup(repo => repo.GetAllFeedbacks())
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);

        // Act
        var result = await controller.GetAllFeedbacks();

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task ShowFeedback_WithValidValues_ReturnsShowedFeedback()
    {
        // Arrange
        GetFeedbackDto getFeedbackDto = new GetFeedbackDto()
        {
            Grade = GradeDto.Excellent,
            Comment = "Excellent",
            FeedbackId = 1,
            Incognito = false,
            IsForDisplay = true,
            Patient = utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com")
        }; 
       
        var response = new ServiceResponse<GetFeedbackDto>();
        response.Data = getFeedbackDto;
        response.Success = true;

        AddFeedbackDto feedbackDto = new AddFeedbackDto()
        {
            Grade = GradeDto.Excellent,
            Comment = "Excellent",
            incognito = false,
            isForDisplay = true
        };

        repositoryStub.Setup(repo => repo.ShowFeedback(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);

        // Act
        var responseController = await controller.ShowFeedback(1, true);

        var addedFeedback = (OkObjectResult)responseController.Result;

        // Assert
            getFeedbackDto.Should().BeEquivalentTo(
                addedFeedback,
                options => options.ComparingByMembers<GetFeedbackDto>().ExcludingMissingMembers()
            );
    }

    [Fact]
    public async Task ShowFeedback_WithInvalidValues_ReturnsShowedFeedback()
    {
         // Arrange
        var response = new ServiceResponse<GetFeedbackDto>();
            
        AddFeedbackDto feedbackDto = new AddFeedbackDto();

        repositoryStub.Setup(repo => repo.ShowFeedback(It.IsAny<int>(), It.IsAny<bool>()))
                .ReturnsAsync(response);

        var controller = new FeedbackController(repositoryStub.Object);

        // Act
        var result = await controller.ShowFeedback(99, true);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}