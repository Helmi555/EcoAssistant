using EcoAssistant.API.Services;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Application.Services;
using EcoAssistant.Infrastructure.Data;
using EcoAssistant.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("Mqtt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<MqttOptions>>().Value);
builder.Services.AddHostedService<EcoAssistant.API.Services.MqttHostedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EcoAssistant API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    c.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();

});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// User
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IUserService,UserService>();

//user Group
builder.Services.AddScoped<IUserGroupRepository, EfUserGroupRepository>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();


//Group
builder.Services.AddScoped<IGroupRepository,EfGroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();

//IndustryCategory
builder.Services.AddScoped<IIndustryCategoryRepository, EfIndustryCategoryRepository>();
builder.Services.AddScoped<IIndustryCategoryService, IndustryCategoryService>();

//Device
builder.Services.AddScoped<IDeviceRepository, EfDeviceRepository>();




var keyString = builder.Configuration["Jwt:Key"] ?? throw new System.InvalidOperationException("Jwt:Key is not configured.");
var key = Encoding.ASCII.GetBytes(keyString);
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
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.Configure<MqttOptions>(
    builder.Configuration.GetSection("Mqtt")
);

// Register the background MQTT service
builder.Services.AddHostedService<MqttHostedService>();
var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

var isHotReload = Environment.GetEnvironmentVariable("DOTNET_WATCH") == "1";

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoAssistant API v1");
    });
}



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();