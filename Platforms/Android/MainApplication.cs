using Android.App;
using Android.Content.Res;
using Android.Runtime;

namespace linphone_maui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public static IntPtr Handler;

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            Handler = handle;

            AssetManager assets = Assets;
            string path = FilesDir.AbsolutePath;
            string rc_path = path + "/default_rc";

            if (!File.Exists(rc_path))
            {
                using (StreamReader sr = new StreamReader(assets.Open("linphonerc_default")))
                {
                    string content = sr.ReadToEnd();
                    File.WriteAllText(rc_path, content);
                }
            }
            string factory_path = path + "/factory_rc";
            if (!File.Exists(factory_path))
            {
                using (StreamReader sr = new StreamReader(assets.Open("linphonerc_factory")))
                {
                    string content = sr.ReadToEnd();
                    File.WriteAllText(factory_path, content);
                }
            }
            App.ConfigFilePath = rc_path;
            App.FactoryFilePath = factory_path;

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }


}
