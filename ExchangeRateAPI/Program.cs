using Microsoft.EntityFrameworkCore;
using ExchangeRateDB.Data;

namespace ExchangeRateDB
{
    public class Program
    {
        public static void Main( string[] args )
        {
            var builder = WebApplication.CreateBuilder(args);

            // The line below adds the FixerSettings.json from the output directory
            builder.Configuration.AddJsonFile("FixerSettings.json", optional: false);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add DbContext service.
            builder.Services.AddDbContext<ExchangeRateDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ensure the database is created
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ExchangeRateDbContext>();

            try {
                dbContext.Database.EnsureCreated();
            }
            catch (Exception ex) {
                // Log or handle the exception as appropriate
                Console.WriteLine($"An error occurred while ensuring the database is created: {ex.Message}");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}