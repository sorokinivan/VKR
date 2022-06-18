using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using VKR.Models;
using VKR.Models.AuthorizationModels;
using VKR.Repositories.Authorization;
using VKR.Repositories.Exams;
using VKR.Repositories.News;
using VKR.Repositories.ProfessionalGrades;
using VKR.Repositories.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
builder.Services.AddScoped<IProfessionalGradeRepository, ProfessionalGradeRepository>();
builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
    .AddEntityFrameworkStores<DBContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddKendo();
builder.Services.Configure<IdentityOptions>(opts => {
    opts.Password.RequiredLength = 2;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
    opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});
builder.Services.ConfigureApplicationCookie(configure => { configure.LoginPath = "/Authorization/Index"; });
builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseSqlServer("workstation id=VKRDatabase.mssql.somee.com;packet size=4096;user id=Nextop077_SQLLogin_1;pwd=gcsoq6d7r5;data source=VKRDatabase.mssql.somee.com;persist security info=False;initial catalog=VKRDatabase");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
