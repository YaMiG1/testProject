using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SkillExtractor.Api.Models;

namespace SkillExtractor.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<CVDocument> CVDocuments { get; set; } = null!;
    public DbSet<EmployeeSkill> EmployeeSkills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee configuration
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Employee>()
            .Property(e => e.FullName)
            .IsRequired();

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.CVDocuments)
            .WithOne(cv => cv.Employee)
            .HasForeignKey(cv => cv.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Skill configuration
        modelBuilder.Entity<Skill>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<Skill>()
            .Property(s => s.Name)
            .IsRequired();

        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique();

        // CVDocument configuration
        modelBuilder.Entity<CVDocument>()
            .HasKey(cv => cv.Id);

        modelBuilder.Entity<CVDocument>()
            .Property(cv => cv.RawText)
            .IsRequired();

        // EmployeeSkill configuration (join entity with composite key)
        modelBuilder.Entity<EmployeeSkill>()
            .HasKey(es => new { es.EmployeeId, es.SkillId });

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(es => es.Employee)
            .WithMany(e => e.EmployeeSkills)
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeSkill>()
            .HasOne(es => es.Skill)
            .WithMany(s => s.EmployeeSkills)
            .HasForeignKey(es => es.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
