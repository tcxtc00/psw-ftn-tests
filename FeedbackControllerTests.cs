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
}