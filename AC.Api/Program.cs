using System.Text;
using AC.Application;
using AC.Application.Services.Security;
using AC.Infrastructure.Persistence.EntityFramework;
using AC.Infrastructure.Persistence.EntityFramework.Seeders;
using AC.Infrastructure.Services.Security;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.TagActionsBy(api =>
    {
        var tag = api.ActionDescriptor.EndpointMetadata
            .OfType<SwaggerOperationAttribute>()
            .FirstOrDefault()?.Tags?.FirstOrDefault();

        return [tag ?? api.ActionDescriptor.RouteValues["controller"]!];
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// jwt----------------------------------------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
//---------------------------------------------------------------
builder.Services.AddCustomDbContext(builder.Configuration);
// para evitar hacer DI por cada crud----------------------------
builder.Services.AddApplication();
//----------------------------------------------------------------
var app = builder.Build();

// insersion de los seeders---------------------------------------
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await sp.GetRequiredService<RoleSeeder>().SeedAsync();
    await sp.GetRequiredService<UserSeeder>().SeedAsync();
}
//-----------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AirCargo API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();