using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingService> BookingServices => Set<BookingService>();
    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table names (valfritt, men fint)
        modelBuilder.Entity<Company>().ToTable("Companies");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Service>().ToTable("Services");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Booking>().ToTable("Bookings");
        modelBuilder.Entity<BookingService>().ToTable("BookingServices");

        // Company relations
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Employees)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Services)
            .WithOne(s => s.Company)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Customers)
            .WithOne(cu => cu.Company)
            .HasForeignKey(cu => cu.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Bookings)
            .WithOne(b => b.Company)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Customer -> Bookings
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Bookings)
            .WithOne(b => b.Customer)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Employee -> Bookings
        modelBuilder.Entity<Employee>()
            .HasMany(e => e.Bookings)
            .WithOne(b => b.Employee)
            .HasForeignKey(b => b.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking -> BookingServices (many-to-many Booking <-> Service via join entity)
        modelBuilder.Entity<BookingService>()
            .HasKey(bs => new { bs.BookingId, bs.ServiceId });

        modelBuilder.Entity<BookingService>()
            .HasOne(bs => bs.Booking)
            .WithMany(b => b.BookingServices)
            .HasForeignKey(bs => bs.BookingId);

        modelBuilder.Entity<BookingService>()
            .HasOne(bs => bs.Service)
            .WithMany(s => s.BookingServices)
            .HasForeignKey(bs => bs.ServiceId);

        // Store BookingStatus as int
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<int>();

        //decimal for services
        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        //user
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
