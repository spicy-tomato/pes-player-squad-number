using Microsoft.EntityFrameworkCore;
using PesPlayerSquadNumber;
using PesPlayerSquadNumber.Services.Implementations;
using PesPlayerSquadNumber.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("PPSNDbContextConnection") ??
                       throw new InvalidOperationException("Connection string 'PPSNDbContextConnection' not found.");

builder.Services.AddDbContext<PpsnDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITransfermarktService, TransfermarktService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<INationService, NationService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();