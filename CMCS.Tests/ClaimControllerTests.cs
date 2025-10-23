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
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<ClaimController>>();
                var environment = new Mock<IWebHostEnvironment>();
                var controller = new ClaimController(context, environment.Object, logger.Object);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Claim>>(viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
            }
        }
    }
}
