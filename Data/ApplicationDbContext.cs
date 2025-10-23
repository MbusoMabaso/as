using System.Collections.Generic;
using System.Reflection.Emit;
using CMCS.Models; // Assuming your models are in this namespace
using Microsoft.EntityFrameworkCore;
using ClaimApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ClaimApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor that takes DbContextOptions and passes them to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for your models.
        // Each DbSet corresponds to a table in your database.
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimApproval> ClaimApprovals { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerID);

            modelBuilder.Entity<ClaimApproval>()
                .HasOne(ca => ca.Claim)
                .WithMany()
                .HasForeignKey(ca => ca.ClaimID);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimID);

            // Seed data
            modelBuilder.Entity<Lecturer>().HasData(
                new Lecturer { LecturerID = 1, Name = "John Smith", Email = "john.smith@university.ac.za" },
                new Lecturer { LecturerID = 2, Name = "Sarah Johnson", Email = "sarah.johnson@university.ac.za" },
                new Lecturer { LecturerID = 3, Name = "Michael Brown", Email = "michael.brown@university.ac.za" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { UserID = 1, Username = "admin", Password = "admin123", Role = "AcademicManager" },
                new User { UserID = 2, Username = "coordinator", Password = "coord123", Role = "ProgrammeCoordinator" },
                new User { UserID = 3, Username = "hr", Password = "hr123", Role = "HR" }
            );
        }

    }

    public interface IClaimService
    {
        Task<Claim> CreateAsync(Claim claim, IFormFile file);
        Task<Claim> GetAsync(int id);
        Task ApproveAsync(int id, string approverId);
        Task RejectAsync(int id, string approverId, string reason = null);
        Task<IEnumerable<Claim>> GetPendingAsync();
    }
}