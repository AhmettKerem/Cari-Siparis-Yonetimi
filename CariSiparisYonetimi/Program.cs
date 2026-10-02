using CariSiparisYonetimi.Data;
using Microsoft.EntityFrameworkCore;
using CariSiparisYonetimi.Services;
using CariSiparisYonetimi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controller ve Razor View desteðini DI sistemine ekler.
builder.Services.AddControllersWithViews();

// AppDbContext'i SQL Server baðlantýsýyla DI sistemine kaydeder.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
// Arayüzlerin hangi servis sýnýflarýyla karþýlanacaðýný tanýmlar.
builder.Services.AddScoped<ICariService, CariService>();
builder.Services.AddScoped<ILogoService, LogoService>();
builder.Services.AddScoped<ICariLogoService, CariLogoService>();
builder.Services.AddScoped<ISiparisService, SiparisService>();

var app = builder.Build();

// Geliþtirme dýþýndaki ortamlarda hata ekranýný ve HSTS'yi etkinleþtirir.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Varsayýlan controller, action ve isteðe baðlý Id bilgisini tanýmlar.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
