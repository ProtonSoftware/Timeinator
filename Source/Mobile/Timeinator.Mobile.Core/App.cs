using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Timeinator.Mobile.Core
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
            // Run the initialization on a different thread
            // So it doesn't block UI thread
            // And as a result the App will startup faster
            Task.Run(() =>
            {
                // Set initial page
                RegisterAppStart<LoginPageViewModel>();

                // Mvx specific stuff
                CreatableTypes()
                    .EndingWith("Service")
                    .AsInterfaces()
                    .RegisterAsLazySingleton();

                // If there is no DI setup yet
                if (Dna.Framework.Construction == null)
                {
                    // Setup brand-new DI
                    DI.InitialSetup();
                }
            });
        }
    }
}
