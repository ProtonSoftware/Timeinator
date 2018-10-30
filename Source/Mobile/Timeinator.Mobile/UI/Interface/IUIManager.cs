using System.Threading.Tasks;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Manages every UI interactions
    /// </summary>
    public interface IUIManager
    {
        Task ShowModalOnCurrentNavigation(ContentPage page);

        Task HideRecentModalFromCurrentNavigation();

        void HideMenu();
    }
}
