using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options)=>
{
    options.AddPolicy("DevCors", (corsBuilders) =>
    {
        corsBuilders.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("ProdCors", (corsBuilders) =>
    {
        corsBuilders.WithOrigins("https://myproducitonsite.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// string? tokenKeyString= builder.Configuration.GetSection("AppSettings:Token").Value;

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//          options.TokenValidationParameters = new TokenValidationParameters()
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//                 tokenKeyString!= null ? tokenKeyString: ""
//             )),
//             ValidateIssuer = false,
//             ValidateAudience = false
//         };
//     });

    
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:TokenKey").Value
            )),
            ValidateIssuer= false,
            ValidateAudience= false
        };
    }) ;

    var app = builder.Build();
    
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else 
{
app.UseCors("ProdCors");
app.UseHttpsRedirection();
}
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
