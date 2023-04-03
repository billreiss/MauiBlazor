using Microsoft.Identity.Client;
using MsalAuthInMaui.MsalClient;

namespace MsalAuthInMaui;

public partial class MainPage : ContentPage
{
    private string _accessToken = null;

    public string AccessToken
    {
        get 
        {
            if (_accessToken == null)
            {
                _accessToken = Preferences.Default.Get("accessToken", string.Empty);
            }
            return _accessToken; 
        }
        set 
        { 
            _accessToken = value;
            Preferences.Default.Set("accessToken", _accessToken);
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(AccessToken));
        }

    }
    public bool IsLoggedIn
    {
        get => !string.IsNullOrWhiteSpace(AccessToken);
        set
        {
            if (!value)
            {
                AccessToken = String.Empty;
            }
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(ButtonCaption));
        }
    }

    public string ButtonCaption
    {
        get
        {
            return "Get Weather Forecast";
        }
    }
    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        await Login().ConfigureAwait(false);
    }

    private async Task Login()
    {
        try
        {
            // Attempt silent login, and obtain access token.
            var result = await PCAWrapper.Instance.AcquireTokenSilentAsync(PCAWrapper.Scopes).ConfigureAwait(false);

            // Set access token.
            AccessToken = result.AccessToken;

            // Display Access Token from AcquireTokenSilentAsync call.
            await ShowOkMessage("Access Token from AcquireTokenSilentAsync call", AccessToken).ConfigureAwait(false);
        }
        // A MsalUiRequiredException will be thrown, if this is the first attempt to login, or after logging out.
        catch (MsalUiRequiredException)
        {
            // Perform interactive login, and obtain access token.
            var result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(PCAWrapper.Scopes).ConfigureAwait(false);
            IsLoggedIn = true;

            // Set access token.
            AccessToken = result.AccessToken;

            // Display Access Token from AcquireTokenInteractiveAsync call.
            await ShowOkMessage("Success", "Sucessfully logged in.").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            IsLoggedIn = false;
            await ShowOkMessage("Exception in AcquireTokenSilentAsync", ex.Message).ConfigureAwait(false);
        }
    }

    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        // Log out.
        _ = await PCAWrapper.Instance.SignOutAsync().ContinueWith(async (t) =>
        {
            await ShowOkMessage("Signed Out", "Sign out complete.").ConfigureAwait(false);
            IsLoggedIn = false;
        }).ConfigureAwait(false);
    }

    private async void OnGetWeatherForecastButtonClicked(object sender, EventArgs e)
    {
        // Call the Secure Web API to get the weatherforecast data.
        var weatherForecastData = await CallSecureWebApi().ConfigureAwait(false);

        // Show the data.
        if (weatherForecastData != string.Empty)
            await ShowOkMessage("WeatherForecast data", weatherForecastData).ConfigureAwait(false);
    }

    // Call the Secure Web API.
    private async Task<string> CallSecureWebApi()
    {
        try
        {
            // Get the weather forecast data from the Secure Web API.
            var client = new HttpClient();

            // Send the request.
            bool retry = true;
            HttpResponseMessage response = null;
            while (retry)
            {

                if (!string.IsNullOrWhiteSpace(AccessToken))
                {
                    // Create the request.
                    var message = new HttpRequestMessage(HttpMethod.Get, "https://msalmauitestapi.azurewebsites.net/weatherforecast");

                    // Add the Authorization Bearer header.
                    message.Headers.Add("Authorization", $"Bearer {AccessToken}");

                    response = await client.SendAsync(message).ConfigureAwait(false);
                }
                if (string.IsNullOrWhiteSpace(AccessToken) || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // If we got here, either the access token does not exist, or it was invalid, possibly expired. Let's log in again.
                    await Login();
                }
                else
                {
                    retry = false;
                }
            }

            if (response != null)
            {
                // Get the response.
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // Return the response.
                return responseString;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    private Task ShowOkMessage(string title, string message)
    {
        _ = Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert(title, message, "OK").ConfigureAwait(false);
        });
        return Task.CompletedTask;
    }
}