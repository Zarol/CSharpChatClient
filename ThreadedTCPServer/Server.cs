using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections;

namespace ThreadedTCPServer
{
    class Server
    {
        private TcpListener tcpListerner;
        private Thread listenThread;

        private ArrayList userList;

        public Server()
        {
            this.userList = new ArrayList();
            this.tcpListerner = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        /**
         * Starts the TcpListener, then sits in a loop accepting connections.
         * AcceptTcpClient will block until a client has connected, which will then
         * use a thread to handle communication with the new client.
         */
        private void ListenForClients()
        {
            this.tcpListerner.Start();

            while (true)
            {
                //Blocks until a client has connected to the server
                TcpClient tcpClient = this.tcpListerner.AcceptTcpClient();

                //Store the connected client into an ArrayList
                userList.Add(tcpClient);

                //Create a thread to handle communication with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(tcpClient);
            }
        }

        /**
         * Acquire NetworkStream from the TcpClient to read.
         * Loop reading all the information from the client.
         * If no bytes have been recieved, the client has disconnected.
         */
        private void HandleClientComm(object client)
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
                    System.Diagnostics.Debug.WriteLine("[Server] A socket error has occured.");
                    break;
                }

                if (bytesRead == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[Server] Client has disconnected.");
                    break;
                }

                //Message has been successfully recieved
                //ASCIIEncoding encoder = new ASCIIEncoding();
                //System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                ASCIIEncoding encoder = new ASCIIEncoding();
                broadcastMessage(encoder.GetString(message, 0, bytesRead));
            }

            tcpClient.Close();
        }

        /**
         * Broadcasts a string to all currently connected clients.
         */
        private void broadcastMessage(string message)
        {
            NetworkStream clientStream;
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(message);

            //Locking the list guarantees the thread safety of the ArrayList
            lock (userList.SyncRoot)
            {
                foreach (TcpClient user in userList)
                {
                    clientStream = user.GetStream();
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
            }
        }
    }
}
