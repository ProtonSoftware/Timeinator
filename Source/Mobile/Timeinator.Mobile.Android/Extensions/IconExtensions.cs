using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The extensions for everything that can be converted to icons
    /// </summary>
    public static class IconExtensions
    {
        public static int ToIcon(this TimeTaskType taskType)
        {
            // Return associated icon based on provided type
            switch (taskType)
            {
                case TimeTaskType.Generic:
                    return Resource.Drawable.icon_type_generic;

                case TimeTaskType.Reading:
                    return Resource.Drawable.icon_type_reading;

                default:
                    return -1;
            }
        }

        public static int ToIcon(this Priority taskType)
        {
            // Return associated icon based on provided priority value
            switch (taskType)
            {
                case Priority.One:
                    return Resource.Drawable.icon_type_generic;

                case Priority.Two:
                    return Resource.Drawable.icon_type_reading;

                default:
                    return -1;
            }
        }
    }
}