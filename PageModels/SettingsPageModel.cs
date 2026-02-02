using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace CoffeeShop.PageModels
{
    public partial class SettingsPageModel : BaseViewModel
    {
        private const string UseSystemThemeKey = "use_system_theme";
        private const string ManualThemeKey = "manual_theme";

        public SettingsPageModel()
        {
            Title = "Settings";
            
            // Load saved preferences
            useSystemTheme = Preferences.Get(UseSystemThemeKey, true);
            isDarkMode = Preferences.Get(ManualThemeKey, false);
            
            UpdateCurrentThemeText();
        }

        [ObservableProperty]
        private bool useSystemTheme;

        [ObservableProperty]
        private bool isDarkMode;

        [ObservableProperty]
        private string currentThemeText = "";

        // Show manual toggle only when not using system theme
        public bool IsManualThemeControlVisible => !UseSystemTheme;
        public bool IsManualThemeControlEnabled => !UseSystemTheme;

        partial void OnUseSystemThemeChanged(bool value)
        {
            Preferences.Set(UseSystemThemeKey, value);
            System.Diagnostics.Debug.WriteLine($"💾 UseSystemTheme set to: {value}");
            
            // Notify UI that visibility changed
            OnPropertyChanged(nameof(IsManualThemeControlVisible));
            OnPropertyChanged(nameof(IsManualThemeControlEnabled));
            
            if (Application.Current == null) return;
            
            if (value)
            {
                // Enable system theme mode
                System.Diagnostics.Debug.WriteLine("🔄 Enabling system theme mode...");
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                System.Diagnostics.Debug.WriteLine($"✅ UserAppTheme set to: {Application.Current.UserAppTheme}");
                
                // Force a theme re-evaluation on Android by querying RequestedTheme
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null)
                    {
                        var requestedTheme = Application.Current.RequestedTheme;
                        System.Diagnostics.Debug.WriteLine($"📱 Requested theme after enabling system mode: {requestedTheme}");
                    }
                });
            }
            else
            {
                // Enable manual theme mode
                System.Diagnostics.Debug.WriteLine("🔄 Enabling manual theme mode...");
                ApplyManualTheme();
            }
            
            UpdateCurrentThemeText();
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            // Only apply if not using system theme
            if (!UseSystemTheme)
            {
                Preferences.Set(ManualThemeKey, value);
                ApplyManualTheme();
                UpdateCurrentThemeText();
            }
        }

        private void ApplyManualTheme()
        {
            if (Application.Current == null) return;
            
            Application.Current.UserAppTheme = IsDarkMode 
                ? AppTheme.Dark 
                : AppTheme.Light;
            
            System.Diagnostics.Debug.WriteLine($"🎨 Manual theme applied: {Application.Current.UserAppTheme}");
        }

        private void UpdateCurrentThemeText()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current == null)
                {
                    CurrentThemeText = "Unknown";
                    return;
                }

                var actualTheme = Application.Current.RequestedTheme;
                var userTheme = Application.Current.UserAppTheme;
                var source = UseSystemTheme ? "System" : "Manual";
                var theme = actualTheme == AppTheme.Dark ? "Dark" : "Light";
                
                CurrentThemeText = $"Current: {theme} ({source})";
                System.Diagnostics.Debug.WriteLine($"📊 Theme status - Requested: {actualTheme}, User: {userTheme}, Source: {source}");
            });
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void OnAppearing()
        {
            UpdateCurrentThemeText();
        }
    }
}
