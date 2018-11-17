using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The base class for every page view model in this app
    /// </summary>
    public class BasePageViewModel : BaseViewModel
    {
        #region Private Members

        private bool mIsBusy = false;

        private string mTitle = string.Empty;

        #endregion

        #region Public Properties
        
        public bool IsBusy
        {
            get => mIsBusy;
            set => SetProperty(ref mIsBusy, value);
        }

        public string Title
        {
            get => mTitle;
            set => SetProperty(ref mTitle, value);
        }

        #endregion

        #region Xamarin Default Stuff

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
