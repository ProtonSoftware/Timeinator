using Android.App;
using Android.Media;
using System;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    public class RingtonePlayer : IRingtonePlayer
    {
        #region Private Members

        private readonly Ringtone mSound = RingtoneManager.GetRingtone(Application.Context, RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));

        #endregion

        #region Interface Implementation

        public void Play() => mSound.Play();

        public void Stop() => mSound.Stop();

        public void ChangeRingtone(string type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}