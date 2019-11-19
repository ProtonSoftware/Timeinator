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

        /// <summary>
        /// Gets allowed movement directions
        /// We allow only Up-Down movements
        /// </summary>
        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            // Allow up and down movements
            var dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            return MakeMovementFlags(dragFlags, 0);
        }

        /// <summary>
        /// Called whenever an item is being moved
        /// </summary>
        public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
        {
            // Inform adapter
            mAdapter.NotifyItemMoved(p1.AdapterPosition, p2.AdapterPosition);

            // Return successful move operation
            return true;
        }

        /// <summary>
        /// Called whenever an item has been moved to different position in the list
        /// </summary>
        public override void OnMoved(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, int fromPos, RecyclerView.ViewHolder target, int toPos, int x, int y)
        {
            // Fire on movement event to inform about item movement
            OnMovement.Invoke(fromPos, toPos);

            // Do base stuff
            base.OnMoved(recyclerView, viewHolder, fromPos, target, toPos, x, y);
        }

        /// <summary>
        /// Called whenever an item was fully swiped
        /// </summary>
        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Don't handle swipes since we care only about movements in this helper
            return;
        }

        #endregion
    }
}