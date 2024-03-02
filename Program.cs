using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ECommerceStore.Controllers;
using ECommerceStore.Models;
using ECommerceStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

//logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers(); //Map controllers automatically

app.UseHttpsRedirection();

//database operations when the program first starts
using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
{
    //Run Db script to create Products and ProductCategories
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ProductContext>();
    DatabaseInitializerService.Initialize(dbContext);

    //Create Roles
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[]
        { RoleType.Manager.ToString(), RoleType.Admin.ToString(), RoleType.User.ToString() }; //Roles to be added

    foreach (var role in roles)
        //Ensure the role does not exist to prevent duplicate entry
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

//database operations when the program first starts
using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.CreateScope())
{
    //Create User
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var email = "manager@gmail.com";
    var password = "Password123,"; //Password must contain special character

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser();
        user.Email = email;
        user.UserName = email;
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);

        await userManager.AddToRolesAsync(user,
            new[] { RoleType.Manager.ToString(), RoleType.Admin.ToString(), RoleType.User.ToString() });
    }
}

app.Run();