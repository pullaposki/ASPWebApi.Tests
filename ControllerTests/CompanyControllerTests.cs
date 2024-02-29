using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;

namespace WebApi.Tests.ControllerTests
{
  public class CompanyControllerTests
  {
    private readonly CompanyController _controller;
    public CompanyControllerTests()
    {
      // Create options for DbContext instance
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique name for the in-memory database
        .Options;

      // Create instance of DbContext
      var context = new ApplicationDbContext(options);
      
      var mockService = A.Fake<ICompanyService>();
      var mockRepo = A.Fake<ICompanyRepo>();
      
      _controller = new CompanyController(context, mockRepo, mockService);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
      // Arrange
      _controller.ModelState.AddModelError("error", "some error");

      // Act
      var result = await _controller.Create(new ACreateCompanyRequestDto());

      // Assert
      var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
    }
  }
}
