using HRSupport.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HRSupport.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employeess { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; } 
        public DbSet<Intern> Interns { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // 1. EMPLOYEE (ÇALIŞAN) TABLOSU AYARLARI
            // ==========================================
            modelBuilder.Entity<Employee>().ToTable("Employees"); 

            // EF Core'un bu property'i veritabanında sütun olarak aramamasını sağlıyoruz
            modelBuilder.Entity<Employee>().Ignore(e => e.fullname);

            // İsim uyuşmazlıklarını çözüyoruz
            modelBuilder.Entity<Employee>()
                .Property(e => e.Deparment)
                .HasColumnName("Department");

            modelBuilder.Entity<Employee>()
                .Property(e => e.CreatedTime)
                .HasColumnName("Createdate");

            // GLOBAL QUERY FILTER: Sadece silinmemiş olan çalışanları getir
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);


            // ==========================================
            // 2. LEAVEREQUEST (İZİN TALEBİ) TABLOSU AYARLARI
            // ==========================================
            modelBuilder.Entity<LeaveRequest>().ToTable("LeaveRequests");

            // BaseEntity'den gelen CreatedTime alanını SQL'deki Createdate sütunuyla eşleştiriyoruz
            modelBuilder.Entity<LeaveRequest>()
                .Property(e => e.CreatedTime)
                .HasColumnName("Createdate");

            
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);


        }
    }
}