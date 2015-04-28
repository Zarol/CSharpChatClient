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
    public partial class MainWindow : Window
    {
        public static TcpClient server;
        public static Login loginPage;
        public static ChatRoom chatRoomPage;
        public static Frame _frameMain;

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();            
            chatRoomPage = new ChatRoom();
            loginPage = new Login();
            _frameMain = frameMain;
        }

        /// <summary>
        /// Initialize the current page on the window to the login page.
        /// </summary>
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            _frameMain.Navigate(loginPage);
        }
    }
}
