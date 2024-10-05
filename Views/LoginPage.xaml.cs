using C971.Services;

namespace C971.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public LoginPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (UsernameEntry == null || PasswordEntry == null || LoginStatusLabel == null)
                {
                    await DisplayAlert("Error", "One or more UI elements are not initialized.", "OK");
                    return;
                }

                string username = UsernameEntry?.Text;
                string password = PasswordEntry?.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    LoginStatusLabel.Text = "Please enter both username and password.";
                    LoginStatusLabel.IsVisible = true;
                    return;
                }

                var user = await _databaseService.GetUser(username, password);
                if (user != null)
                {
                    await DisplayAlert("Login Successful", $"Welcome, {username}!", "OK");
                    await Navigation.PushAsync(new Dashboard());
                }
                else
                {
                    LoginStatusLabel.Text = "Invalid credentials, please try again.";
                    LoginStatusLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
