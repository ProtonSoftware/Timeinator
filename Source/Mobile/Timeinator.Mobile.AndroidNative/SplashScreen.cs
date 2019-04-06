
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    [Activity(
        Label = "Timeinator", 
        MainLauncher = true,
        NoHistory = true,
        Theme = "@style/AppTheme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenAppCompatActivity<MvxAppCompatSetup<App>, App>
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

            // Set back main application theme after the application is loaded and splash screen can disappear
            SetTheme(Resource.Style.AppTheme);
        }
    }
}