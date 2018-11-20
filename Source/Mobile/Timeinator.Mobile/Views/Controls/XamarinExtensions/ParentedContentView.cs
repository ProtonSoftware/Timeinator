using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// An extension to the <see cref="ContentView"/> allowing it to store it's parent BindingContext to use
    /// </summary>
    public class ParentedContentView : ContentView
    {
        /// <summary>
        /// The BindingContext of the parent of this view
        /// </summary>
        public object ParentContext
        {
            get => GetValue(ParentContextProperty);
            set => SetValue(ParentContextProperty, value);
        }

        // Using a DependencyProperty as the backing store for ParentContext.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty ParentContextProperty =
            BindableProperty.Create(nameof(ParentContext), typeof(object), typeof(ParentedContentView));
    }
}
