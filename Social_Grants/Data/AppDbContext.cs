using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Models;
using Social_Grants.Models.Account;
using Social_Grants.Models.Grant;

namespace Social_Grants.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<AppUser> TblAppUsers { get; set; }
    public DbSet<BankDetails> TblBankDetails { get; set; }
    public DbSet<ChosenPaymentMethod> TblChosenPaymentMethod { get; set; }
    public DbSet<PostOffice> TblPostOffices { get; set; }
    public DbSet<ChosenPostOffice> TblChosenPostOffice { get; set; }
    public DbSet<PaymentDetails> TblPaymentDetails { get; set; }
    public DbSet<Dependent> TblDependent { get; set; }
    public DbSet<GrantApplications> TblGrantApplications { get; set; }
    public DbSet<Grants> TblGrants { get; set; }
    public DbSet<Gender> TblGender { get; set; }
    public DbSet<Province> TblProvinces { get; set; }
    public DbSet<City> TblCities { get; set; }
    public DbSet<GrantQuestions> TblGrantQuestions { get; set; }
    public DbSet<ApplicantGrantAnswers> TblApplicantGrantAnswers{ get; set; }
    public DbSet<GrantAnswers> TblGrantAnswers { get; set; }
    public DbSet<ApplyForWho> TblApplicationForWho { get; set; }
    public DbSet<UserDocuments> TblUserDocuments { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
