using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Tests.ControllerTests
{
  public class CompanyControllerTests
  {
    readonly CompanyController _sut;
    
    readonly ICompanyRepo _mockRepo;
    readonly ICompanyService _mockService;
    
    public CompanyControllerTests()
    {
      // Create options for DbContext instance
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
        .Options;

      // Create instance of DbContext
      var context = new ApplicationDbContext(options);
      
      _mockService = A.Fake<ICompanyService>();
      _mockRepo = A.Fake<ICompanyRepo>();
      
      _sut = new CompanyController(context, _mockRepo, _mockService);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
      // Arrange
      _sut.ModelState.AddModelError("error", "some error");

      // Act
      var result = await _sut.Create(new ACreateCompanyRequestDto());

      // Assert
      var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenModelAndDtoAreValid()
    {
      // Arrange
      var validDto = new ACreateCompanyRequestDto
      {
        CompanyName = "Test Company",
        PriceCategoryId = 1
      };

      var validModel = new Company
      {
        CompanyId = 1,
        CompanyName = validDto.CompanyName,
        PriceCategoryId = validDto.PriceCategoryId
      };
      
      A.CallTo(() => _mockService
        .CreateCompany(validDto))
        .Returns(validModel);
      
      A.CallTo(() => _mockRepo
        .CreateAsync(validModel))
        .Returns(Task.FromResult(validModel));

      // Act
      var result = await _sut.Create(validDto);
      
      // Assert
      var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
      var returnedCompany = createdAtActionResult.Value.Should().BeAssignableTo<ACompanyResponseDto>().Subject;

      returnedCompany.CompanyId.Should().Be(validModel.CompanyId);
      returnedCompany.CompanyName.Should().Be(validModel.CompanyName);
      returnedCompany.PriceCategoryId.Should().Be(validModel.PriceCategoryId);
    }
    
    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenExceptionIsThrown()
    {
      // Arrange
      A.CallTo(() => _mockService
        .CreateCompany(A<ACreateCompanyRequestDto>.Ignored))
        .Throws<Exception>();

      // Act
      var result = await _sut.Create(new ACreateCompanyRequestDto());

      // Assert
      var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
      statusCodeResult.StatusCode.Should().Be(500);
    }
  }
}
