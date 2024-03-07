using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Tests.Helpers;

public class DbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
    
    public static async Task SeedCompanies(ApplicationDbContext context)
    {
        for (int i = 0; i < 5; i++)
        {
            context.Add(new Company
            {
                CompanyId = 999 + i,
                CompanyName = $"Test Company {i + 1}",
                PriceCategoryId = 1
            });
        }
        await context.SaveChangesAsync();
    }

    public static void Seed(ApplicationDbContext context)
    {
        if (context.PriceCategories.Any())
        {
            return;
        }

        var priceCategories = new PriceCategory[]
        {
            new PriceCategory
            {
                PriceCategoryName = "Category 1",
                ShipRates = new ShipRates { PerKg = 10, PerCubicMeter = 20, PerKm = 30 }
            },
            new PriceCategory
            {
                PriceCategoryName = "Category 2",
                ShipRates = new ShipRates { PerKg = 15, PerCubicMeter = 25, PerKm = 35 }
            },
            new PriceCategory
            {
                PriceCategoryName = "Category 3",
                ShipRates = new ShipRates { PerKg = 20, PerCubicMeter = 30, PerKm = 40 }
            }
        };

        foreach (var priceCategory in priceCategories)
        {
            // Only add the priceCategory if it doesn't already exist
            if (!context.PriceCategories
                    .Any(pc => pc.PriceCategoryName == priceCategory.PriceCategoryName))
            {
                context.PriceCategories.Add(priceCategory);
            }
        }

        var companies = new Company[]
        {
            new Company
            {
                CompanyName = "Company 1",
                PriceCategory = priceCategories.Single(pc => pc.PriceCategoryName == "Category 1")
            },
            new Company
            {
                CompanyName = "Company 2",
                PriceCategory = priceCategories.Single(pc => pc.PriceCategoryName == "Category 2")
            },
            new Company
            {
                CompanyName = "Company 3",
                PriceCategory = priceCategories.Single(pc => pc.PriceCategoryName == "Category 3")
            }
        };
        context.Companies.AddRange(companies);

        var employees = new Employee[]
        {
            new Employee { FirstName = "John", LastName = "Doe", Position = "Manager", CompanyId = 1 },
            new Employee { FirstName = "Jane", LastName = "Doe", Position = "Developer", CompanyId = 1 },
            new Employee { FirstName = "Alice", LastName = "Smith", Position = "Manager", CompanyId = 2 },
            new Employee { FirstName = "Bob", LastName = "Johnson", Position = "Developer", CompanyId = 2 },
            new Employee { FirstName = "Charlie", LastName = "Williams", Position = "Manager", CompanyId = 3 },
            new Employee { FirstName = "David", LastName = "Brown", Position = "Developer", CompanyId = 3 },
        };

        foreach (Employee employee in employees)
        {
            context.Employees.Add(employee);
        }

        context.SaveChanges();
    }
}