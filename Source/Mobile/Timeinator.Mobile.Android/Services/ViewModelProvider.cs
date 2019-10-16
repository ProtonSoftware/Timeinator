using MvvmCross.ViewModels;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    public class ViewModelProvider : IViewModelProvider
    {
        public T GetInjectedPageViewModel<T>() where T : MvxViewModel => DI.Container.GetInstance<T>();
    }
}