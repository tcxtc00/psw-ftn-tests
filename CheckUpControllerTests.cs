using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using psw_ftn.Controllers;
using psw_ftn.Dtos;
using psw_ftn.Dtos.CheckUpDtos;
using psw_ftn.Dtos.UserDtos;
using psw_ftn.Models;
using psw_ftn.Services.CheckUpService;

namespace Psw.UnitTests;

public class CheckUpControllerTests
{
    private readonly Mock<ICheckUpService> repositoryStub = new();
    private readonly Random rand = new();
    private readonly Utils utils = new();

    [Fact]
    public async Task BookCheckUp_WithExistingCheckUp_ReturnsBookedCheckup()
    {
        // Arrange
        CheckUpDto checkUp = utils.CreateCheckup(1, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"), utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist));
        var response = new ServiceResponse<CheckUpDto>();
        response.Data = checkUp;
        response.Success = true;
            
        BookCheckUpDto bookCheckUp = new BookCheckUpDto()
        {
                CheckUpId = checkUp.CheckUpId,
                PatientId = checkUp.Patient.UserId
        };

        repositoryStub.Setup(repo => repo.BookCheckUp(It.IsAny<BookCheckUpDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var responseController = await controller.BookCheckUp(bookCheckUp);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task BookCheckUp_WithExistingCheckUp_ReturnsNotFound()
    {
        // Arrange
        var response = new ServiceResponse<CheckUpDto>();
            
        BookCheckUpDto bookCheckUp = new BookCheckUpDto()
        {
                CheckUpId = 99,
                PatientId = 99
        };

        repositoryStub.Setup(repo => repo.BookCheckUp(It.IsAny<BookCheckUpDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var result = await controller.BookCheckUp(bookCheckUp);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

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
    public async Task GetPatientCheckUps_WithExistingCheckUps_ReturnsExistingCheckUps()
    {
        // Arrange
        var expectedCheckUps = new List<CheckUpDto>
        {
            utils.CreateCheckup(1, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"), utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist)),
            utils.CreateCheckup(2, utils.CreatePatient(2, "Mitar", "Miric", 1, "mitar@gmail.com"), utils.CreateDoctor(4, "Nada", "Macura", 1, "nada@gmail.com", DrExpertiseDto.Specialist))
        };

        var response = new ServiceResponse<List<CheckUpDto>>();
        response.Data = expectedCheckUps;
        response.Success = true;
        
        repositoryStub.Setup(repo => repo.GetPatientCheckUps(It.IsAny<FilterCheckUpDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var responseController = await controller.GetPatientCheckUps(FilterCheckUpDto.HistoryCheckUps);
       
        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<CheckUpDto>());
    }

    [Fact]
    public async Task GetAllPatients_WithExistingPatients_ReturnsAllPatients()
    {
        // Arrange
        var expectedPatients = new List<UserDto>
        {
            utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"),
            utils.CreatePatient(2, "Mitar", "Miric", 1, "mitar@gmail.com")
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedPatients;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetAllPatients())
                .ReturnsAsync(response);
        // Act
        var controller = new CheckUpController(repositoryStub.Object);

        var responseController = await controller.GetAllPatients();

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task GetAllDoctors_WithExistingDoctors_ReturnsAllDoctors()
    {
        // Arrange
        var expectedDoctors = new List<UserDto>
        {
            utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist),
            utils.CreateDoctor(4, "Nada", "Macura", 1, "nada@gmail.com", DrExpertiseDto.Specialist)
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedDoctors;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetAllDoctors())
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetAllDoctors();

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task GetDoctorsByExpertise_WithExpertiseGeneralist_ReturnDoctorsGeneralist()
    {
         // Arrange
        var expectedDoctors = new List<UserDto>
        {
            utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist),
            utils.CreateDoctor(4, "Nada", "Macura", 1, "nada@gmail.com", DrExpertiseDto.Generalist)
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedDoctors;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetDoctorsByExpertise(DrExpertiseDto.Generalist))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetDoctorsByExpertise(DrExpertiseDto.Generalist);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task GetDoctorsByExpertise_WithExpertiseSpecialist_ReturnDoctorsSpecialist()
    {
         // Arrange
        var expectedDoctors = new List<UserDto>
        {
            utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Specialist),
            utils.CreateDoctor(4, "Nada", "Macura", 1, "nada@gmail.com", DrExpertiseDto.Specialist)
        };

        var response = new ServiceResponse<List<UserDto>>();
        response.Data = expectedDoctors;
        response.Success = true;

        repositoryStub.Setup(repo => repo.GetDoctorsByExpertise(DrExpertiseDto.Specialist))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);
        
        // Act
        var responseController = await controller.GetDoctorsByExpertise(DrExpertiseDto.Specialist);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<UserDto>());
    }

    [Fact]
    public async Task GetAvailableCheckUps_WithExistingCheckUps_ReturnsExistingCheckUps()
    {
         // Arrange
        var expectedCheckUps = new List<CheckUpDto>
        {
            utils.CreateCheckup(1, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"), utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Specialist)),
        };

        var response = new ServiceResponse<List<CheckUpDto>>();
        response.Data = expectedCheckUps;
        response.Success = true;
        
        repositoryStub.Setup(repo => repo.GetAvailableCheckUps(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>() , It.IsAny<CheckUpPriorityDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var responseController = await controller.GetAvailableCheckUps(3, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(10) , CheckUpPriorityDto.Doctor);
       
        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<CheckUp>());
    }

    [Fact]
    public async Task CheckUpFeedback_WithExistingCheckUp_ReturnsExistingCheckUp()
    {
        // Arrange
        CheckUpDto checkUp = utils.CreateCheckup(1, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"), utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist));
        var response = new ServiceResponse<CheckUpDto>();
        response.Data = checkUp;
        response.Success = true;
            
        HistoryCheckUpDto historyCheckUp = new HistoryCheckUpDto()
        {
                CheckUpId = checkUp.CheckUpId,
                Comment = "Great",
                Grade = GradeDto.Excellent 
        };

        repositoryStub.Setup(repo => repo.CheckUpFeedback(It.IsAny<HistoryCheckUpDto>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var responseController = await controller.CheckUpFeedback(historyCheckUp);

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task CancelCheckUp_WithCheckUpIdAndComment_ReturnsExistingCheckUp()
    {
        // Arrange
        CheckUpDto checkUp = utils.CreateCheckup(1, utils.CreatePatient(1, "Petar", "Petrovic", 1, "pera@gmail.com"), utils.CreateDoctor(3, "Branimir", "Nestorovic", 1, "nestorovic@gmail.com", DrExpertiseDto.Generalist));
        var response = new ServiceResponse<CheckUpDto>();
        response.Data = checkUp;
        response.Success = true;

        repositoryStub.Setup(repo => repo.CancelCheckUp(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var responseController = await controller.CancelCheckUp(1, "Reason");

        var result = (OkObjectResult)responseController.Result;

        // Assert
        result.Value.Should().BeEquivalentTo(response, options => options.ComparingByMembers<CheckUpDto>());

    }

    [Fact]
    public async Task CancelCheckUp_WithNonExistingparams_ReturnsNotFound()
    {
       // Arrange
        var response = new ServiceResponse<CheckUpDto>();

        repositoryStub.Setup(repo => repo.CancelCheckUp(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(response);

        var controller = new CheckUpController(repositoryStub.Object);

        // Act
        var result = await controller.CancelCheckUp(1,"Comment");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}