using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;

using VrRacingGameDataCollection;

namespace Server {
    class Program {
        /// <summary>
        /// The server itself.
        /// </summary>
        public static TcpListener Listener;
        /// <summary>
        /// The thread used for listening to and handling new client requests.
        /// </summary>
        public static Thread ListenForClients;
        /// <summary>
        /// The lists of clients, updated realtime.
        /// </summary>
        public static Dictionary<string, Client> clientList = new Dictionary<string, Client>();

        public static string ip = "127.0.0.1";
		public static string serverName = "";
		public static string password = "";
        public static int port = 25001;
        public static bool running = false;

        public static void CloseClient(Client client) {
            if (client == null) return;

            while (client.Socket.Connected) client.Socket.Close();
            clientList.Remove(client.Username);

            Console.WriteLine("CLIENT REMOVED");
        }

        static void Main( string[] args ) {
            //Process.Start(@"Client.exe");

            SetOptions();
            Console.WriteLine("=================== Virtual Reality Racing Game server ===================\n");

            Console.WriteLine("Starting server on " + ip + ":" + port);
            Console.WriteLine("Server name: " + serverName);
            Console.WriteLine("Password: " + password);

            try {
                Listener = new TcpListener(IPAddress.Parse(ip), port);
                ListenForClients = new Thread(Listen);
                ListenForClients.Start();
            } catch (Exception ex) { Console.WriteLine("\n" + ex + "\n"); }
            
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
                Client client = null;

                try {
                    client = new Client(Listener.AcceptTcpClient());
                    Console.WriteLine("New connection request.");

                } catch (Exception ex) {
                    Console.WriteLine("\n" + ex + "\n");

                    CloseClient(client);

                    running = false;
                }
            }
        }

        private static void CloseServer() {
            foreach (KeyValuePair<string, Client> pair in clientList) {
				SendMessage(
					pair.Value, 
					new Packet(
						"Server", 
						pair.Key, 
						VrrgDataCollectionType.Command, 
						new string[] { "message", "serverClosed" }
					)
				);

                CloseClient(pair.Value);
            }

            Listener.Stop();
            Environment.Exit(0);
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
                    case "bc": case "broadcast":
                        if (input.Count < 3) {
                            Console.WriteLine("Insufficient parameters given, ex: bc Hello World!");
                            continue;
                        }

                        string broadcastMessage = "";

                        for (int i = 2; i < input.Count; i++) {
                            string space = "";
                            if (i != 2) space = " ";
                            broadcastMessage += space + input[i];
                        }

						Broadcast(
							new Packet(
								"Server", 
								"ALL", 
								VrrgDataCollectionType.Message, 
								new string[] { "broadcastMessage", broadcastMessage }
							)
						);
                        break;
                    case "pm":
                        if (input.Count < 3) {
                            Console.WriteLine("Insufficient parameters given, ex: pm johnny Hello World!");
                            continue;
                        }

                        if (clientList.ContainsKey(input[2].ToLower())) {
                            string pmMessage = "";

                            for (int i = 3; i < input.Count; i++) {
                                string space = "";
                                if (i != 3)
                                    space = " ";
                                pmMessage += space + input[i];
                            }

							SendMessage(
								clientList[input[2].ToLower()], 
					            new Packet(
						            "Server", 
			                        input[2], 
						            VrrgDataCollectionType.Message,
						            new string[] { "message", pmMessage }
								), true
							);
                        } else {
                            Console.WriteLine("Client \"" + input[2] + "\" does not exist.");
                        }
                        break;
                    case "list": case "ls":
                        if (clientList.Count == 0) {
                            Console.WriteLine("0 clients connected.");
                            break;
                        }

                        Console.WriteLine(clientList.Count + " client(s) connected:\n");

                        foreach (KeyValuePair<string, Client> client in clientList) {
                            Console.WriteLine(client.Value.Username + " - " + client.Value.Socket.Client.LocalEndPoint);
                        }
                        break;
                    case "kick":

						if (clientList.ContainsKey(input[2].ToLower())) clientList.Remove(input[2]);
						else Console.WriteLine("Client \"" + input[2] + "\" does not exist.");

						if (clientList.ContainsKey(input[2])) Console.WriteLine("Client \"" + input[2] + "\" could not be kicked.");
						else Console.WriteLine("Client \"" + input[2] + "\" was successfully kicked.");

                        break;
                    case "exit": case "quit": case "close": case "stop":
                        CloseServer();
                        break;
                    case "newclient":
                        Process.Start(@"Client.exe");
                        break;
                }
            }
        }

		/// <summary>
		/// Sends an array of bytes to the appointed client.
		/// </summary>
		/// <param name="client">The client to receive the packet</param>
		/// <param name="packet">The packet to be sent to the client</param>
		public static void SendMessage(Client client, Packet packet, bool logMessage = true)
		{
			try
			{
				byte[] buffer = Encoding.ASCII.GetBytes(packet.ToString());

				NetworkStream sendStream = client.Socket.GetStream();
				sendStream.Write(buffer, 0, buffer.Length);

				if (logMessage)
					Console.WriteLine("Server > " + client.Username + ": " + packet);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n" + ex + "\n");

				CloseClient(client);
			}
		}

        /// <summary>
        /// Broadcasts a packet to all the connected clients.
        /// </summary>
		/// <param name="packet">The packet to broadcast</param>
        public static void Broadcast (Packet packet) {
            foreach (KeyValuePair<string, Client> pair in clientList) {
                try {
                    SendMessage(pair.Value, packet, false);
                    Console.WriteLine("Server > All: " + packet);
                } catch (Exception ex) {
                    Console.WriteLine("\n" + ex + "\n");

                    CloseClient(pair.Value);
                }
            }
        }

        /// <summary>
        /// Initializer for the server.
        /// </summary>
        private static void SetOptions() {
			int optionCount = 0;

            while (optionCount < 4) {
                Console.WriteLine("=================== Virtual Reality Racing Game server ===================");

                switch (optionCount) {
                    default:
                        Console.WriteLine("Option (" + optionCount + ") does not exist");
                        break;
                    case 0: // Set IP address and port
                        Console.Write("IP (press ENTER to bind all available): ");
                        string result = Console.ReadLine();

                        if (result.Trim(' ') != "") {
                            // Check if port was given, if so split the result at ":" and update the variables
                            if (result.IndexOf(':') != -1 && result.IndexOf(':') != result.Length) {
                                string[] temp = result.Split(':');
                                ip = temp[0];

                                try {
                                    port = Convert.ToInt16(temp[1]);
                                } catch (Exception ex) {
                                    Console.WriteLine("\nInvalid port \"" + temp[1] + "\". Press enter to retry.");
                                    Console.ReadLine();

                                    continue;
                                }

                            } else ip = result;
                        } else ip = "0.0.0.0";

                        IPAddress garbage;
                        if (IPAddress.TryParse(ip, out garbage)) optionCount++;
                        else {
                            Console.WriteLine("\nInvalid IP Address \"" + ip + "\". Press enter to retry.");
                            Console.ReadLine();
                        }
                        break;
                    case 1: // Set max players.
                        Console.Write("Max players (2 - 16): ");
                        string read = Console.ReadLine();
                        int maxPlayers = 0;

                        if (string.IsNullOrEmpty(read)) maxPlayers = 16;
                        else {
                            try {
                                maxPlayers = Convert.ToInt16(read);
                                if (maxPlayers < 0) maxPlayers = 2;
                                if (maxPlayers > 16) maxPlayers = 16;
                            } catch (Exception ex) {}
                        }

                        optionCount++;
                        break;
					case 2: // Set servername.
						Console.Write("Server name: ");

						try
						{
							serverName = Console.ReadLine();

							if (serverName.Length < 32) optionCount++;
							else {
								Console.WriteLine("Server name too long (Max nr of characters is 32). Press enter to retry.");
								Console.ReadLine();
							}
						}
						catch (Exception ex) { }

						break;
					case 3: // Set password.
						Console.Write("Password (Optional, leave blank to keep the server open): ");

						try
						{
							password = Console.ReadLine();

							if (password.Length < 32) optionCount++;
							else {
								Console.WriteLine("Password too long (Max nr of characters is 32). Press enter to retry.");
								Console.ReadLine();
							}
						}
						catch (Exception ex) { }

						break;
                }

                Console.Clear();
            }
        }
    }
}
