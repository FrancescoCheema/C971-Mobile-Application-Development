using C971.Views;

namespace C971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var dashBoard = new Dashboard();
            var navPage = new NavigationPage(dashBoard);
            MainPage = navPage;
        }
    }
}