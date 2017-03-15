namespace Chat {

    using ChatLogger;
    //using LogLib;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// A class that represents a chat client.
    /// </summary>
    public class ChatClient {

        #region Fields

        /// <summary>
        /// TcpListener client. Initialized when a connection with a client has been established.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// Instance of Logger class. Responsible for saving chat messages to a log.
        /// </summary>
        private ILoggingService logger;

        /// <summary>
        /// NetworkStream used to send and receive messages.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// OnError event.
        /// </summary>
        public event OnChatErrorEvent OnError;

        /// <summary>
        /// OnMessageReceived event.
        /// </summary>
        public event OnMessageReceivedEvent OnMessageReceived;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Client constructor.
        /// </summary>
        /// <param name="port">Host port to connect to.</param>
        /// <param name="hostname">Host address to connect to.</param>
        /// <param name="logger">Logging class</param>
        public ChatClient(string hostname = "127.0.0.1", int port = 13000, ILoggingService logger = null) {
            Port = port;
            Hostname = hostname;
            Stop = false;
            this.logger = logger;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Host address the client will connect to.
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// Host port the client will connnect to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Sets or gets if the listening loop should end.
        /// </summary>
        public bool Stop { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Connects the TcpClient to the host specified by hostname, and
        /// the port specified by the port properties.
        /// </summary>
        public void Connect() {
            try {
                if (client == null) {
                    client = new TcpClient();
                    client.Connect(Hostname, Port);
                    stream = client.GetStream();

                    logger?.Log(string.Format("Client connected host {0} on port {1}", Hostname, Port));

                    // Main listening loop
                    while (!Stop
                        && GetPendingMessages()) {
                        if (PollConnection()) {
                            OnError?.Invoke(this, new OnErrorEventArgs(ChatError.HOST_DICONNECTED, "Connection lost (Host disconnected?)"));
                            break;
                        }

                        Thread.Sleep(200);
                    }
                    // End main listening loop
                }
            }
            catch (ArgumentOutOfRangeException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.PORT_OUT_OF_RANGE, e.Message));
            }
            catch (SocketException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.SOCKET_ERROR, e.Message));
            }
            catch (IOException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.IO_ERROR, e.Message));
            }
            catch (ObjectDisposedException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.OBJECT_DISPOSED, e.Message));
            }
            catch (InvalidOperationException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.INVALID_OPERATION, e.Message));
            }
            finally {
                Disconnect();
            }
        }

        /// <summary>
        /// Closes the TcpClient and NetworkStream and disposes the objects.
        /// </summary>
        public void Disconnect() {
            logger?.Log("Client disconnected.");

            stream?.Close();

            if (client != null) {
                client.Close();
                client = null;
            }

            Stop = false;
        }

        /// <summary>
        /// Sends a message using network stream.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns>True if the stream could be written to, false otherwise.</returns>
        public void SendMessage(string message) {
            try {
                if (stream.CanWrite) {
                    // Encode text to ASCII and store it as a byte array
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    // Send the message
                    stream.Write(data, 0, data.Length);
                    logger?.Log(message);
                }
            }
            catch (IOException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.IO_ERROR, e.Message));
            }
            catch (ObjectDisposedException e) {
                OnError?.Invoke(this, new OnErrorEventArgs(ChatError.OBJECT_DISPOSED, e.Message));
            }
        }

        /// <summary>
        /// Listen for incoming messages without blocking. Invokes the OnMessageReceived event
        /// to deliver the message.
        /// </summary>
        /// <returns>True if the stream can be read, false otherwise.</returns>
        private bool GetPendingMessages() {
            if (stream.CanRead) {
                byte[] data = new byte[256];
                string incomingMessage = string.Empty;

                // Perform read if data is available
                while (stream.DataAvailable) {
                    int bytes = stream.Read(data, 0, data.Length);
                    incomingMessage += Encoding.ASCII.GetString(data, 0, bytes);
                }

                // Trim leading whitespace
                incomingMessage = incomingMessage.Trim();

                if (incomingMessage.Length > 0) {
                    OnMessageReceived?.Invoke(this, new MessageReceivedEventArgs(incomingMessage));
                    logger?.Log(incomingMessage);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Polls the Client Socket to check if the connection is still alive.
        /// </summary>
        /// <returns>True if connection has been lost, false otherwise.</returns>
        private bool PollConnection() {
            return client.Client.Poll(200, SelectMode.SelectRead);
        }

        #endregion Methods
    }
}