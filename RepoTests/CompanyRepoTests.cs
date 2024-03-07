using FluentAssertions;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repos;
using WebApi.Tests.Helpers;

namespace WebApi.Tests.RepoTests;

public class CompanyRepoTests
{
    private readonly CompanyRepo _sut;
    
    private readonly ApplicationDbContext _inMemoryDbContext;
    public CompanyRepoTests()
    {
        _inMemoryDbContext = DbContextFactory.Create();
        DbContextFactory.Seed(_inMemoryDbContext);
        
        _sut = new CompanyRepo(_inMemoryDbContext);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldAddNewCompany_WhenCompanyModelIsValid()
    {
        // Arrange
        var model = new Company
        {
            CompanyId = 10000,
            CompanyName = "TestCompany",
            PriceCategoryId = 10000,
            PriceCategory = new PriceCategory() { PriceCategoryId = 10000, PriceCategoryName = "Test" },
            Employees = new List<Employee>()
        };
        
        
        //A.CallTo(()=> _inMemoryDbContext.Companies.Add(model));

        // Act
        var result = await _sut.CreateAsync(model);

        // Assert
        result.Should().BeEquivalentTo(model);
    }
}