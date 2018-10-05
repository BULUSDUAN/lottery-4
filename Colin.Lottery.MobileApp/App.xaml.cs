using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;

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

        protected async override void OnStart()
        {
            // Handle when your app starts

            var connection = new HubConnectionBuilder()
                  .WithUrl("http://localhost/hubs/pk10")
                  .Build();

            connection.On<JArray>("ShowPlans", Application.Current.Properties["ShowPlans"] as Action<JArray>);
            connection.Closed += async (error) => await connection.StartAsync();

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("GetAppNewForcast");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
