using InficareTaskProject.Classes;
using InficareTaskProject.Core.Permission;
using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.Interfaces;
using InficareTaskProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// db connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// add identity
builder.Services.AddIdentity<Student, Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IJwtTokenManager, JwtTokenManager>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:JwtKey"])),

                       ValidateIssuer = true,
                       ValidIssuer = builder.Configuration["JWT:JwtIssuer"],

                       ValidateAudience = true,
                       ValidAudience = builder.Configuration["JWT:JwtAudience"],
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PermissionTypes.CreateStudent, policy =>
    policy.RequireClaim("permissions", PermissionTypes.CreateStudent));

    options.AddPolicy(PermissionTypes.GetStudent, policy =>
    policy.RequireClaim("permissions", PermissionTypes.GetStudent));   
    
    options.AddPolicy(PermissionTypes.UpdateStudent, policy =>
    policy.RequireClaim("permissions", PermissionTypes.UpdateStudent)); 
    
    options.AddPolicy(PermissionTypes.DeleteStudent, policy =>
    policy.RequireClaim("permissions", PermissionTypes.DeleteStudent));

    //options.AddPolicy(PermissionTypes.CreateRole, policy =>
    //policy.RequireClaim("permissions", PermissionTypes.CreateRole));

    //options.AddPolicy(PermissionTypes.UpdateRole, policy =>
    //policy.RequireClaim("permissions", PermissionTypes.UpdateRole));

    //options.AddPolicy(PermissionTypes.GetRole, policy =>
    //policy.RequireClaim("permissions", PermissionTypes.GetRole));

    //options.AddPolicy(PermissionTypes.DeleteRole, policy =>
    //policy.RequireClaim("permissions", PermissionTypes.DeleteRole));
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bearer Jwt Token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyOrigin()    // Allow requests from any origin
        .AllowAnyMethod()    // Allow any HTTP method
        .AllowAnyHeader());  // Allow any header
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapControllers();
await DbInitializer.InitializeAsync(app.Services);


app.Run();
