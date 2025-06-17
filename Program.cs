using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using SampleMvcApp.Services;
using SampleMvcApp.Hubs;
using SampleMvcApp.Support; // Assuming MongoDbSettings is here
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Auth0 setup
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

// SignalR
builder.Services.AddSignalR();

// Configure MongoDB settings from appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Read MongoDB config values
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString) || string.IsNullOrEmpty(mongoSettings.DatabaseName))
{
    throw new Exception("MongoDbSettings must be configured in appsettings.json");
}

// Create MongoClient and get database instance
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);


// Register IMongoDatabase singleton
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// Register your scoped services that require IMongoDatabase
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<MessageService>();

// Cookie policy setup
builder.Services.ConfigureSameSiteNoneCookies();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
