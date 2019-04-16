using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using System;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// A helper class to allow swipes on recycler view items
    /// </summary>
    public class SwipeRecyclerViewItemCallback : ItemTouchHelper.Callback
    {
        #region Private Members

        /// <summary>
        /// The adapter of recycler view
        /// </summary>
        private readonly RecyclerView.Adapter mAdapter;

        #endregion

        #region Public Events

        /// <summary>
        /// The event to fire when full swipe happens
        /// </summary>
        public event Action<int> OnSwipe = (s) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="adapter">The required adapter of recycled view</param>
        public SwipeRecyclerViewItemCallback(RecyclerView.Adapter adapter)
        {
            mAdapter = adapter;
        }

        #endregion

        #region Swipe Methods

        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            var swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(0, swipeFlags);
        }

        public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
        {
            mAdapter.NotifyItemMoved(p1.AdapterPosition, p2.AdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            OnSwipe.Invoke(p0.AdapterPosition);
            mAdapter.NotifyItemRemoved(p0.AdapterPosition);
        }

        #endregion
    }
}