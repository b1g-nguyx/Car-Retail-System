using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Car_Rental_System.Controllers;
using Car_Rental_System.Services;
using Car_Rental_System.Repositories;
using Microsoft.AspNetCore.Hosting;
using FluentAssertions;
using Car_Rental_System.ViewModels;
using Car_Rental_System.Models;
using System.Collections.Generic;
using X.PagedList;
using Car_Rental_System.Models.Enums;

public class ContractControllerTests
{
    private readonly Mock<IContractRepository> _mockContractRepo;
    private readonly Mock<IProfileRepository> _mockProfileRepo;
    private readonly Mock<ICarRentalRepository> _mockCarRentalRepo;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly Mock<IWebHostEnvironment> _mockWebHostEnv;
    private readonly Mock<PdfService> _mockPdfService;
    private readonly Mock<ICloudinaryService> _mockCloudinaryService;
    private readonly Mock<ViewRenderService> _mockViewRenderService;
    private readonly ContractController _controller;

    public ContractControllerTests()
    {
        _mockContractRepo = new Mock<IContractRepository>();
        _mockProfileRepo = new Mock<IProfileRepository>();
        _mockCarRentalRepo = new Mock<ICarRentalRepository>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockWebHostEnv = new Mock<IWebHostEnvironment>();
        _mockPdfService = new Mock<PdfService>();
        _mockCloudinaryService = new Mock<ICloudinaryService>();
        _mockViewRenderService = new Mock<ViewRenderService>();
        
        _controller = new ContractController(
            _mockCarRentalRepo.Object,
            _mockProfileRepo.Object,
            _mockEmailSender.Object,
            _mockContractRepo.Object,
            _mockWebHostEnv.Object,
            _mockPdfService.Object,
            _mockCloudinaryService.Object,
            _mockViewRenderService.Object
        );
    }

    [Fact]
    public async Task Index_ReturnsViewWithContracts()
    {
        // Arrange
        var contracts = new StaticPagedList<Contract>(new List<Contract>(), 1, 10, 0);
        _mockContractRepo.Setup(repo => repo.GetAllAsync("", 10, 1)).ReturnsAsync(contracts);

        // Act
        var result = await _controller.Index(null, 8);

        // Assert
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(contracts);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenContractNotExists()
    {
        // Arrange
        _mockContractRepo.Setup(repo => repo.GetByIdAsync(8)).ReturnsAsync((Contract)null);

        // Act
        var result = await _controller.Details(8);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Details_ReturnsViewWithContractViewModel()
    {
        // Arrange
        var contract = new Contract { Id = 8, UserId = "08731414-9b2a-487e-acab-f9a36628a04d" };
        var profile = new Profile { DrivingLicense = "https://res.cloudinary.com/da2dgjdkf/image/upload/v1742203460/ya1yrkxaoddw6dogt91c.png" };

        _mockContractRepo.Setup(repo => repo.GetByIdAsync(8)).ReturnsAsync(contract);
        _mockProfileRepo.Setup(repo => repo.GetByUserIdAsync(contract.UserId)).ReturnsAsync(profile);

        // Act
        var result = await _controller.Details(8);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Which;
        var model = viewResult.Model.Should().BeOfType<ContractViewModel>().Which;
        model.Contract.Should().Be(contract);
        model.imgaeDriver.Should().Be(profile.DrivingLicense);
    }

    [Fact]
    public async Task CreateContract_ReturnsRedirectToIndex_WhenSuccessful()
    {
        // Arrange
        var contract = new Contract { Id = 8, Email = "bichnqhe173220@fpt.edu.vn" };
        var carRental = new CarRental { Id = 8, Status = RentalStatus.PendingConfirmation };
        
        _mockContractRepo.Setup(repo => repo.GetByIdAsync(8, It.IsAny<string>())).ReturnsAsync(contract);
        _mockViewRenderService.Setup(s => s.RenderViewToStringAsync(_controller, "ContractTemplate", contract)).ReturnsAsync("<html>PDF Content</html>");
        _mockPdfService.Setup(s => s.GeneratePdfFromHtml(It.IsAny<string>())).ReturnsAsync(new byte[0]);
        _mockCarRentalRepo.Setup(repo => repo.GetByIdContractAsync(contract.Id)).ReturnsAsync(carRental);
        
        // Act
        var result = await _controller.CreateContract(8);
        
        // Assert
        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        _mockContractRepo.Verify(repo => repo.UpdateAsync(contract), Times.Once);
        _mockCarRentalRepo.Verify(repo => repo.UpdateAsync(carRental), Times.Once);
        _mockEmailSender.Verify(sender => sender.SendEmailAsync(contract.Email!, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
