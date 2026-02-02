namespace CoffeeShop.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(PageModels.SettingsPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
