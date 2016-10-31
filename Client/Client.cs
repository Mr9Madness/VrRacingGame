using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VrRacingGameDataCollection;

namespace Client {


    static class Client {
        private const int MAXBUFFERSIZE = 256;

        public static string Username;
        public static string Ip;
        public static int Port;

        public static TcpClient Socket;
        public static Thread ListenToServer;

        public static void Connect(string ip = "127.0.0.1", int port = 25001) {
            Ip = ip;
            Port = port;

            try {
                Socket = new TcpClient();
                Socket.Connect(Ip, Port);

                SendMessage(@"Type\2\Command\1\Message\2\username=" + Username);

                ListenToServer = new Thread(Listen);
                ListenToServer.Start();
            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused")) {
                    Console.WriteLine("No server found at " + Ip + ":" + Port);
                } else
                    Console.WriteLine("\n" + ex + "\n");
            }
        }

        /// <summary>
        /// Sends an array of bytes to the appointed client.
        /// </summary>
        /// <param name="message">The message to be sent to the client</param>
        /// <param name="logMessage">If the message should be logged to the console</param>
        private static void SendMessage(string message, bool logMessage = true) {
            try {
                byte[] buffer = Encoding.ASCII.GetBytes(message);

                NetworkStream sendStream = Socket.GetStream();
                sendStream.Write(buffer, 0, buffer.Length);

                if (logMessage)
                    Console.WriteLine(Username + " > Server: " + message);
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            }
        }

        private static void Listen() {

            Console.WriteLine("Connected to server!\nListening for server input...");
            while (Socket.Connected) {
                Command command = new Command(ReceiveMessage());

                if (command.Msg != "usernameRejected")
                    continue;
                Console.WriteLine("The username \"" + Username + "\" already in use on this server.\nClosing connection...");
                Program.CloseConnection();
            }
        }

        private static string ReceiveMessage(bool logMessage = true) {
            try {
                NetworkStream getStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = getStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

                string message = Encoding.ASCII.GetString(actualRead.ToArray());
                if (logMessage)
                    Console.WriteLine(message);
                return message;
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            }

            return null;
        }
    }
}
