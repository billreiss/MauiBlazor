using Microsoft.Extensions.Logging;
using System.Resources;
using WebviewAppTest;
using WebviewAppTest.Data;

namespace BlazorMauiApp
{
    public static class MauiProgram
    {
        private static readonly AppState _appState = new();
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
            builder.Services.AddSingleton<AppState>(_appState);
            builder.Services.AddSingleton<WeatherForecastService>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}