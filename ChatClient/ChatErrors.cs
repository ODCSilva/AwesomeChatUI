namespace Chat {

    /// <summary>
    /// Enumerates possible error types that can be encountered in the ChatClient.
    /// </summary>
    public enum ChatError {
        /// <summary>
        /// Host has disconnected.
        /// </summary>
        HOST_DICONNECTED,

        /// <summary>
        /// Invalid operation.
        /// </summary>
        INVALID_OPERATION,

        /// <summary>
        /// Represents IO Error.
        /// </summary>
        IO_ERROR,

        /// <summary>
        /// Trying to use socket after it has been disposed.
        /// </summary>
        OBJECT_DISPOSED,

        /// <summary>
        /// Port out of maximum socket range.
        /// </summary>
        PORT_OUT_OF_RANGE,

        /// <summary>
        /// Socket error.
        /// </summary>
        SOCKET_ERROR
    }
}