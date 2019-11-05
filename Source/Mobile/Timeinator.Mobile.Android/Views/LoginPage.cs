using Acr.UserDialogs;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SimpleInjector;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(NoHistory = true)]
    public class LoginPage : MvxAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // If we get there by session intent from notification
            if (Intent.Action == IntentActions.ACTION_GOSESSION)
            {
                // Change application's page to continue session
                DI.Container.GetInstance<ApplicationViewModel>().GoToPage(ApplicationPage.TasksSession);
                return;
            }

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.LoginPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            // Set application's font to Lato
            _ = Typeface.CreateFromAsset(Application.Context.Assets, "fonts/Lato-Regular.ttf");

            // Initialize dialogs library
            UserDialogs.Init(() => Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity);

            // Add dialogs library to Mvx DI
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);

            // Create new container
            DI.Container = new Container().AddTimeinatorServices();

            // Migrate the database
            DI.MigrateDatabase();

            // Create settings provider instance to load every setting initially
            DI.Container.GetInstance<ISettingsProvider>();

            // For now the application doesn't have login features, so simply go to the next page
            DI.Container.GetInstance<ApplicationViewModel>().GoToPage(ApplicationPage.TasksList);
        }
    }
}