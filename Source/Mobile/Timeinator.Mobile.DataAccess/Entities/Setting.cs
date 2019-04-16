using Timeinator.Core;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The single setting for this application saved in database
    /// </summary>
    public class Setting : BaseObject<int>
    {
        /// <summary>
        /// The name of this setting property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of this setting such as bool/int/string etc. as string
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The value of this setting as string
        /// </summary>
        public string Value { get; set; }
    }
}
