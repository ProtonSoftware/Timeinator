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

        /// <inheritdoc />
        public void Play() => mSound.Play();

        /// <inheritdoc />
        public void Stop() => mSound.Stop();

        /// <inheritdoc />
        public void ChangeRingtone(string type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}