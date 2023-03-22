using HttpListenerMaui.Services;

namespace HttpListenerMaui;

public partial class MainPage : ContentPage
{
	HttpListenerService listener = new HttpListenerService();
	public MainPage()
	{
		InitializeComponent();
		Task.Run(() => listener.Listen());
	}
}
