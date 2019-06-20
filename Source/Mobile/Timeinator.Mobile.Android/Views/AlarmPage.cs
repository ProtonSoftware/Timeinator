
using Android.App;
using Android.Content;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Timeinator.Mobile.Android
{
    [MvxActivityPresentation]
    [Activity(Label = "View for AlarmPageViewModel")]
    public class AlarmPage : MvxActivity
    {
        private Ringtone mAlarmSound;

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

            // Play ringtone and store it for later cancellation
            mAlarmSound = RingtoneManager.GetRingtone(ApplicationContext, RingtoneManager.GetDefaultUri(RingtoneType.Alarm));
            mAlarmSound.Play();
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            mAlarmSound.Stop();
            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnDestroy()
        {
            // Does not get called - FIX ME
            mAlarmSound.Stop();
            base.OnDestroy();
        }
    }
}