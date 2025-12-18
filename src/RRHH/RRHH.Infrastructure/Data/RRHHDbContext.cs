using Microsoft.EntityFrameworkCore;
using RRHH.Domain.Entities;
namespace RRHH.Infrastructure.Data
{
    public class RRHHDbContext : DbContext
    {
        public RRHHDbContext(DbContextOptions<RRHHDbContext> options)
            : base(options)
                {
                }
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName)
                      .IsRequired()
                      .HasMaxLength(150);
                entity.Property(e => e.Position)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Department)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Salary)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();
                entity.Property(e => e.HireDate)
                      .IsRequired();
            });
        }
    }
}
