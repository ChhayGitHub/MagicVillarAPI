
using MagicVillarAPI.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MagicVillarAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
            //    .WriteTo.File("log/villaLogs.txt",rollingInterval:RollingInterval.Day).CreateLogger();
            //builder.Host.UseSerilog();
            builder.Services.AddControllers(option =>
            {
                //option.ReturnHttpNotAcceptable=true;
            }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
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