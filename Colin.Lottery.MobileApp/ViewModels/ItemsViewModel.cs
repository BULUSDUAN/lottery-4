using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.MobileApp.Views;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Colin.Lottery.Models;

namespace Colin.Lottery.MobileApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "PK10连挂";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });


            Application.Current.Properties["ShowPlans"] = new Action<JArray>(ShowPlans);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //处理接收到的消息
        void ShowPlans(JArray planArray)
        {
            //var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(planArray.ToString());

            //if (IsBusy)
            //    return;

            //IsBusy = true;

            //try
            //{
            //    Items.Clear();
            //    var plans = JsonConvert.DeserializeObject<List<JinMaForcastModel>>(planArray.ToString());
            //    foreach (var plan in plans)
            //    {
            //        Items.Add(plan);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex);
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }
    }
}