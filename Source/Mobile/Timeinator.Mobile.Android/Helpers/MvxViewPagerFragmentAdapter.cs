using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using DK.Ostebaronen.Droid.ViewPagerIndicator.Interfaces;
using Java.Lang;
using MvvmCross.Droid.Support.V4;
using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The adapter that supports MvxFragments and allows view pager to display provided fragments
    /// </summary>
    public class MvxFragmentStatePagerAdapter : FragmentStatePagerAdapter, IIconPageAdapter
    {
        #region Private Members

        /// <summary>
        /// The Android context for this application
        /// </summary>
        private readonly Context mContext;

        /// <summary>
        /// The list of fragments that this adapter displays
        /// </summary>
        private IEnumerable<MvxFragmentInfo> mFragments;

        #endregion

        #region Constructors

        /// <summary>
        /// The constructor thats not used in code, just required by Android
        /// </summary>
        protected MvxFragmentStatePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        /// <summary>
        /// The constructor that should be used to create this adapter
        /// </summary>
        /// <param name="context">The Android context</param>
        /// <param name="fragmentManager">The fragment manager that supports AppCompat</param>
        /// <param name="fragments">The fragments themselves to display</param>
        public MvxFragmentStatePagerAdapter(Context context, FragmentManager fragmentManager, IEnumerable<MvxFragmentInfo> fragments) : base(fragmentManager)
        {
            mContext = context;
            mFragments = fragments;
        }

        #endregion

        #region Android Adapter Override

        /// <summary>
        /// The amount of fragments, needed for base class
        /// </summary>
        public override int Count => mFragments.Count();

        /// <summary>
        /// Gets a single fragment item from the list
        /// It is used by the view pager automatically
        /// </summary>
        /// <param name="position">The position of the fragment</param>
        /// <returns>The fragment as Android default implementation</returns>
        public override Fragment GetItem(int position)
        {
            // Get the fragment info from underlying list
            var fragmentInfo = mFragments.ElementAt(position);

            // Create the fragment itself
            var fragment = Fragment.Instantiate(mContext, FragmentJavaName(fragmentInfo.FragmentType));

            // Set the view model of the fragment to use the one from info
            ((MvxFragment)fragment).ViewModel = fragmentInfo.ViewModel;

            if (fragment is AddNewTimeTaskFragment timeTaskFragment)
            {
                timeTaskFragment.Type = (TimeTaskType)System.Enum.Parse(typeof(TimeTaskType), fragmentInfo.Title);
            }

            // Return the fragment
            return fragment;
        }

        /// <summary>
        /// Gets page title based on the fragment info
        /// It is used by the view pager automatically
        /// </summary>
        /// <param name="position">The position of the fragment</param>
        /// <returns>Java's implementation of string</returns>
        public override ICharSequence GetPageTitleFormatted(int position) => new Java.Lang.String(mFragments.ElementAt(position).Title);

        /// <summary>
        /// A helper to get fragment's name in Java formatted style
        /// </summary>
        /// <param name="fragmentType">The type of fragment to get name from</param>
        /// <returns>Simple string</returns>
        protected static string FragmentJavaName(Type fragmentType) => Class.FromType(fragmentType).Name;

        /// <summary>
        /// Gets the icon resource from fragment at specified position
        /// </summary>
        /// <param name="index">The position of a fragment</param>
        /// <returns>Resource id</returns>
        public int GetIconResId(int index) => mFragments.ElementAt(index).IconResourceId;

        #endregion
    }
}