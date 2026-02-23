using HRSupport.Domain.Common;
using HRSupport.Domain.Entites;
using HRSupport.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;

namespace HRSupport.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Intern> Interns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WeeklyReportRequest> WeeklyReportRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Employee>().Ignore(e => e.Fullname); 

            modelBuilder.Entity<Employee>()
                .Property(e => e.Department)
                .HasColumnName("Department");

            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<LeaveRequest>().ToTable("LeaveRequests");
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Intern>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Email = "admin@hepiyi.com",
                // "Admin123!" şifresinin BCrypt ile hashlenmiş halidir (oluşturma tarihi: 2026)
                PasswordHash = "$2a$11$jLrG8Z.qnfVMlHUTDFVKKOuJl68bpPX1Xs9mYRR8BqN3YXfcVxFDm",
                Role = Roles.Admin,
                IsPasswordChangeRequired = false,
                IsDeleted = false,
                CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
}
    }