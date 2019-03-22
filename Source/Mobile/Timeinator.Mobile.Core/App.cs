using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;

namespace Timeinator.Mobile.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
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

            // Set initial page
            RegisterAppStart<LoginPageViewModel>();
        }
    }
}
