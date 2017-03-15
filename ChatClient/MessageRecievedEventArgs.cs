namespace Chat {
    using System;

    /// <summary>
    /// MessageReceivedEventArgs class.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs {

        #region Constructors

        /// <summary>
        /// Creates an instance of the <see cref="MessageReceivedEventArgs"/> class. 
        /// </summary>
        /// <param name="message"></param>
        public MessageReceivedEventArgs(string message) {
            Message = message;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the received message.
        /// </summary>
        public string Message { get; }

        #endregion Properties
    }
}