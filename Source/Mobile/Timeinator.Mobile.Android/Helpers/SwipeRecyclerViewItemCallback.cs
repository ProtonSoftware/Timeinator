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

        /// <summary>
        /// The background for delete swipe option
        /// </summary>
        private readonly ColorDrawable mDeleteBackground = Application.Context.GetDrawable(Resource.Color.colorPrimary) as ColorDrawable;

        /// <summary>
        /// The background for edit swipe option
        /// </summary>
        private readonly ColorDrawable mEditBackground = new ColorDrawable(Color.ForestGreen);

        /// <summary>
        /// The icon to show on top of the delete background
        /// </summary>
        private readonly Drawable mDeleteIcon = Application.Context.GetDrawable(Resource.Drawable.ic_delete_white_24dp);

        /// <summary>
        /// The icon to show on top of the edit background
        /// </summary>
        private readonly Drawable mEditIcon = Application.Context.GetDrawable(Resource.Drawable.ic_edit_white_24dp);

        /// <summary>
        /// Helper to organise data about drawable object bounds
        /// </summary>
        private class DrawableBoundsData
        {
            #region Public Properties

            public Drawable DrawableObject { get; set; }

            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor which just simply fills all the data
            /// </summary>
            public DrawableBoundsData(Drawable drawable, int left, int top, int right, int bottom)
            {
                DrawableObject = drawable;

                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            /// <summary>
            /// Constructor with only drawable object provided
            /// </summary>
            public DrawableBoundsData(Drawable drawable)
            {
                DrawableObject = drawable;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Set bounds on drawable object and draws it on specified canvas
            /// </summary>
            public void BoundAndDraw(Canvas canvas)
            {
                DrawableObject.SetBounds(Left, Top, Right, Bottom);
                DrawableObject.Draw(canvas);
            }

            #endregion
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The event to fire when full swipe happens
        /// </summary>
        public event Action<int, bool> OnSwipe = (s, direction) => { };

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

        /// <summary>
        /// Gets allowed movement directions
        /// In Swipe, we allow only Left-Right movements
        /// </summary>
        public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
        {
            // Allow left and right movements
            var swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(0, swipeFlags);
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
        /// Fired when item is fully swiped
        /// </summary>
        public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
        {
            // Notify that the item has changed so the UI will place it in initial position
            mAdapter.NotifyItemChanged(p0.AdapterPosition);

            // Experimental - Debugging always shows p1 = 16 when swiping to the right for some reason
            //                So I assume its the correct value, but its not really safe to just do that
            //                So if any bugs happen, fix there
            OnSwipe.Invoke(p0.AdapterPosition, p1 == 16);

            // Don't notify about removal here, because user can cancel it
            // Just a note that it was there initially and may be needed in the future
            //mAdapter.NotifyItemRemoved(p0.AdapterPosition);
        }

        /// <summary>
        /// Called everytime the item moves any pixel and needs to be redrawn
        /// NOTE: This may be called several hundreds of times per second
        /// </summary>
        public override void OnChildDraw(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            // Do all the base stuff this function needs to properly show the moved item
            base.OnChildDraw(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);

            // Get the view of current moved item
            var itemView = viewHolder.ItemView;

            // Set background offset for better visual appearance 
            var backgroundCornerOffset = 10;

            // Prepare data variables for drawable bounds
            var backgroundData = new DrawableBoundsData(mDeleteBackground, 0, itemView.Top, 0, itemView.Bottom);
            var iconData = default(DrawableBoundsData);
            var offsetIsEnough = false;

            // Swiping to the right (Edit)
            if (dX > 0)
            {
                // Change background to edit one
                backgroundData.DrawableObject = mEditBackground;

                // Set background bounds
                backgroundData.Left = itemView.Left;
                backgroundData.Right = itemView.Left + ((int)dX) + backgroundCornerOffset;

                // Change icon to edit one
                iconData = new DrawableBoundsData(mEditIcon);

                // Calculate icon data
                var iconMargin = (itemView.Height - iconData.DrawableObject.IntrinsicHeight) / 4;
                var iconOffset = iconMargin + iconData.DrawableObject.IntrinsicWidth;
                offsetIsEnough = dX > iconOffset;

                // Set icon bounds
                iconData.Top = itemView.Top + (itemView.Height - iconData.DrawableObject.IntrinsicHeight) / 2;
                iconData.Bottom = iconData.Top + iconData.DrawableObject.IntrinsicHeight;
                iconData.Left = itemView.Left + iconOffset;
                iconData.Right = itemView.Left + iconMargin;
            }

            // Swiping to the left (Delete) 
            else if (dX < 0)
            {
                // Set background bounds (Delete background is set by default so no need to change it)
                backgroundData.Left = itemView.Right + ((int)dX) - backgroundCornerOffset;
                backgroundData.Right = itemView.Right;

                // Change icon to delete one
                iconData = new DrawableBoundsData(mDeleteIcon);

                // Calculate icon data
                var iconMargin = (itemView.Height - iconData.DrawableObject.IntrinsicHeight) / 4;
                var iconOffset = iconMargin + iconData.DrawableObject.IntrinsicWidth;
                offsetIsEnough = dX < -1 * iconOffset;

                // Set icon bounds
                iconData.Top = itemView.Top + (itemView.Height - iconData.DrawableObject.IntrinsicHeight) / 2;
                iconData.Bottom = iconData.Top + iconData.DrawableObject.IntrinsicHeight;
                iconData.Left = itemView.Right - iconOffset;
                iconData.Right = itemView.Right - iconMargin;
            }

            // Always draw current background on the screen
            backgroundData.BoundAndDraw(c);

            // If item is moved far enough in any direction...
            if (offsetIsEnough)
                // Then draw the icon
                iconData.BoundAndDraw(c);
        }

        #endregion
    }
}