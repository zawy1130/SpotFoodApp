using Microsoft.EntityFrameworkCore;
using SpotFoodApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// ====================== SERVICES ======================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình JSON để tránh vòng lặp reference
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ====================== CORS (RẤT QUAN TRỌNG) ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Cho phép tất cả (dùng cho dev)
              .AllowAnyMethod()
              .AllowAnyHeader();

    });
});

// Kestrel - Nghe trên tất cả IP (quan trọng để Android Emulator kết nối)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5205);   // Port 5205
});

var app = builder.Build();

// ====================== MIDDLEWARE PIPELINE ======================

// Swagger chỉ bật khi Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Quan trọng: UseCors phải đặt ở vị trí này
app.UseCors("AllowAll");

// app.UseHttpsRedirection();   // Tạm comment vì đang dùng HTTP

app.UseAuthorization();

app.MapControllers();

app.Run();