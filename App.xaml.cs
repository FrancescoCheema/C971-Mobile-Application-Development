using C971.Models;
using C971.Services;
using C971.Views;
using Microsoft.Maui.Controls;


namespace C971
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService;

        public App()
        {
            InitializeComponent();

            _databaseService = new DatabaseService();

            InitializeAppAsync().ConfigureAwait(false);

            SeedUsers().ConfigureAwait(false);


            MainPage = new NavigationPage(new LoginPage());
        }

        private async Task InitializeAppAsync()
        {
            try
            {
                await _databaseService.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        public async Task SeedUsers()
        {
            await _databaseService.Init();

            await _databaseService.AddUser("testuser", "password123");

            var users = await _databaseService.GetAllUsers();

            if(users == null)
            {
                Console.WriteLine("user does not exist");
            }
        }
    }
}
