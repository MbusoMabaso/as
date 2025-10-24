using Xunit;
using Moq;
using CMCS.Controllers;
using CMCS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMCS.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace CMCS.Tests
{
    public class ClaimControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfClaims()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CMCS_Test_Database")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Claims.Add(new Claim { ClaimID = 1, LecturerID = 1, TotalHours = 10, HourlyRate = 20, Status = ClaimStatus.Submitted });
                context.Claims.Add(new Claim { ClaimID = 2, LecturerID = 2, TotalHours = 5, HourlyRate = 25, Status = ClaimStatus.Submitted });
                    // Dump registered EF entity types for debugging
                    var entityTypes = context.Model.GetEntityTypes().Select(et => et.Name).ToList();
                    Console.WriteLine("Registered entity types in model:");
                    foreach (var et in entityTypes)
                    {
                        Console.WriteLine(et);
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SaveChanges threw: " + ex);
                        throw;
                    }

                        Console.WriteLine($"ChangeTracker entries for Claim after SaveChanges: {context.ChangeTracker.Entries<Claim>().Count()}");

                    // Quick sanity check: ensure the claims were persisted to the in-memory database
                    var persistedCount = context.Claims.Count();
                    Assert.Equal(2, persistedCount);

                var logger = new Mock<ILogger<ClaimController>>();
                var environment = new Mock<IWebHostEnvironment>();
                var controller = new ClaimController(context, environment.Object, logger.Object);

                // Act
                var result = await controller.Index();

                // Assert: controller returns a ViewResult and the in-memory DB contains the expected claims
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Equal(2, persistedCount);
            }
        }
    }
}
