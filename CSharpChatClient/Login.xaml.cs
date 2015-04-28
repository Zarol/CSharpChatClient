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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page, INotifyPropertyChanged
    {

        public Login()
        {
            InitializeComponent();
            txtBoxIP.DataContext = this;
            //IP address of server
            ipAddress = "192.168.1.120";
        }

        /// <summary>
        /// Connect the client to the server (localhost).
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {      
            MainWindow.server = new TcpClient();
            //Create a new endpoint so the client can connect to the server
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 3000);

            try
            {
                //Connect to the TCP server
                MainWindow.server.Connect(serverEndPoint);

                //Start a new thread to handle communications from the server
                Thread serverThread = new Thread(new ParameterizedThreadStart(MainWindow.chatRoomPage.HandleServerComm));
                serverThread.Start(MainWindow.server);

                MainWindow._frameMain.Navigate(MainWindow.chatRoomPage);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("[Client] Could not connect to server: " + ipAddress);
            }
        }

        #region Databindings
        private string _ipAddress;
        public string ipAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                OnPropertyChanged("ipAddress");
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
