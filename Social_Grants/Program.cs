using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Social_Grants.Data;
//using Microsoft.AspNetCore.Mvc.Authorization;
using Social_Grants.Models.Account;
using Social_Grants.Options;
using Social_Grants.Services;
using Social_Grants.Services.Abstract;
using Social_Grants.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options =>
{
    Options.Cookie.Name = "SocialGrants.Session";
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.IsEssential = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
});
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//});
// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
});
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Users", policy => policy.RequireRole("Administrator", "Customer"));
//});
// Authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DependentAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, GrantApplicationAuthorizationHandler>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection("Azure"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(5);
    o.SlidingExpiration = true;
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //services.AddControllers();
    await SeedData.Initialize(services);
}
if (!app.Environment.IsDevelopment())
{ 
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
