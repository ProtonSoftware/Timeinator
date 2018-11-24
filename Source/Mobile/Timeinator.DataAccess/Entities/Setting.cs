using Timeinator.Core;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The single setting for this application saved in database
    /// </summary>
    public class Setting : BaseObject<int>
    {
        /// <summary>
        /// Name of the setting property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of this setting as object
        /// </summary>
        public object Value { get; set; }
    }
}
