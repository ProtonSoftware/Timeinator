namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The player for ringtone sound
    /// </summary>
    public interface IRingtonePlayer
    {
        /// <summary>
        /// Plays the sound
        /// </summary>
        void Play();

        /// <summary>
        /// Stops the current sound from playing
        /// </summary>
        void Stop();

        /// <summary>
        /// Changes current sound to specified type
        /// </summary>
        /// <param name="type">The type to change sound to</param>
        void ChangeRingtone(string type);
    }
}
