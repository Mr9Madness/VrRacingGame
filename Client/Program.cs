using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;
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
                
                if (command.Msg != "usernameRejected") continue;
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
                if (logMessage) Console.WriteLine(message);
                return message;
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            }

            return null;
        }
    }

    class Program {
        private static string Ip = "127.0.0.1";
        private static int Port = 25001;

        static void Main(string[] args) {
            while (true) {
                SetOptions();

                char ans;

                while (true) {
                    Console.WriteLine("=================== Virtual Reality Racing Game client ===================\n");

                    Client.Connect(Ip, Port);

                    while (Client.Socket != null) {
                        string input = Console.ReadLine();
                        if (input == "disconnect") CloseConnection();
                        if (input == "exit") Environment.Exit(0);
                    }

                    Console.WriteLine("Disconnected from server!");
                    Console.Write("Would you like to try to reconnect? (Y/N): ");
                    ans = Console.ReadKey().KeyChar;

                    Console.Clear();

                    if (ans == 'y' || ans == 'Y')
                        continue;
                    if (ans == 'n' || ans == 'N')
                        break;
                }
                Console.Write("Would you like to try to restart the client? (Y/N): ");

                if (ans == 'y' || ans == 'Y')
                    continue;
                if (ans == 'n' || ans == 'N')
                    break;
            }
        }

        public static void CloseConnection() {
            Client.ListenToServer.Abort();

            while (Client.Socket != null)
                Client.Socket.Close();
        }

        /// <summary>
        /// Initializer for the client.
        /// </summary>
        private static void SetOptions() {
            int optionCount = 0;

            while (optionCount < 2) {
                Console.WriteLine("=================== Virtual Reality Racing Game client ===================");

                switch (optionCount) {
                    default:
                        Console.WriteLine("Option (" + optionCount + ") does not exist.\nThe program will now close.");
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;
                    case 0: // Set max players.
                        Console.Write("Username (3 - 32 characters): ");
                        string username = Console.ReadLine();

                        if (username.Length < 3) {
                            Console.WriteLine("\nThe username \"" + username + "\" is too short. Press enter to retry.");
                            Console.ReadLine();
                            continue;
                        } else if (username.Length > 32) {
                            string temp = "";
                            for (int i = 0; i < 32; i++)
                                temp += username[i];

                            Client.Username = temp;
                        } else {
                            Client.Username = username;
                        }

                        optionCount++;
                        break;
                    case 1: // Set IP address and port
                        Console.Write("Server IP: ");
                        string result = Console.ReadLine();

                        if (result.Trim(' ') != "") {
                            // Check if port was given, if so split the result at ":" and update the variables
                            if (result.IndexOf(':') != -1 && result.IndexOf(':') != result.Length) {
                                string[] temp = result.Split(':');
                                Ip = temp[0];

                                try { Port = Convert.ToInt16(temp[1]); } catch (Exception ex) {
                                    Console.WriteLine("\nInvalid port \"" + temp[1] + "\". Press enter to retry.");
                                    Console.ReadLine();

                                    Console.Clear();
                                    continue;
                                }

                            } else
                                Ip = result;
                        } else
                            Ip = "127.0.0.1";

                        IPAddress garbage;
                        if (IPAddress.TryParse(Ip, out garbage))
                            optionCount++;
                        else {
                            Console.WriteLine("\nInvalid IP Address \"" + Ip + "\". Press enter to retry.");
                            Console.ReadLine();
                        }
                        break;
                }

                Console.Clear();
            }
        }
    }
}
