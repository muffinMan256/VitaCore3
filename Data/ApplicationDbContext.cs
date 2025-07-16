using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using VitaCore.Models;
using System.Reflection.Emit;

namespace VitaCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppointmentModel> Appointments { get; set; }
        public DbSet<AlarmModel> Alarms { get; set; }
        public DbSet<ChartDataModel> ChartData { get; set; }
        public DbSet<DoctorModel> Doctors { get; set; }
        public DbSet<EcgSignalModel> EcgSignal { get; set; }
        public DbSet<LocationMapModel> LocationMap { get; set; }
        public DbSet<MedicalHistoryModel> MedicalHistory { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<PatientModel> Patients { get; set; }
        public DbSet<PhysicalActivityModel> PhysicalActivities { get; set; }
        public DbSet<RecommendationModel> Recommendation { get; set; }
        public DbSet<SensorDataModel> SensorData { get; set; }
        public DbSet<UserFavoritesModel> UserFavorites { get; set; }


        //public DbSet<UserFavorite> UserFavorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRole");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaim");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogin");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaim");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserToken");
            });

            builder.Entity<AlarmModel>().ToTable("Alarms");
            builder.Entity<ChartDataModel>().ToTable("ChartData");
            builder.Entity<DoctorModel>().ToTable("Doctors");
            builder.Entity<EcgSignalModel>().ToTable("EcgSignal");
            builder.Entity<LocationMapModel>().ToTable("LocationMap");
            builder.Entity<MedicalHistoryModel>().ToTable("MedicalHistory");
            builder.Entity<MessageModel>().ToTable("Messages");
            builder.Entity<PatientModel>().ToTable("Patients");
            builder.Entity<PhysicalActivityModel>().ToTable("PhysicalActivities");
            builder.Entity<RecommendationModel>().ToTable("Recommendation");
            builder.Entity<SensorDataModel>().ToTable("SensorData");
            builder.Entity<UserFavoritesModel>().ToTable("UserFavorites");
            builder.Entity<AppointmentModel>().ToTable("Appointments");


        }



    }
}
