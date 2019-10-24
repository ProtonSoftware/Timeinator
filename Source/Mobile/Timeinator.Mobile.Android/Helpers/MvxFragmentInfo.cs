using MvvmCross.ViewModels;
using System;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// All the informations about a single MvxFragment with view model support
    /// </summary>
    public class MvxFragmentInfo
    {
        #region Public Properties

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

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for generic fragments
        /// </summary>
        public MvxFragmentInfo() { }

        /// <summary>
        /// Constructor for time task types fragments
        /// </summary>
        /// <param name="taskType">The task's type for this fragment to create</param>
        /// <param name="viewModel">The view model for this fragment</param>
        public MvxFragmentInfo(TimeTaskType taskType, IMvxViewModel viewModel)
        {
            Title = taskType.ToString();
            IconResourceId = taskType.ToIcon();
            FragmentType = typeof(AddNewTimeTaskFragment);
            ViewModel = viewModel;
        }

        #endregion
    }
}