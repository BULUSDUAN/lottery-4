using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.MobileApp.Views;
using Colin.Lottery.MobileApp.ViewModels;
using Colin.Lottery.Models;

namespace Colin.Lottery.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForcastPage : ContentPage
    {
        ForcastViewModel viewModel;

        public ForcastPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ForcastViewModel(DisplayAlert);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as JinMaForcastModel;
            if (item == null)
                return;

            await Navigation.PushAsync(new ForcastDetailPage(item.Rule.ToPk10Rule()));
            ((ListView)sender).SelectedItem = null;
        }

        async void BetAll_Clicked(object sender, EventArgs e)
        {
            //            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));

            await DisplayAlert("提醒", "此功能正在开发中，敬请期待", "再等等");
        }
    }
}