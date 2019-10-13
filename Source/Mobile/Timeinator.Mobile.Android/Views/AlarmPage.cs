using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity]
    public class AlarmPage : MvxAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AlarmPage);

            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);

            Window.AddFlags(
                WindowManagerFlags.ShowWhenLocked |
                WindowManagerFlags.DismissKeyguard |
                WindowManagerFlags.KeepScreenOn |
                WindowManagerFlags.TurnScreenOn |
                WindowManagerFlags.AllowLockWhileScreenOn
                );
        }
    }
}