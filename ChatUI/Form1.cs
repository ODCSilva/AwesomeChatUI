namespace ChatUI {
    using Chat;
    using ChatLogger;
    //using LogLib;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// ChatUI class.
    /// </summary>
    public partial class ChatUI : Form {

        #region Fields

        /// <summary>
        /// Specifies the text to be displayed in the status bar when connected.
        /// </summary>
        private const string CONNECTEDTEXT = "Connected to {0}:{1}";

        /// <summary>
        /// Specifies the text to be displayed in the status bar when disconnected.
        /// </summary>
        private const string DISCONNECTEDTEXT = "Disconnected";

        /// <summary>
        /// Instance of ChatClient responsible for managing chat connectivity.
        /// </summary>
        private ChatClient client;

        /// <summary>
        /// Thread instance responsible for running the message listening loop.
        /// </summary>
        private Thread workerThread;

        private readonly ILoggingService chatLogger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatUI"/> class.
        /// </summary>
        public ChatUI(ILoggingService logger) {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var localPath = new Uri(assemblyPath).LocalPath;

            chatLogger = logger;
            client = new ChatClient(logger: chatLogger);
            client.OnMessageReceived += new OnMessageReceivedEvent(Client_OnMessageRecieved);
            client.OnError += new OnChatErrorEvent(Client_OnChatError);
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Send button Click delegate.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ButtonSend_Click(object sender, EventArgs e) {
            var message = messageComposer.Text;
            Thread sendThread = new Thread(() => client.SendMessage(message));

            if (message != string.Empty) {
                sendThread.Start();
                messageComposer.Text = string.Empty;
                UpdateConversationWindow(">> " + message + "\r\n");
            }
        }

        /// <summary>
        /// Delegate used to respont to FormClosing event.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ChatUI_FormClosing(object sender, FormClosingEventArgs e) {
            client.Stop = true;
        }

        /// <summary>
        /// Event handler that responds to ChatClient.OnChatErrorEvent.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Client_OnChatError(object sender, OnErrorEventArgs e) {
            var message = e.ErrorMessage;

            switch (e.ErrorType) {
                case ChatError.SOCKET_ERROR:
                    DisableChatControls();
                    MessageBox.Show(e.ErrorMessage, "Error");
                    break;

                case ChatError.HOST_DICONNECTED:
                    DisableChatControls();
                    UpdateConversationWindow(e.ErrorMessage);
                    break;

                default:
                    MessageBox.Show(e.ErrorMessage, "Error");
                    break;
            }

            UpdateStatusText(DISCONNECTEDTEXT);
        }

        /// <summary>
        /// Event handler that responds to ChatClient.OnMessageRecieved.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void Client_OnMessageRecieved(object sender, MessageReceivedEventArgs e) {
            UpdateConversationWindow(e.Message + "\r\n");
        }

        /// <summary>
        /// Event handler that responds to the Click event on the Connect menu option.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ConnectMenuItem_Click(object sender, EventArgs e) {
            workerThread = new Thread(client.Connect);
            workerThread.Name = "Chat Client";
            workerThread.Start();
            EnableChatControls();
            UpdateStatusText(string.Format(CONNECTEDTEXT, client.Hostname, client.Port));
        }

        /// <summary>
        /// Disables chat controls when not connected. Thread-safe.
        /// </summary>
        private void DisableChatControls() {
            if (InvokeRequired) {
                MethodInvoker invoker = new MethodInvoker(delegate () {
                    buttonSend.Enabled = false;
                    connectMenuItem.Enabled = true;
                    conversation.Enabled = false;
                    disconnectMenuItem.Enabled = false;
                    messageComposer.Enabled = false;
                });

                BeginInvoke(invoker);
            }
            else {
                buttonSend.Enabled = false;
                connectMenuItem.Enabled = true;
                conversation.Enabled = false;
                disconnectMenuItem.Enabled = false;
                messageComposer.Enabled = false;
            }
        }

        /// <summary>
        /// Event handler that responds to the Click event on the Disconnect menu option.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisconnectMenuItem_Click(object sender, EventArgs e) {
            client.Stop = true;
            DisableChatControls();
            UpdateStatusText(DISCONNECTEDTEXT);
        }

        /// <summary>
        /// Enables chat controls when the client is connected. Thread-safe.
        /// </summary>
        private void EnableChatControls() {
            if (InvokeRequired) {
                MethodInvoker invoker = new MethodInvoker(delegate () {
                    buttonSend.Enabled = true;
                    connectMenuItem.Enabled = false;
                    conversation.Enabled = true;
                    disconnectMenuItem.Enabled = true;
                    messageComposer.Enabled = true;
                });

                BeginInvoke(invoker);
            }
            else {
                buttonSend.Enabled = true;
                connectMenuItem.Enabled = false;
                conversation.Enabled = true;
                disconnectMenuItem.Enabled = true;
                messageComposer.Enabled = true;
            }
        }

        /// <summary>
        /// Event handler that responds to the Click event on the Exit menu option.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void ExitMenuItem_OnClick(object sender, EventArgs e) {
            Application.Exit();
        }

        /// <summary>
        /// Event handler that responds to the KeyUp event on the messageComposer.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">Event arguments.</param>
        private void MessageComposer_KeyUp(object sender, KeyEventArgs e) {
            // If user presses enter then send the message.
            if (e.KeyCode == Keys.Enter) {
                ButtonSend_Click(null, null);
            }
        }

        /// <summary>
        /// Updates the conversation window in a thread-safe manner.
        /// </summary>
        /// <param name="message">The message to post in the conversation window.</param>
        private void UpdateConversationWindow(string message) {
            if (conversation.InvokeRequired) {
                MethodInvoker invoker = new MethodInvoker(delegate () {
                    conversation.AppendText(message);
                });
                conversation.BeginInvoke(invoker);
            }
            else {
                conversation.AppendText(message);
            }
        }

        /// <summary>
        /// Updates the text in the status bar. Thread-safe.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void UpdateStatusText(string message) {
            if (statusText.GetCurrentParent().InvokeRequired) {
                MethodInvoker invoker = new MethodInvoker(delegate () {
                    statusText.Text = message;
                });
                statusText.GetCurrentParent().BeginInvoke(invoker);
            }
            else {
                statusText.Text = message;
            }
        }

        #endregion Methods
    }
}