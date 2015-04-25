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

        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * Connect the client to the server (localhost).
         */
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = "192.168.1.120";
            server = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 3000);

            try
            {
                server.Connect(serverEndPoint);

                Thread serverThread = new Thread(new ParameterizedThreadStart(HandleServerComm));
                serverThread.Start(server);

                btnConnect.IsEnabled = false;
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("[Client] Could not connect to server: " + ipAddress);
            }
        }

        /**
         * Sends the server a message within the textbox.
         */
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            //NetworkStream clientStream = server.GetStream();

            //ASCIIEncoding encoder = new ASCIIEncoding();
            //byte[] buffer = encoder.GetBytes(txtBoxMessage.Text);

            //clientStream.Write(buffer, 0, buffer.Length);
            //clientStream.Flush();
            ChatLog = "Hey, how's it going?/nPretty good, how about yourself?";
            txtBoxMessage.Text = ChatLog;
        }

        /**
         * Acquire NetworkStream from the TcpClient to read.
         * Loop reading all the information from the server.
         * If no bytes have been recieved, the server has been disconnected.
         */
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
                ASCIIEncoding encoder = new ASCIIEncoding();
                //broadcastMessage(encoder.GetString(message, 0, bytesRead));
            }

            tcpClient.Close();
        }

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
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
