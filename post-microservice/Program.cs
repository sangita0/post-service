using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using post_microservice.Services;

var builder = WebApplication.CreateBuilder(args);

// Load Firebase configuration
string firebaseKeyPath = Path.Combine(Directory.GetCurrentDirectory(), "microservice-project-96b24-firebase-adminsdk-6fb47-4f62549698.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseKeyPath);

builder.Services.AddSingleton(FirestoreDb.Create(builder.Configuration["Firebase:ProjectId"]));
builder.Services.AddScoped<PostService>(); // Register PostService
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
