using Acr.UserDialogs;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using SimpleInjector;
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

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.LoginPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            // Set application's font to Lato
            _ = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Regular.ttf");

            // Initialize dialogs library
            UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);

            // Add dialogs library to Mvx DI
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);

            // Add Android-specific dependency injection implementations
            DI.Container.Register<IUIManager, UIManager>(Lifestyle.Singleton);
            DI.Container.Register<SessionNotificationService>(Lifestyle.Singleton);
            DI.Container.Register<IRingtonePlayer, RingtonePlayer>(Lifestyle.Singleton);

            // Migrate the database
            DI.MigrateDatabase();

            // If we get there by session intent from notification
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                // Change application's page to continue session
                DI.Application.GoToPage(ApplicationPage.TasksSession);
            }
            else
            {
                // For now the application doesn't have login features, so simply go to the next page
                DI.Application.GoToPage(ApplicationPage.TasksList);
            }
        }
    }
}