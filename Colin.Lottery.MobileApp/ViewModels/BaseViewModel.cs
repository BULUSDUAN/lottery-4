using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Colin.Lottery.MobileApp.Models;
using Colin.Lottery.MobileApp.Services;
using Colin.Lottery.Models;

namespace Colin.Lottery.MobileApp.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<JinMaForcastModel> DataStore =>
            DependencyService.Get<IDataStore<JinMaForcastModel>>() ?? new MockDataStore();

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private readonly Func<string, string, string, Task> _alert;

        protected BaseViewModel(Func<string, string, string, Task> alert)
        {
            _alert = alert;
        }

        protected BaseViewModel()
        {
        }

        protected void Alert(string title, string message, string cancel)
        {
            Device.BeginInvokeOnMainThread(() => _alert(title, message, cancel));
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}