using RSMerauke.Pages;

namespace RSMerauke;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new pgSetting());
        MainPage.BindingContext = new pgSettingVM();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const int newWidth = 500;
        const int newHeight = 800;

        window.Width = newWidth;
        window.Height = newHeight;


        return window;
    }
}
