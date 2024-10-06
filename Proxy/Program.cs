var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.WebHost.UseUrls("http://*:8088");
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Aquí permites el acceso desde tu frontend Angular
              .AllowAnyHeader()                      // Permite cualquier encabezado
              .AllowAnyMethod();                     // Permite cualquier método (GET, POST, PUT, DELETE, etc.)
    });
});

// Configure the HTTP request pipeline.
var app = builder.Build();
app.UseCors("AllowAngularClient");
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();
 
app.MapControllers();

app.Run();
