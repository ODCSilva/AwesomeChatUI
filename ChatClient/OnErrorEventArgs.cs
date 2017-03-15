namespace Chat {

    /// <summary>
    /// OnErrorEventArgs class.
    /// </summary>
    public class OnErrorEventArgs {

        #region Constructors

        /// <summary>
        /// Initializes an instance of the OnErrorEventArgs class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public OnErrorEventArgs(ChatError type, string message) {
            ErrorType = type;
            ErrorMessage = message;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// The type of error returned.
        /// </summary>
        public ChatError ErrorType { get; }

        #endregion Properties
    }
}