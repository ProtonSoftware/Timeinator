using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using System;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// A helper class to allow movements on recycler view items
    /// </summary>
    public class MovementRecyclerViewItemCallback : ItemTouchHelper.Callback
    {
        #region Private Members

        /// <summary>
        /// The adapter of recycler view
        /// </summary>
        private readonly RecyclerView.Adapter mAdapter;

        #endregion

        #region Public Events

        /// <summary>
        /// The event to fire when an item moves above another one
        /// </summary>
        public event Action<int, int> OnMovement = (to, from) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="adapter">The required adapter of recycled view</param>
        public MovementRecyclerViewItemCallback(RecyclerView.Adapter adapter)
        {
            mAdapter = adapter;
        }

        #endregion

        #region Movement Methods

        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            var dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            return MakeMovementFlags(dragFlags, 0);
        }

        public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
        {
            mAdapter.NotifyItemMoved(p1.AdapterPosition, p2.AdapterPosition);
            return true;
        }

        public override void OnMoved(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, int fromPos, RecyclerView.ViewHolder target, int toPos, int x, int y)
        {
            OnMovement.Invoke(fromPos, toPos);
            base.OnMoved(recyclerView, viewHolder, fromPos, target, toPos, x, y);
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Don't handle swipes since we care only about movements
            return;
        }

        #endregion
    }
}