using BrainWave.API.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Providers from appsettings.json
var provider = builder.Configuration.GetValue<string>("UseProvider")?.ToLower();
var csSqlite = builder.Configuration.GetConnectionString("Sqlite");
var csPg = builder.Configuration.GetConnectionString("Postgres");

// JWT config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = key
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TasksRepository>();
builder.Services.AddSingleton<ReminderRepository>();
builder.Services.AddSingleton<ExportRepository>();
builder.Services.AddSingleton<BadgeRepository>();
builder.Services.AddSingleton<CollaborationRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
