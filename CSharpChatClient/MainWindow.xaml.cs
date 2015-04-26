using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSharpChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        TcpClient server;

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            txtBoxChat.DataContext = this;
        }

        /// <summary>
        /// Connect the client to the server (localhost).
        /// </summary>
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //IP of server's computer
            string ipAddress = "192.168.1.120";
            server = new TcpClient();
            //Create a new endpoint so the client can connect to the server
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 3000);

            try
            {
                //Connect to the TCP server
                server.Connect(serverEndPoint);

                //Start a new thread to handle communications from the server
                Thread serverThread = new Thread(new ParameterizedThreadStart(HandleServerComm));
                serverThread.Start(server);

                //Turn the connect button off to forbid connecting multiple times
                btnConnect.IsEnabled = false;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("[Client] Could not connect to server: " + ipAddress);
            }
        }

        /// <summary>
        /// Sends the server a message within the textbox.
        /// </summary>
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            //Creates a TCP stream to send the server a message
            NetworkStream clientStream = server.GetStream();

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(txtBoxMessage.Text);

            //Sends the server the message, flush the data from the stream
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        /// <summary>
        /// Acquire NetworkStream from the TcpClient to read.
        /// Loop reading all the information from the server.
        /// If no bytes have been recieved, the server has been disconnected.
        /// </summary>
        private void HandleServerComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("[Client] A socket error has occured.");
                    break;
                }

                if (bytesRead == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[Client] Server has been disconnected.");
                    break;
                }

                //Message has been successfully recieved
                //Append the message to the chat log
                ASCIIEncoding encoder = new ASCIIEncoding();
                ChatLog += encoder.GetString(message, 0, bytesRead) + "\r\n";
            }

            tcpClient.Close();
        }


        #region Databindings
        private string _chatLog;
        public string ChatLog
        {
            get { return _chatLog; }
            set
            {
                _chatLog = value;
                OnPropertyChanged("ChatLog");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Databinding property changed event handler.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
       #endregion
}
