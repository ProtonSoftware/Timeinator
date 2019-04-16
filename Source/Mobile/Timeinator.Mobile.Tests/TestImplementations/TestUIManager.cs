using System;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Test class that implements <see cref="IUIManager"/>
    /// </summary>
    public class TestUIManager : IUIManager
    {
        #region Base UI Implementation

        public void ChangeLanguage(string langCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel)
        {
            return null;
        }

        public Task ExecuteOnMainThread(Action action)
        {
            action();
            return null;
        }

        public Task GoBackToPreviousPage(MvxViewModel currentVM)
        {
            return null;
        }

        public Task GoToViewModelPage(MvxViewModel viewModel)
        {
            return null;
        }

        #endregion
    }
}
