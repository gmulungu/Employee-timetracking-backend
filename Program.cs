using EmployeeTimeTrackingBackend;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseCors("AllowAngularApp");


app.MapControllers();

app.Run();


Database db = new Database();
db.GetConnection();