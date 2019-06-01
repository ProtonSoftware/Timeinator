using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Droid.Support.V7.AppCompat;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    [Activity(
        Label = "Timeinator", 
        MainLauncher = true,
        NoHistory = true,
        Theme = "@style/AppTheme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenAppCompatActivity<MvxAppCompatSetup<App>, App>
    {
    }
}