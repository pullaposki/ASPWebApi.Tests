using FakeItEasy;
using FluentAssertions;
using WebApi.Services;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Interfaces;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;
using WebApi.Mappers;

namespace WebApi.Tests.ServiceTests
{
    public class CompanyServiceTests
    {
        private readonly CompanyService _sut;
        private readonly ICompanyRepo _mockRepo;

        private readonly ApplicationDbContext _inMemoryDbContext;

        public CompanyServiceTests()
        {
            _inMemoryDbContext = Helpers.DbContextFactory.Create();
            Helpers.DbContextFactory.Seed(_inMemoryDbContext);

            _mockRepo = A.Fake<ICompanyRepo>();
            _sut = new CompanyService(_inMemoryDbContext, _mockRepo);
        }

        [Fact]
        public async Task CreateCompany_ReturnsCorrectCompany_WhenDtoIsValid()
        {
            // Arrange
            var dto = new ACreateCompanyRequestDto
            {
                CompanyName = "Test Company",
                PriceCategoryId = 1
            };
            var priceCategory = await _inMemoryDbContext.PriceCategories.FindAsync(dto.PriceCategoryId);

            var model = new Company
            {
                CompanyId = 1,
                CompanyName = dto.CompanyName,
                PriceCategoryId = priceCategory.PriceCategoryId,
                PriceCategory = priceCategory,
                Employees = new List<Employee>()
            };
            
            // Act
            var result = await _sut.CreateCompany(dto);
            
            // Assert
            A.CallTo(() => _mockRepo.CreateAsync(A<Company>.Ignored)).MustHaveHappened();
            result.CompanyName.Should().Be(model.CompanyName);
            result.PriceCategory.Should().BeEquivalentTo(model.PriceCategory);
        }
        
        [Fact]
        public async Task CreateCompany_ThrowsException_WhenPriceCategoryNotFound()
        {
            // Arrange
            var dto = new ACreateCompanyRequestDto
            {
                CompanyName = "Test Company",
                PriceCategoryId = 999
            };
            
            // Act
            Func<Task> act = async () => await _sut.CreateCompany(dto);
            
            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Price category not found");
        }
    }
}