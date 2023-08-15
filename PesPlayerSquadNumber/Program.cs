using Microsoft.EntityFrameworkCore;
using PesPlayerSquadNumber;
using PesPlayerSquadNumber.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("PPSNDbContextConnection") ??
                       throw new InvalidOperationException("Connection string 'PPSNDbContextConnection' not found.");

builder.Services.AddDbContext<PpsnDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<TransfermarktService>();
builder.Services.AddScoped<ClubService>();
builder.Services.AddScoped<NationService>();
builder.Services.AddScoped<PlayerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
