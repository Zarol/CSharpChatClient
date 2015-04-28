using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
    /// Interaction logic for ChatRoom.xaml
    /// </summary>
    public partial class ChatRoom : Page, INotifyPropertyChanged
    {
        public ChatRoom()
        {
            InitializeComponent();
            txtBoxChat.DataContext = this;
        }
        /// <summary>
        /// Sends the server a message within the textbox.
        /// </summary>
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            //Creates a TCP stream to send the server a message
            NetworkStream clientStream = MainWindow.server.GetStream();

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
        public void HandleServerComm(object client)
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
        #endregion
    }
}
