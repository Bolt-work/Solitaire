using SolitaireApp.Views;

namespace SolitaireApp
{
    public partial class App : Application
    {
        public App(GamePage gamePage)
        {
            InitializeComponent();

            MainPage = gamePage;
        }
    }
}
