using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;

namespace Server {

    class Client {
        private const int MAXBUFFERSIZE = 256;
        public string Username;
        public TcpClient Socket;

        public Client(TcpClient socket) {
            try {
                Socket = socket;
                Username = ReceiveMessage().Split('=')[1];
            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused")) return;

                Console.WriteLine("\n" + ex + "\n");
            }
        }

        private string ReceiveMessage() {
            try {
                NetworkStream getStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = getStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

                return Encoding.ASCII.GetString(actualRead.ToArray());
            } catch (Exception ex) {
                Console.WriteLine("\"" + ex + "\"");
            }
            return null;
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
        public static Dictionary<string, Client> clientList = new Dictionary<string, Client>();

        private static string ip = "127.0.0.1";
        private static string serverName = "";
        private static int port = 25001;
        private static bool running = false;

        static void Main( string[] args ) {
            Process.Start(@"D:\github\VrRacingProject\Client\bin\Debug\Client.exe");

            SetOptions();
            Console.WriteLine("=================== Virtual Reality Racing Game server ===================\n");

            Console.WriteLine("Starting server on " + ip + ":" + port);

            try {
                Listener = new TcpListener(IPAddress.Parse(ip), port);
                ListenForClients = new Thread(Listen);
                ListenForClients.Start();
            } catch (Exception ex) { Console.WriteLine("\n" + ex + "\n"); }

            Console.WriteLine("Server name: " + serverName);

            Thread serverCmd = new Thread(ServerCommands);
            serverCmd.Start();
        }

        /// <summary>
        /// The server uses this to handle new connection requests.
        /// </summary>
        private static void Listen() {
            Listener.Start();
            running = true;

            Console.WriteLine("Listening for clients...\n");

            while (running) {
                try {
                    Client client = new Client(Listener.AcceptTcpClient());

                    Console.WriteLine("New connection request.");
                    if (!clientList.ContainsKey(client.Username)) {
                        clientList.Add(client.Username, client);
                        Console.WriteLine(client.Username + " joined the server.\n");
                    } else {
                        SendMessage(client, "usernameRejected");
                        Console.WriteLine("Connection request denied, username \"" + client.Username + "\" was already in use.\n");
                    }

                } catch (Exception ex) {
                    Console.WriteLine("\n" + ex + "\n");

                    running = false;
                }
            }
        }

        /// <summary>
        /// Handle server commands while server is active.
        /// </summary>
        private static void ServerCommands() {
            while (true) {
                List<string> input = new List<string>(new[] { Console.ReadLine() });
                if (input[0].Contains(" ")) input.AddRange(input[0].Split(' '));
                else input.Add(input[0]);

                switch (input[1].ToLower()) {
                    default:
                        Console.WriteLine("Command \"" + input[1] + "\" does not exist.\n");
                        break;
                    case "broadcast":
                        string broadcastMessage = "";

                        for (int i = 2; i < input.Count; i++) {
                            string space = "";
                            if (i != 2) space = " ";
                            broadcastMessage += space + input[i];
                        }

                        Broadcast(broadcastMessage);
                        break;
                    case "pm":
                        if (clientList.ContainsKey(input[2])) {
                            string pmMessage = "";

                            for (int i = 3; i < input.Count; i++) {
                                string space = "";
                                if (i != 3)
                                    space = " ";
                                pmMessage += space + input[i];
                            }

                            SendMessage(clientList[input[2]], pmMessage, true);
                        } else {
                            Console.WriteLine("Client \"" + input[2] + "\" does not exist.");
                        }
                        break;
                    case "kick":
                        break;
                    case "exit": case "quit": case "stop":
                        //CloseServer();
                        Console.WriteLine("Server termination requested.");
                        break;
                    case "newClient":
                        Process.Start(@"D:\github\VrRacingProject\Client\bin\Debug\Client.exe");
                        break;
                }
            }
        }

        /// <summary>
        /// Sends an array of bytes to the appointed client.
        /// </summary>
        /// <param name="client">The client to receive the message</param>
        /// <param name="message">The message to be sent to the client</param>
        public static void SendMessage(Client client, string message, bool logMessage = false) {
            try {
                byte[] buffer = Encoding.ASCII.GetBytes(message);

                NetworkStream sendStream = client.Socket.GetStream();
                sendStream.Write(buffer, 0, buffer.Length);

                if (logMessage)
                    Console.WriteLine("Server > " + client.Username + ": " + message);
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            }
        }

        /// <summary>
        /// Broadcasts a message to all the connected clients.
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        public static void Broadcast (string message) {
            try {
                foreach (KeyValuePair<string, Client> pair in clientList) {
                    SendMessage(pair.Value, message);
                }

                Console.WriteLine("Server > All: " + message);
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
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
