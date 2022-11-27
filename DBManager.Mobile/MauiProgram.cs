using Blazored.Modal;
using Blazored.Toast;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DBManager.Mobile.Services;

namespace DBManager.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<GrpcService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<TableService>();
            builder.Services.AddSingleton<ColumnService>();
            builder.Services.AddSingleton<RowService>();


            return builder.Build();
        }
    }
}