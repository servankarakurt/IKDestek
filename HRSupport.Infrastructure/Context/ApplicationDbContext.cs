using HRSupport.Domain.Common;
using HRSupport.Domain.Entities;
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
        public DbSet<LeaveApprovalHistory> LeaveApprovalHistories { get; set; }
        public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeNote> EmployeeNotes { get; set; }
        public DbSet<InternTask> InternTasks { get; set; }
        public DbSet<MentorNote> MentorNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Employee>().Ignore(e => e.FullName);
            modelBuilder.Entity<Employee>()
                .Property(e => e.Department)
                .HasColumnName("Department");
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().Property(u => u.Role).HasColumnName("Role");
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<LeaveRequest>().ToTable("LeaveRequests");
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Intern>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<EmployeeLeaveBalance>().ToTable("EmployeeLeaveBalances");

            modelBuilder.Entity<EmployeeNote>().ToTable("EmployeeNotes");
            modelBuilder.Entity<EmployeeNote>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<InternTask>().ToTable("InternTasks");
            modelBuilder.Entity<InternTask>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MentorNote>().ToTable("MentorNotes");
            modelBuilder.Entity<MentorNote>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}