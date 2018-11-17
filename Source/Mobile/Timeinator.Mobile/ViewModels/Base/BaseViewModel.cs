using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The base class for every view model to derive from
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// The event that is fired whenever any property in view model changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fired automatically whenever any property changes
        /// Fires the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">The name of a property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // Make sure that event exists
            if (PropertyChanged == null)
                return;

            // Fire the event
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
