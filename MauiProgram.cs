using Android.App;
using Android.OS;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace linphone_maui
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                 .ConfigureLifecycleEvents(events =>
                 {
#if ANDROID
                     events.AddAndroid(android => android
                         .OnCreate(OnActivity));
#endif
                 });

#if DEBUG
                     builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void OnActivity(Activity activity, Bundle? savedInstanceState)
        {
           var handle = activity.Handle;
            var manager = new LinphoneManager();
            manager.Init(App.ConfigFilePath, App.FactoryFilePath, handle);
            manager.Start();
            ((App)App.Current).Manager = manager;
        }
    }
}
