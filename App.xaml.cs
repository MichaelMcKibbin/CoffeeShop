using Microsoft.Maui.ApplicationModel;

namespace CoffeeShop
{
    public partial class App : Application
    {
        private const string UseSystemThemeKey = "use_system_theme";
        private const string ManualThemeKey = "manual_theme";

        public App()
        {
            /*
            Called by .NET MAUI to load App.xaml.cs
            Merges all resource dictionaries(Colors, Styles)
            Sets up initial theme based on device settings
            Runs once when the app first launches
            */
            InitializeComponent();
            this.RequestedThemeChanged += OnRequestedThemeChanged;
            
            // Apply saved theme preference on startup
            ApplySavedThemePreference();
            
            // Log initial theme
            System.Diagnostics.Debug.WriteLine($"🚀 App started with theme: {this.UserAppTheme}, Requested: {this.RequestedTheme}");
        }

        private void ApplySavedThemePreference()
        {
            var useSystemTheme = Preferences.Get(UseSystemThemeKey, true);
            
            System.Diagnostics.Debug.WriteLine($"📖 Read UseSystemTheme preference: {useSystemTheme}");
            
            if (!useSystemTheme)
            {
                // Apply manual theme
                var isDarkMode = Preferences.Get(ManualThemeKey, false);
                this.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
                System.Diagnostics.Debug.WriteLine($"📱 Manual theme applied on startup: {(isDarkMode ? "Dark" : "Light")}");
            }
            else
            {
                // Use system theme (default)
                this.UserAppTheme = AppTheme.Unspecified;
                System.Diagnostics.Debug.WriteLine($"📱 Using system theme on startup (Unspecified)");
            }
        }

        // Window created when app starts
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        
        private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"🎨 RequestedThemeChanged fired: {e.RequestedTheme}, UserAppTheme: {this.UserAppTheme}");
            
            // KEY FIX: If UserAppTheme is Unspecified, we're in system theme mode - ALWAYS apply
            if (this.UserAppTheme != AppTheme.Unspecified)
            {
                // We're in manual mode (Light or Dark explicitly set)
                System.Diagnostics.Debug.WriteLine($"🚫 System theme change ignored (manual mode - UserAppTheme={this.UserAppTheme})");
                return;
            }
            
            // We're following system theme (Unspecified), so apply the change
            System.Diagnostics.Debug.WriteLine($"✅ Applying system theme change: {e.RequestedTheme}");

#if ANDROID
            global::Android.Util.Log.Info("CoffeeShop", $"🎨 Theme changed event: {e.RequestedTheme}");
            
            // Update Android status bar color to match theme
            var colorHex = e.RequestedTheme == AppTheme.Dark ? "#161312" : "#FAF7F2";
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
#pragma warning disable CA1422 // Validate platform compatibility
                    Platform.CurrentActivity?.Window?.SetStatusBarColor(
                        global::Android.Graphics.Color.ParseColor(colorHex)
                    );
#pragma warning restore CA1422
                    global::Android.Util.Log.Info("CoffeeShop", $"✅ Status bar updated to: {colorHex}");
                    System.Diagnostics.Debug.WriteLine($"✅ Status bar color set to: {colorHex}");
                }
                catch (Exception ex)
                {
                    global::Android.Util.Log.Error("CoffeeShop", $"❌ Status bar update failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"❌ Status bar update failed: {ex.Message}");
                }
            });
#endif

            // Save current theme for reference
            Preferences.Set("last_system_theme", e.RequestedTheme.ToString());
        }
    }
}