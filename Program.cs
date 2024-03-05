using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ECommerceStore.Controllers;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ProductContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<ClaimsPrincipal>(s =>
    s.GetService<IHttpContextAccessor>()!.HttpContext!.User);

builder.Services.AddScoped<RolesController>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        // Other configs...
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                // Call this to skip the default logic and avoid using the default response
                context.HandleResponse();

                // Write to the response in any way you wish
                context.Response.StatusCode = 401;
                context.Response.Headers.Append("my-custom-header", "custom-value");
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ExceptionHandler("UnAuthorized Access")));
                context.Response.ContentType.ToJson();
            }
        };
    });

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//TODO: builder.Services.AddSwaggerExamplesFromAssemblyOf<PurchaseExample>();
builder.Services.AddSwaggerGen(c =>
{
    //TODO: c.ExampleFilters();
});

builder.Services.AddControllers();

//Select type of Database being used
if (DbType.IsSqlLite)
{
    builder.Services.AddDbContext<ProductContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionLite")));
}
else
{
    builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

}

//logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddAzureWebAppDiagnostics();

builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "azure-diagnostics-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});
builder.Services.Configure<AzureBlobLoggerOptions>(options =>
{
    options.BlobName = "log.txt";
});


var app = builder.Build();

//https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-api-management-using-vs?view=aspnetcore-8.0
//Azure API Management needs the Swagger definitions to always be present,
//regardless of the application's environment. To ensure they are always generated,
//move app.UseSwagger(); outside of the if (app.Environment.IsDevelopment()) block.
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers(); //Map controllers automatically

// app.UseHttpsRedirection();

//database operations when the program first starts
using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
{
    //Run Db script to create Products and ProductCategories
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ProductContext>();
    DatabaseInitializerService.Initialize(dbContext);

    //Create Roles
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[]
        { RoleType.SuperAdmin.ToString(), RoleType.Manager.ToString(), RoleType.Admin.ToString(), RoleType.User.ToString() }; //Roles to be added

    foreach (var role in roles)
        //Ensure the role does not exist to prevent duplicate entry
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

//database operations when the program first starts
using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
{
    
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    //Create UserA
    var emailA = "manager@gmail.com";
    var passwordA = "Password123,"; //Password must contain special character

    if (await userManager.FindByEmailAsync(emailA) == null)
    {
        var user = new IdentityUser();
        user.Email = emailA;
        user.UserName = emailA;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, passwordA);

        await userManager.AddToRolesAsync(user,
            new[] { RoleType.SuperAdmin.ToString(), RoleType.Manager.ToString(), RoleType.Admin.ToString(), RoleType.User.ToString() });
    }
    
    //Create UserB
    var emailB = "gibahtony@gmail.com";
    var passwordB = "Password123,"; //Password must contain special character
    if (await userManager.FindByEmailAsync(emailB) == null)
    {
        var user = new IdentityUser();
        user.Email = emailB;
        user.UserName = emailB;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, passwordB);

        await userManager.AddToRolesAsync(user,
            new[] { RoleType.SuperAdmin.ToString(), RoleType.Manager.ToString(), RoleType.Admin.ToString(), RoleType.User.ToString() });
    }
}

app.Run();