using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Manages ringtone sound playback
    /// </summary>
    public interface IRingtonePlayer
    {
        void Play();

        void Stop();

        void ChangeRingtone(string type);
    }
}
