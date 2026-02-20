using HRSupport.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace HRSupport.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet ismindeki "Employeess" yazım hatası düzeltildi
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Intern> Interns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WeeklyReportRequest> WeeklyReportRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // 1. EMPLOYEE (ÇALIŞAN) TABLOSU AYARLARI
            // ==========================================
            modelBuilder.Entity<Employee>().ToTable("Employees");

            // EF Core'un bu property'i veritabanında sütun olarak aramamasını sağlıyoruz
            modelBuilder.Entity<Employee>().Ignore(e => e.fullname);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Department)
                .HasColumnName("Department");

            // HATA VEREN "Createdate" EŞLEŞTİRMESİ KALDIRILDI

            // GLOBAL QUERY FILTER: Sadece silinmemiş olan çalışanları getir
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);


            // ==========================================
            // 2. LEAVEREQUEST (İZİN TALEBİ) TABLOSU AYARLARI
            // ==========================================
            modelBuilder.Entity<LeaveRequest>().ToTable("LeaveRequests");

            // HATA VEREN "Createdate" EŞLEŞTİRMESİ KALDIRILDI

            // GLOBAL QUERY FILTER: Sadece silinmemiş izin taleplerini getir
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);


            // ==========================================
            // 3. INTERN (STAJYER) VE USER TABLOSU AYARLARI
            // ==========================================
            // Stajyerler ve Kullanıcılar için de varsayılan silinmeme filtresi ekleyebilirsiniz
            modelBuilder.Entity<Intern>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}