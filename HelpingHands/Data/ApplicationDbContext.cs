using HelpingHands.Models;
using HelpingHands.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpingHands.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {


        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Code from: https://www.entityframeworktutorial.net/efcore/configure-many-to-many-relationship-in-ef-core.aspx

            //Define the many-to-many relationships to entity framework core so it accepts relationship as it is on the sql server

            #region ManyToMany
            builder.Entity<PreferredSuburb>().HasKey(PS => new { PS.SuburbID, PS.NurseID });

            builder.Entity<PreferredSuburb>()
                .HasOne<Suburb>(PS => PS.Suburb)
                .WithMany(s => s.PreferredSuburbs)
                .HasForeignKey(PS => PS.SuburbID);

             builder.Entity<PreferredSuburb>()
                .HasOne<Nurse>(PS => PS.Nurse)
                .WithMany(s => s.PreferredSuburbs)
                .HasForeignKey(PS => PS.NurseID);

            builder.Entity<PatientChronicCondition>().HasKey(PC => new { PC.PatientID, PC.ChronicID });

            builder.Entity<PatientChronicCondition>()
                .HasOne<Patient>(PC => PC.Patient)
                .WithMany(p => p.PatientCondition)
                .HasForeignKey(PC => PC.PatientID);
            
            builder.Entity<PatientChronicCondition>()
                .HasOne<ChronicCondition>(PC => PC.ChronicCondition)
                .WithMany(p => p.PatientCondition)
                .HasForeignKey(PC => PC.ChronicID);

            #endregion
            //builder.Entity<CareContract>(cc =>
            //{
            //    cc.HasKey(x => x.ContractID);

            //    cc.HasOne(x => x.Nurse).WithOne().HasForeignKey<CareContract>(x => x.NurseID).IsRequired();
            //    cc.HasOne(x => x.Patient).WithOne().HasForeignKey<CareContract>(x => x.PatientID).IsRequired();
            //    cc.HasOne(x => x.Suburb).WithOne().HasForeignKey<CareContract>(x => x.SuburdID).IsRequired();
            //});

            //builder.Entity<CareContract>().HasKey(cc => new { cc.NurseID, cc.PatientID, cc.SuburdID });

            //builder.Entity<CareContract>()
            //    .HasOne<Patient>(CC => CC.Patient)
            //    .WithMany(p => p.CareContracts)
            //    .HasForeignKey(CC => CC.PatientID);

            //builder.Entity<CareContract>()
            //    .HasOne<Nurse>(CC => CC.Nurse)
            //    .WithMany(s => s.CareContracts)
            //    .HasForeignKey(CC => CC.NurseID); 

            //builder.Entity<CareContract>()
            //     .HasOne<Suburb>(PS => PS.Suburb)
            //    .WithMany(s => s.CareContracts)
            //    .HasForeignKey(PS => PS.SuburdID);

            //builder.Entity<CareVisit>()
            //.HasOne(cv => cv.CareContract)
            //.WithMany(cc => cc.CareVisits)
            //.HasPrincipalKey(cc => cc.ContractID)
            //.HasForeignKey(cv => cv.ContractID);

        }

       public DbSet<UserModel> Users { get; set; }
       public DbSet<Nurse> Nurse { get; set; }
       public DbSet<Patient> Patient { get; set; }
       public DbSet<City> City { get; set; }
       public DbSet<Suburb> Suburb { get; set; }
       public DbSet<ChronicCondition> ChronicCondition { get; set; }
       public DbSet<PatientChronicCondition> PatientChronicCondition { get; set; }
       public DbSet<PreferredSuburb> PreferredSuburb { get; set; }
       public DbSet<CareVisit> CareVisit { get; set; }
       public DbSet<CareContract> CareContract { get; set; }
    }
}
