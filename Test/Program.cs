using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Test;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 41)), // MySQL sürümünüze göre güncelleyin
        mySqlOptions => mySqlOptions.EnableRetryOnFailure() 
    )
);

// Identity servislerini ekleyin (AppUser ve IdentityRole kullanarak)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Identity ayarlarını buradan yapılandırabilirsiniz.
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    // İsteğe bağlı: Kestrel ayarlarını yapılandırabilirsiniz.
    // options.ListenAnyIP(8080);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDBContext>();
    context.Database.Migrate();
}

app.MapControllers();

app.Run();
