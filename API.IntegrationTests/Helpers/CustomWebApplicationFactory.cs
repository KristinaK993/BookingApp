using API.Data;
using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.IntegrationTests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Ta bort den riktiga databasen
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            // Samma InMemory-databas för app + seeding
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Bygg provider, nollställ DB och seeda testdata
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Nollställ databasen inför varje testkörning
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Company
                db.Companies.Add(new Company { Id = 1, Name = "TestCompany" });

                // Customer
                db.Customers.Add(new Customer
                {
                    Id = 1,
                    CompanyId = 1,
                    FirstName = "Anna",
                    LastName = "Test",
                    Email = "anna@test.com"
                });

                // Employee
                db.Employees.Add(new Employee
                {
                    Id = 1,
                    CompanyId = 1,
                    FirstName = "Erik",
                    LastName = "Test"
                });

                // Service
                db.Services.Add(new Service
                {
                    Id = 1,
                    CompanyId = 1,
                    Name = "Test Service",
                    DurationMinutes = 30,
                    Price = 395
                });

                db.SaveChanges();
            }
        });
    }
}
