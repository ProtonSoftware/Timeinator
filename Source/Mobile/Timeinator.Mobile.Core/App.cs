using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The main entry point for the application in MvvmCross environment
    /// </summary>
    public class App : MvxApplication
    {
        /// <summary>
        /// Override for initialization method that's called when App starts up
        /// </summary>
        public override void Initialize()
        {
            // Set initial page
            RegisterAppStart<LoginPageViewModel>();

            // Mvx specific stuff
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
        }
    }
}
