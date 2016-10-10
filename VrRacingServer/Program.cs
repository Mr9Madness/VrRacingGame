using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server {

    class Client {
        public string Username;
        public TcpClient Socket = new TcpClient();

        public Client(string username, string ip, int port) {
            Username = username;

            try {
                Console.WriteLine(Username);
                Socket.Connect(ip, port);
            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused")) return;

                Console.WriteLine("\n" + ex + "\n");
            }
        }
    }

    class Program {
        /// <summary>
        /// The server itself.
        /// </summary>
        private static TcpListener Listener;
        /// <summary>
        /// The thread used for listening to and handling new client requests.
        /// </summary>
        private static Thread ListenForClients;
        /// <summary>
        /// The lists of clients, updated realtime.
        /// </summary>
        private static Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();

        private static string ip = "127.0.0.1";
        private static string serverName = "";
        private static int port = 25001;
        private static bool running = false;

        static void Main( string[] args ) {
            SetOptions();
            Console.WriteLine("=================== Virtual Reality Racing Game server ===================\n");

            Console.WriteLine("Starting server on " + ip + ":" + port);

            try {
                Listener = new TcpListener(IPAddress.Parse(ip), port);
                ListenForClients = new Thread(Listen);
                ListenForClients.Start();
            } catch (Exception ex) { Console.WriteLine("\n" + ex + "\n"); }

            Console.WriteLine("Server name: " + serverName);

            Client client = new Client("Test", ip, port);

            Console.ReadLine();
        }

        /// <summary>
        /// The startup of the server
        /// </summary>
        private static void Listen() {
            Listener.Start();
            running = true;

            Console.WriteLine("Listening for clients...\n");

            while (running) {
                try {
                    TcpClient client = Listener.AcceptTcpClient();
                    clientList.Add("Test", client);

                    Console.WriteLine("Client Joined!");
                } catch (Exception ex) {
                    Console.WriteLine("\n" + ex + "\n");

                    running = false;
                }
            }
        }

        /// <summary>
        /// Sends an array of bytes to the appointed client.
        /// </summary>
        /// <param name="client">The client to receive the message</param>
        /// <param name="message">The message to be sent to the client</param>
        private static void SendMessage (TcpClient client, string message) {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            NetworkStream sendStream = client.GetStream();
            sendStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Broadcasts a message to all the connected clients.
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        private static void Broadcast (string message) {
            foreach (KeyValuePair<string, TcpClient> pair in clientList) {
                SendMessage(pair.Value, message);
            }
        }

        /// <summary>
        /// Initializer for the server.
        /// </summary>
        private static void SetOptions() {
            int i = 0;

            while (i < 3) {
                Console.WriteLine("=================== Virtual Reality Racing Game server ===================");

                switch (i) {
                    default:
                        Console.WriteLine("Option (" + i + ") does not exist");
                        break;
                    case 0: // Set IP address and port
                        Console.Write("IP (press ENTER to bind all available): ");
                        string result = Console.ReadLine();

                        if (result.Trim(' ') != "") {
                            // Check if port was given, if so split the result at ":" and update the variables
                            if (result.IndexOf(':') != -1 && result.IndexOf(':') != result.Length) {
                                string[] temp = result.Split(':');
                                ip = temp[0];

                                try { port = Convert.ToInt16(temp[1]); } catch (Exception ex) {
                                    Console.WriteLine("\nInvalid port \"" + temp[1] + "\". Press enter to retry.");
                                    Console.ReadLine();

                                    continue;
                                }

                            } else ip = result;
                        } else ip = "0.0.0.0";

                        IPAddress garbage;
                        if (IPAddress.TryParse(ip, out garbage)) i++;
                        else {
                            Console.WriteLine("\nInvalid IP Address \"" + ip + "\". Press enter to retry.");
                            Console.ReadLine();
                        }
                        break;
                    case 1: // Set max players.
                        Console.Write("Max players (2 - 16): ");

                        try {
                            int maxPlayers = Convert.ToInt16(Console.ReadLine());
                            if (maxPlayers < 0) maxPlayers = 2;
                            if (maxPlayers > 16) maxPlayers = 16;

                            i++;
                        } catch (Exception ex) { }
                        break;
                    case 2: // Set servername.
                        Console.Write("Server name: ");

                        try {
                            serverName = Console.ReadLine();

                            i++;
                        } catch (Exception ex) { }

                        break;
                }

                Console.Clear();
            }
        }
    }
}
