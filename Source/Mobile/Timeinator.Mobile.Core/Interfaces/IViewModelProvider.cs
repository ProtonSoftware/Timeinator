using MvvmCross.ViewModels;

namespace Timeinator.Mobile.Domain
{
    public interface IViewModelProvider
    {
        T GetInjectedPageViewModel<T>() where T : MvxViewModel;
    }
}
