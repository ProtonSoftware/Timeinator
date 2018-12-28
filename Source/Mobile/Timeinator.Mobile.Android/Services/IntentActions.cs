namespace Timeinator.Mobile.Droid
{
    public class IntentActions
    {
        public const string ACTION_NOTHING = "com.gummybearstudio.NOTHING";
        public const string ACTION_SHOW = "com.gummybearstudio.SHOW";
        public const string ACTION_GOSESSION = "com.gummybearstudio.GOSESSION";
        public const string ACTION_NEXTTASK = "com.gummybearstudio.NEXTTASK";
        public const string ACTION_PAUSETASK = "com.gummybearstudio.PAUSETASK";
        public const string ACTION_RESUMETASK = "com.gummybearstudio.RESUMETASK";

        public static string FromEnum(NotificationAction e)
        {
            switch (e)
            {
                case NotificationAction.DoNothing:
                    return ACTION_SHOW;
                case NotificationAction.GoToSession:
                    return ACTION_GOSESSION;
                case NotificationAction.NextSessionTask:
                    return ACTION_NEXTTASK;
                case NotificationAction.PauseSession:
                    return ACTION_PAUSETASK;
                case NotificationAction.ResumeSession:
                    return ACTION_RESUMETASK;
            }
            return ACTION_NOTHING;
        }
    }
}