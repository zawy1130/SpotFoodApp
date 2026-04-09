using Android.App;
using Android.Content.PM;
using Android.OS;

using Android;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SpotFoodApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize 
        | ConfigChanges.Orientation 
        | ConfigChanges.UiMode 
        | ConfigChanges.ScreenLayout 
        | ConfigChanges.SmallestScreenSize 
        | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Xin quyền GPS runtime
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this,
                    new string[] { 
                        Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation },
                    1);
            }
        }
    }
}