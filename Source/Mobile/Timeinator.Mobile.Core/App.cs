using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;

namespace Timeinator.Mobile.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            RegisterAppStart<LoginPageViewModel>();
        }
    }
}
