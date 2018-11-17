namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for a popup message that is shown to user and can accept the response
    /// </summary>
    public class PopupMessageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The title of this popup
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The message of this popup
        /// May ask a question or inform user about some errors etc.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to show on a button that user clicks if he accepts the question
        /// If set to null, the popup will not accept any response from the user
        /// </summary>
        public string AcceptButtonText { get; set; }

        /// <summary>
        /// The text to show on a button that user clicks if he declines
        /// By default, set to "OK" if the popup just informs the user and returns no response
        /// </summary>
        public string CancelButtonText { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor that creates a popup which just informs the user about something
        /// </summary>
        /// <param name="title">The required title of this popup</param>
        /// <param name="message">The message to show to the user</param>
        /// <param name="buttonText">The text to show on a button, default as OK</param>
        public PopupMessageViewModel(string title, string message, string buttonText = "Ok")
        {
            // Set provided values
            Title = title;
            Message = message;
            CancelButtonText = buttonText;
        }


        /// <summary>
        /// The constructor that creates a popup which asks user a question and returns the response
        /// </summary>
        /// <param name="title">The required title of this popup</param>
        /// <param name="message">The question to show to the user</param>
        /// <param name="acceptButtonText">The text to show on a button when user agrees</param>
        /// <param name="cancelButtonText">The text to show on a button when user declines</param>
        public PopupMessageViewModel(string title, string message, string acceptButtonText, string cancelButtonText)
        {
            // Set provided values
            Title = title;
            Message = message;
            AcceptButtonText = acceptButtonText;
            CancelButtonText = cancelButtonText;
        }

        #endregion
    }
}
