using MvvmCross.ViewModels;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The helper that provides view models for every page
    /// </summary>
    public class ViewModelProvider : IViewModelProvider
    {
        /// <summary>
        /// Gets the view model for provided page type with every needed DI service injected
        /// </summary>
        /// <typeparam name="T">The type of page's view model</typeparam>
        /// <returns>The ready view model</returns>
        public T GetInjectedPageViewModel<T>() where T : MvxViewModel => DI.Container.GetInstance<T>();
    }
}