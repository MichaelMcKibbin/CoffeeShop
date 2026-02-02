using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;

namespace CoffeeShop.Platforms.Android
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
        WindowSoftInputMode = global::Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Log initial theme
            LogCurrentTheme("OnCreate");
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            
            // Log when configuration changes
            LogCurrentTheme("OnConfigurationChanged");
            
            // Manually trigger theme update in MAUI (only if in system theme mode)
            UpdateAppThemeIfSystemMode(newConfig);
        }

        private static void UpdateAppThemeIfSystemMode(Configuration config)
        {
            var currentNightMode = config.UiMode & UiMode.NightMask;
            var isDeviceDark = currentNightMode == UiMode.NightYes;
            
            global::Android.Util.Log.Info("CoffeeShop", $"🎨 Android Theme detected: {(isDeviceDark ? "Dark" : "Light")}");
            
            Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(() =>
            {
                var app = Microsoft.Maui.Controls.Application.Current;
                if (app == null) return;
                
                // KEY FIX: Only update if we're following system theme (Unspecified)
                if (app.UserAppTheme != Microsoft.Maui.ApplicationModel.AppTheme.Unspecified)
                {
                    global::Android.Util.Log.Info("CoffeeShop", $"🚫 Not in system theme mode, ignoring (UserAppTheme={app.UserAppTheme})");
                    return;
                }
                
                // We're in system theme mode - explicitly set the detected theme
                var targetTheme = isDeviceDark 
                    ? Microsoft.Maui.ApplicationModel.AppTheme.Dark 
                    : Microsoft.Maui.ApplicationModel.AppTheme.Light;
                
                global::Android.Util.Log.Info("CoffeeShop", $"✅ Applying detected theme: {targetTheme}");
                app.UserAppTheme = targetTheme;
                
                // Immediately set back to Unspecified to continue following system
                app.UserAppTheme = Microsoft.Maui.ApplicationModel.AppTheme.Unspecified;
                
                global::Android.Util.Log.Info("CoffeeShop", $"✅ Reset to Unspecified to follow future changes");
            });
        }

        private void LogCurrentTheme(string context)
        {
            var currentNightMode = Resources?.Configuration?.UiMode & UiMode.NightMask;
            var isDarkMode = currentNightMode == UiMode.NightYes;
            global::Android.Util.Log.Info("CoffeeShop", $"📱 {context} - Current theme: {(isDarkMode ? "Dark" : "Light")}");
        }
    }
}

