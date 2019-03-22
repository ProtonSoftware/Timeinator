using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Manages every UI interactions
    /// </summary>
    public interface IUIManager
    {
        Task GoBackToPreviousPage(MvxViewModel currentVM);

        Task GoToViewModelPage(MvxViewModel viewModel);

        Task<bool> DisplayPopupMessageAsync(PopupMessageViewModel viewmodel);

        Task ExecuteOnMainThread(Action action);

        void ChangeLanguage(string langCode);
    }
}
