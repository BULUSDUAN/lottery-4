using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Colin.Lottery.MobileApp.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Colin.Lottery.MobileApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();


            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
