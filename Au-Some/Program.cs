using Au_Some.Core.Identity;
using Au_Some.Core.ServiceContract;
using Au_Some.Core.Services;
using Au_Some.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>().AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
;
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app=builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
