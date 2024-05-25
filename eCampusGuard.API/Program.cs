using System.Reflection;
using eCampusGuard.API.Data;
using eCampusGuard.API.Extensions;
using eCampusGuard.API.Helpers;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using eCampusGuard.Services.EMailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });

});

builder.Services.AddSignalR().AddJsonProtocol();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<XEnumNamesDocumentFilter>();
    options.OperationFilter<AuthorizationOperationFilter>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"https://0.0.0.0:{port}";
builder.WebHost.UseUrls(url);

var app = builder.Build();


app.UseCors("CorsPolicy");

app.UseHttpLogging();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<AreaHub>("/Area/feed/{id}");

app.MapControllers();

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<SQLDataContext>();
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    var seed = new Seed(userManager, roleManager, unitOfWork);
    await seed.SeedData();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error Occured during migration");
}

app.Run();

