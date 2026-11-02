using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddSportsDbIntegration(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddPushNotifications(builder.Configuration);
builder.Services.AddEmail(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// NEMA UseHttpsRedirection() - namerno. App radi ISKLJUCIVO preko HTTP-a na lokalnoj mrezi
// (telefon gadja http://<lokalna-ip>:5237, bez TLS sertifikata) - middleware nema kuda da
// preusmeri (nijedan HTTPS port nije konfigurisan), pa je samo bacao bezopasno upozorenje
// u svaki request. Ako se ikad doda pravi HTTPS (produkcija/hosting), vratiti ovo.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
