using MvvmCross.ViewModels;
using System;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// All the informations about a single MvxFragment with view model support
    /// </summary>
    public class MvxFragmentInfo
    {
        /// <summary>
        /// The title of the fragment to display
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The icon for this fragment as resource id
        /// </summary>
        public int IconResourceId { get; set; }

        /// <summary>
        /// The type of the fragment
        /// </summary>
        public Type FragmentType { get; set; }

        /// <summary>
        /// The view model to use in the fragment
        /// </summary>
        public IMvxViewModel ViewModel { get; set; }
    }
}