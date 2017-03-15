namespace Chat {

    /// <summary>
    /// Delegate used to handle chat-related error events.
    /// </summary>
    /// <param name="sender">Object sender</param>
    /// <param name="e">Error event arguments.</param>
    public delegate void OnChatErrorEvent(object sender, OnErrorEventArgs e);

    /// <summary>
    /// Delegate used to handle the message recieved event.
    /// </summary>
    /// <param name="sender">Object sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void OnMessageReceivedEvent(object sender, MessageReceivedEventArgs e);
}