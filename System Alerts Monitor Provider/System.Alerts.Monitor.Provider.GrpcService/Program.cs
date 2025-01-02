using SQLitePCL;
using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Provider.Controller.Implementation;
using System.Alerts.Monitor.Provider.Controller.Interfaces;
using System.Alerts.Monitor.Provider.DAL.Factories;
using System.Alerts.Monitor.Provider.DAL.Implementations;
using System.Alerts.Monitor.Provider.DAL.Interfaces;
using System.Alerts.Monitor.Provider.GrpcService.Mapper;
using System.Alerts.Monitor.Provider.GrpcService.Services;

namespace System.Alerts.Monitor.Provider.GrpcService
{
    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                //SET SERVICE OPERATION MODE
                var operationmode = builder.Configuration.GetValue<string>("Service:Mode");
                builder.Services.Configure<SAMOptions>(options =>
                {
                    options.ServiceOpeartionMode = (string.IsNullOrEmpty(operationmode)) ? ServiceOperationMode.CSV : (ServiceOperationMode)Enum.Parse(typeof(ServiceOperationMode), operationmode);
                });

                //INIT LOG TOOL
                var logPath = builder.Configuration.GetValue<string>("Log:Path");
                BasicLogToFile logger = new($"{logPath}{DateTime.Now:yyMMddHHmmss}.log");

                //INIT CSV FACTORY
                var csvStorePath = builder.Configuration.GetValue<string>("DataStore:CSV");
                CsvFactory csvFactory = new(csvStorePath!, logger);

                //INIT SQLITE FACTORY
                Batteries.Init();
                var sqliteStorePath = builder.Configuration.GetValue<string>("DataStore:SQLITE");
                SQLiteFactory sqlFactory = new(sqliteStorePath!, csvFactory, logger);


                // Add services to the container.
                builder.Services.AddGrpc();

                // Add Utility classes
                builder.Services
                        .AddSingleton<BasicLogToFile>(logger)
                        .AddSingleton<CsvFactory>(csvFactory)
                        .AddSingleton<SQLiteFactory>(sqlFactory);


                // Add Mapper classes
                builder.Services
                        .AddSingleton<ControllerSystemAlertMapper>()
                        .AddSingleton<SystemAlertMapper>();

                // Add Controller classes
                builder.Services.AddScoped<ISystemAlertDAL, SystemAlertDAL>();
                builder.Services.AddScoped<ISystemAlertController, SystemAlertController>();

                // ADD SystemAlertService
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.MapGrpcService<SystemAlertService>();

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }
    }

}
