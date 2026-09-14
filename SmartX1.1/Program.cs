using LiteDB;
using SmartX.Services;
using SmartX1._1.Models;
using SmartX1._1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register LiteDB as a singleton (single shared connection for the app's lifetime)
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "smartx.db");
builder.Services.AddSingleton<ILiteDatabase>(_ => new LiteDatabase($"Filename={dbPath};Connection=shared"));

// Register your services
builder.Services.AddScoped<ISensorProfileService, SensorProfileService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddSingleton<IFileAttachmentService, FileAttachmentService>();

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();
