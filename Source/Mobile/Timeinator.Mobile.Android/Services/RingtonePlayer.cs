using Android.App;
using Android.Media;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    public class RingtonePlayer : IRingtonePlayer
    {
        #region Private members

        private Ringtone mSound;

        #endregion

        #region Constructor

        public RingtonePlayer()
        {
            mSound = RingtoneManager.GetRingtone(Application.Context, RingtoneManager.GetDefaultUri(RingtoneType.Alarm));
        }

        #endregion

        #region Interface implementation

        public void Play() => mSound.Play();

        public void Stop() => mSound.Stop();

        public void ChangeRingtone(string type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}