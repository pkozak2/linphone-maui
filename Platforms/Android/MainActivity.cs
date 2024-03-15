using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.OS.Storage;
using Java.Lang;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace linphone_maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //var mgnr = new Org.Linphone.Core.Tools.Service.CoreManager()
        //AndroidX.Media.AudioAttributesCompat
        protected override void OnCreate(Bundle? savedInstanceState)
        {

            //var aaaa = AudioAttributesCompat.ContentTypeMovie;
            try
            {
                Class a = Class.ForName("androidx.media.AudioAttributesCompat");
            }
            catch { }
            try
            {
                Class b = Class.ForName("AndroidX.Media.AudioAttributesCompat");
            }
            catch { }
            base.OnCreate(savedInstanceState);
        }
        
    }
}
