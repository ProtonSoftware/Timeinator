using MvvmCross.ViewModels;
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

        void ChangeLanguage(string langCode);
    }
}
