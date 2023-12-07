using bnAPI.Hubs;
using TMDbLib.Client;

var builder = WebApplication.CreateBuilder(args);
var tmbdKey = Environment.GetEnvironmentVariable("TMDbApiKey");
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<TMDbClient>(p => new TMDbClient(tmbdKey));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        b =>
        {
            b.WithOrigins("http://127.0.0.1:5173", "https://thankful-bush-0e0949c0f.3.azurestaticapps.net")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAllHeaders");
app.MapControllers();

app.MapHub<LobbyHub>("/lobby-hub");

app.Run();