using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scholar.Common.Identity;
using Scholar.Common.Storage;
using Scholar.Data;
using Scholar.Models;
using Scholar.Repositories;
using Scholar.Repositories.Impl;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<ScholarDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null)));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Brute-force throttling: lock the account after repeated failed sign-ins.
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // Password strength requirements.
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.User.RequireUniqueEmail = true;
})
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ScholarDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";

    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;

    options.Cookie.Name = "Scholar.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipleFactory>();

// Multi-tenant scoping: resolves the current institute from the signed-in user
// so the DbContext can auto-filter and stamp tenant-owned entities.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();

// Transactional email (SMTP). Configure the "Email" section; keep secrets in user-secrets/env.
builder.Services.Configure<Scholar.Common.Email.EmailOptions>(
    builder.Configuration.GetSection(Scholar.Common.Email.EmailOptions.SectionName));
builder.Services.AddScoped<Scholar.Common.Email.IEmailSender, Scholar.Common.Email.SmtpEmailSender>();
builder.Services.AddScoped<Scholar.Common.Email.IEmailTemplateRenderer, Scholar.Common.Email.EmailTemplateRenderer>();

// Generic repository available for every entity type.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Per-controller business-logic services (controllers stay thin HTTP wiring).
builder.Services.AddScoped<Scholar.Services.IStudentService, Scholar.Services.StudentService>();
builder.Services.AddScoped<Scholar.Services.IFeesService, Scholar.Services.FeesService>();
builder.Services.AddScoped<Scholar.Services.IInstituteService, Scholar.Services.InstituteService>();
builder.Services.AddScoped<Scholar.Services.ITestService, Scholar.Services.TestService>();
builder.Services.AddScoped<Scholar.Services.IAttendanceService, Scholar.Services.AttendanceService>();
builder.Services.AddScoped<Scholar.Services.ISectionService, Scholar.Services.SectionService>();
builder.Services.AddScoped<Scholar.Services.IClassService, Scholar.Services.ClassService>();
builder.Services.AddScoped<Scholar.Services.IInstituteSubjectService, Scholar.Services.InstituteSubjectService>();
builder.Services.AddScoped<Scholar.Services.ISubjectGroupService, Scholar.Services.SubjectGroupService>();
builder.Services.AddScoped<Scholar.Services.IDashboardService, Scholar.Services.DashboardService>();
builder.Services.AddScoped<Scholar.Services.IPastPaperService, Scholar.Services.PastPaperService>();
builder.Services.AddScoped<Scholar.Services.ITeacherService, Scholar.Services.TeacherService>();
builder.Services.AddScoped<Scholar.Services.IChapterService, Scholar.Services.ChapterService>();
builder.Services.AddScoped<Scholar.Services.ISubjectService, Scholar.Services.SubjectService>();
builder.Services.AddScoped<Scholar.Services.IGradeService, Scholar.Services.GradeService>();
builder.Services.AddScoped<Scholar.Services.IBoardService, Scholar.Services.BoardService>();
builder.Services.AddScoped<Scholar.Services.IAuthService, Scholar.Services.AuthService>();
builder.Services.AddScoped<Scholar.Services.IAccountService, Scholar.Services.AccountService>();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

builder.Services.Configure<R2Options>(builder.Configuration.GetSection(R2Options.SectionName));

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    R2Options r2 = sp.GetRequiredService<IOptions<R2Options>>().Value;
    AmazonS3Config config = new()
    {
        ServiceURL = r2.ServiceUrl,
        ForcePathStyle = true,
        AuthenticationRegion = "auto"
    };
    return new AmazonS3Client(r2.AccessKey, r2.SecretKey, config);
});

builder.Services.AddScoped<IFileStorage, R2FileStorage>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication? app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ScholarDbContext db = scope.ServiceProvider.GetRequiredService<ScholarDbContext>();
    db.Database.Migrate();

    // Ensure the app's roles exist on every environment.
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (string role in Scholar.Constants.Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
