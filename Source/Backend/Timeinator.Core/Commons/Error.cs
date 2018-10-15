namespace Timeinator.Core
{
    /// <summary>
    /// The generic error that can happen in this application
    /// </summary>
    public class Error
    {
        #region Public Properties

        /// <summary>
        /// The classification of this error
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// The identity of this error
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// The human-readable message of this error
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override default string conversion to display error in nice format
        /// </summary>
        public override string ToString() => string.Format("{0} {1} {2}", Classification, Identification, Message);

        #endregion
    }
}
