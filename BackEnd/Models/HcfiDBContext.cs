using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class HcfiDBContext : DbContext
    {
        public HcfiDBContext()
        {
        }
        public HcfiDBContext(DbContextOptions<HcfiDBContext> options)
         : base(options)
        {
        }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeSarf> EmployeeSarfs { get; set; }
        public virtual DbSet<EmployeeSarf_Esthkak> EmployeeSarf_Esthkaks { get; set; }
        public virtual DbSet<EmployeeSarf_Estkta3> EmployeeSarf_Estkta3s { get; set; }
        public virtual DbSet<Esthkak> Esthkaks { get; set; }
        public virtual DbSet<Estkta3> Estkta3s { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Configure entity relationships and any constraints here
        //    modelBuilder.Entity<EmployeeSarf_Esthkak>()
        //        .HasKey(e => e.Id);

        //    modelBuilder.Entity<EmployeeSarf_Estkta3>()
        //        .HasKey(e => e.Id);

        //    modelBuilder.Entity<EmployeeSarf>()
        //        .HasMany(e => e.EmployeeSarf_Esthkaks)
        //        .WithOne(e => e.EmployeeSarf)
        //        .HasForeignKey(e => e.EmployeeSarfId);

        //    modelBuilder.Entity<EmployeeSarf>()
        //        .HasMany(e => e.EmployeeSarf_Estkta3s)
        //        .WithOne(e => e.EmployeeSarf)
        //        .HasForeignKey(e => e.EmployeeSarfId);
        //}
    }

}
