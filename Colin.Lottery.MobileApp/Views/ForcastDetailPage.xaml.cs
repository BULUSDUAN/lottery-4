using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.MobileApp.ViewModels;
using Colin.Lottery.Models;

namespace Colin.Lottery.MobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForcastDetailPage : ContentPage
    {
        ForcastDetailViewModel viewModel;

        public ForcastDetailPage(Pk10Rule rule)
        {
            InitializeComponent();
            
            BindingContext = this.viewModel = new ForcastDetailViewModel(rule,lv,DisplayAlert);
        }
    }
}