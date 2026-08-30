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

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ScholarDbContext>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipleFactory>();

// Generic repository available for every entity type.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

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
