using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.VehicleModel)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.VehicleModelId);

            modelBuilder.Entity<VehicleModel>()
                .HasOne(vm => vm.Brand)
                .WithMany(b => b.Models)
                .HasForeignKey(vm => vm.BrandId);


            modelBuilder.Entity<VehicleModel>()
                .HasOne(vm => vm.VehicleType)
                .WithMany(vt => vt.VehicleModels)
                .HasForeignKey(vm => vm.VehicleTypeId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Reservations)
                .HasForeignKey(r => r.VehicleId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Payment)
                .WithOne()
                .HasForeignKey<Payment>(p => p.ReservationId);

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v));

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), v));
        }
    }

}
