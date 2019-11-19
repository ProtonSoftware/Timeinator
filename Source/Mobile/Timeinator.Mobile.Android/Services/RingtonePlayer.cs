using Android.App;
using Android.Media;
using System;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The player for ringtone sound in Android
    /// </summary>
    public class RingtonePlayer : IRingtonePlayer
    {
        #region Private Members

        /// <summary>
        /// The sound to use when playing
        /// </summary>
        private Ringtone mSound = RingtoneManager.GetRingtone(Application.Context, RingtoneManager.GetDefaultUri(RingtoneType.Ringtone));

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Implements <see cref="IRingtonePlayer.Play"/>
        /// </summary>
        public void Play() => mSound.Play();

        /// <summary>
        /// Implements <see cref="IRingtonePlayer.Stop"/>
        /// </summary>
        public void Stop() => mSound.Stop();

        /// <summary>
        /// Implements <see cref="IRingtonePlayer.ChangeRingtone(string)"/>
        /// </summary>
        public void ChangeRingtone(string type)
        {
            // TODO: Implement this sometime soon
            throw new NotImplementedException();
        }

        #endregion
    }
}