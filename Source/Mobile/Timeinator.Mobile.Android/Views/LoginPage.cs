
using Acr.UserDialogs;
using Android.App;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using System.Linq;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for LoginPageViewModel",
              NoHistory = true)]
    public class LoginPage : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginPage);

            // Initialize dialogs library
            UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);

            // Add dialogs library to Mvx DI
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);

            // If there is no DI setup yet, i.e. IUserTimeHandler is not injected
            if (!Dna.Framework.Construction.Services.Any(x => x.ServiceType == typeof(IUserTimeHandler) && x.ImplementationType == typeof(AndroidTimeHandler)))
            {
                // Add Android-specific dependency injection implementations
                Dna.Framework.Construction.Services.AddScoped<IUserTimeHandler, AndroidTimeHandler>();
                Dna.Framework.Construction.Services.AddSingleton<IUIManager, UIManager>();
            }

            // Build new DI
            Dna.Framework.Construction.Build();

            // If we get there by session intent from notification
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                // Change application's page to continue session
                DI.Application.GoToPage(ApplicationPage.TasksSession);
            }
        }
    }
}