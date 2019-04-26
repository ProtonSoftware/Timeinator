using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
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

        private ColorDrawable mBackground;
        private Drawable mIcon;

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

            mBackground = new ColorDrawable(Color.Red);
            mIcon = Application.Context.GetDrawable(Resource.Drawable.ic_delete_black_18dp);
            mIcon.SetTint(Resource.Color.colorWhite);
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

        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);

            var itemView = viewHolder.ItemView;
            var backgroundCornerOffset = 10;

            var iconMargin = (itemView.Height - mIcon.IntrinsicHeight) / 4;
            var iconTop = itemView.Top + (itemView.Height - mIcon.IntrinsicHeight) / 2;
            var iconBottom = iconTop + mIcon.IntrinsicHeight;
            var iconOffset = iconMargin + mIcon.IntrinsicWidth;

            // Swiping to the right
            if (dX > 0)
            {
                var iconLeft = itemView.Left + iconOffset;
                var iconRight = itemView.Left + iconMargin;
                mIcon.SetBounds(iconLeft, iconTop, iconRight, iconBottom);

                mBackground.SetBounds(itemView.Left, itemView.Top,
                        itemView.Left + ((int)dX) + backgroundCornerOffset,
                        itemView.Bottom);
            }

            // Swiping to the left
            else if (dX < 0)
            {
                var iconLeft = itemView.Right - iconOffset;
                var iconRight = itemView.Right - iconMargin;
                mIcon.SetBounds(iconLeft, iconTop, iconRight, iconBottom);

                mBackground.SetBounds(itemView.Right + ((int)dX) - backgroundCornerOffset,
                        itemView.Top, itemView.Right, itemView.Bottom);
            }

            // No swipe
            else
            { 
                mBackground.SetBounds(0, 0, 0, 0);
            }

            // Always draw the background
            mBackground.Draw(c);

            // If item is moved far enough in any direction...
            if (dX > iconOffset || dX < -1 * iconOffset)
                // Then draw the icon
                mIcon.Draw(c);
        }

        #endregion
    }
}