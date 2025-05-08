using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace SolitaireApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Ensure the app is initialized
            Microsoft.Maui.MauiApplication.Current.OnCreate();

            // Lock the app to landscape mode
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            // Set full-screen mode
            Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);
            Window.DecorView.SystemUiVisibility = (Android.Views.StatusBarVisibility)
                (SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.ImmersiveSticky);
        }
    }
}
