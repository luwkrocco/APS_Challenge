using System.Alerts.Monitor.Common;
using System.Alerts.Monitor.Common.Protos;
using System.Alerts.Monitor.Dashboard.DAL;
using System.Alerts.Monitor.Dashboard.DAL.Interfaces;
using System.Alerts.Monitor.Dashboard.DAL.Mapper;
using System.Alerts.Monitor.Dashboard.DAL.Services;

namespace System.Alerts.Monitor.Dashboard.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //INIT LOG TOOL
            var logPath = builder.Configuration.GetValue<string>("Log:Path");
            var logger = new BasicLogToFile($"{logPath}{DateTime.Now:yyMMddHHmmss}.log");
            builder.Services.AddSingleton<BasicLogToFile>(logger);

            #region GRPC SERVICE RELATED

            //Services
            builder.Services
                .AddScoped<DALMapper>()
                .AddScoped<ISAMProviderService, SAMProviderService>()
                .AddScoped<DALController>();

            var systemAlertServiceURI = builder.Configuration.GetValue<string>("SystemAlertService:URI")!;
            builder.Services
                .AddGrpcClient<SystemAlertService.SystemAlertServiceClient>("SystemAlertServiceClient", (o) => o.Address = new Uri(systemAlertServiceURI))
                .ConfigureChannel(c => c.HttpHandler = new SocketsHttpHandler() { EnableMultipleHttp2Connections = true })
                .EnableCallContextPropagation(o => o.SuppressContextNotFoundErrors = true);

            #endregion

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Dashboard/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Monitor}");

            app.Run();
        }
    }
}
