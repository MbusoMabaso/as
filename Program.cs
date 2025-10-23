using System.IO;
using System.Threading.Tasks;
using ClaimApp.Data;
using ClaimApp.Models;
using ClaimApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ClaimApp.Tests
{
    public class ClaimServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"db_{System.Guid.NewGuid()}")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_SavesClaimAndFile_WhenValid()
        {
            var ctx = CreateContext();
            var envMock = new Mock<IWebHostEnvironment>();
            var tmp = Path.Combine(Path.GetTempPath(), "wwwroot");
            Directory.CreateDirectory(tmp);
            envMock.Setup(e => e.WebRootPath).Returns(tmp);

            var svc = new ClaimService(ctx, envMock.Object, new NullLogger<ClaimService>());

            var model = new Claim { LecturerId = "lec1", HoursWorked = 2, HourlyRate = 50, Notes = "test" };

            // build a fake IFormFile
            var content = System.Text.Encoding.UTF8.GetBytes("dummy");
            var stream = new MemoryStream(content);
            var file = new FormFile(stream, 0, content.Length, "upload", "doc.pdf");

            var created = await svc.CreateAsync(model, file);

            Assert.NotEqual(0, created.Id);
            Assert.Equal("lec1", created.LecturerId);
            Assert.False(string.IsNullOrEmpty(created.UploadedFileName));
        }

        [Fact]
        public async Task ApproveAsync_ChangesStatus()
        {
            var ctx = CreateContext();
            var claim = new Claim { LecturerId = "x", HoursWorked = 1, HourlyRate = 1 };
            ctx.Claims.Add(claim);
            await ctx.SaveChangesAsync();

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var svc = new ClaimService(ctx, envMock.Object, new NullLogger<ClaimService>());
            await svc.ApproveAsync(claim.Id, "admin");
            var loaded = await ctx.Claims.FindAsync(claim.Id);
            Assert.Equal(ClaimStatus.Approved, loaded.Status);
        }
    }
}
