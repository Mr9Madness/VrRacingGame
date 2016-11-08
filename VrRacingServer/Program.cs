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
        public static Dictionary<string, Client> ClientList = new Dictionary<string, Client>();

        public static string Ip = "127.0.0.1";
		public static string ServerName = "";
		public static string Password = "";
        public static int Port = 25001;
        public static int MaxPlayers = 0;

        public static void CloseClient(Client client) {
            if (client == null) return;

            if (client.Socket.Connected)
                SendMessage(
                    client, 
                    new Packet(
                        "Server",
                        client.Username,
                        VrrgDataCollectionType.Command,
                        new [] { "disconnectClient", "true" }
                    )
                );

            client.ListenToClient.Abort();

            while (client.Socket.Connected) client.Socket.Close();
            ClientList.Remove(client.Username);

            if (ClientList.ContainsKey(client.Username.ToLower()))
                ClientList.Remove(client.Username.ToLower());

            Console.WriteLine(client.Username + " has left the server.");
            client = null;
        }

        static void Main( string[] args ) {
            //Process.Start(@"Client.exe");

            SetOptions();
            Console.WriteLine("=================== Virtual Reality Racing Game server ===================\n");

            Console.WriteLine("Starting server on " + Ip + ":" + Port);
            Console.WriteLine("Server name: " + ServerName);
            Console.WriteLine("Password: " + Password);

            try {
                Listener = new TcpListener(IPAddress.Parse(Ip), Port);

                ListenForClients = new Thread(Listen);
                ListenForClients.Start();
            } catch (Exception ex) {
                Console.WriteLine("\n" + ex + "\n");
            
                CloseServer();
            }
            
            Thread serverCmd = new Thread(ServerCommands);
            serverCmd.Start();
        }

        /// <summary>
        /// The server uses this to handle new connection requests.
        /// </summary>
        private static void Listen() {
            Listener.Start();
            Console.WriteLine("Listening for clients...\n");

            while (true) {
                Client client = null;

                try { client = new Client(Listener.AcceptTcpClient()); }
                catch (Exception ex) {
                    if (!ex.ToString().Contains("actively refused")) Console.WriteLine("\n" + ex + "\n");

                    if (client != null) CloseClient(client);
                    break;
                }
            }

            Listener.Stop();
        }

        private static void CloseServer() {
            foreach (KeyValuePair<string, Client> pair in ClientList) {
				SendMessage(
					pair.Value, 
					new Packet(
						"Server", 
						pair.Key, 
						VrrgDataCollectionType.Command, 
						new [] { "message", "serverClosed" }
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

                        if (ClientList.ContainsKey(input[2].ToLower())) {
                            string pmMessage = "";

                            for (int i = 3; i < input.Count; i++) {
                                string space = "";
                                if (i != 3)
                                    space = " ";
                                pmMessage += space + input[i];
                            }

							SendMessage(
								ClientList[input[2].ToLower()], 
					            new Packet(
						            "Server", 
			                        input[2], 
						            VrrgDataCollectionType.Message,
						            new [] { "message", pmMessage }
								)
							);
                        } else {
                            Console.WriteLine("Client \"" + input[2] + "\" does not exist.");
                        }
                        break;
                    case "list": case "ls":
                        if (ClientList.Count == 0) {
                            Console.WriteLine("No clients connected.");
                            break;
                        }

                        Console.WriteLine(ClientList.Count + " client(s) connected:\n");

                        foreach (KeyValuePair<string, Client> client in ClientList) {
                            Console.WriteLine(client.Value.Username + " - " + client.Value.Socket.Client.LocalEndPoint);
                        }
                        break;
                    case "kick":
                        Client c = ClientList[input[2].ToLower()];

                        SendMessage(c,
                            new Packet(
                                "Server",
                                c.Username,
                                VrrgDataCollectionType.Command,
                                new [] { "disconnectClient", "true" }
                            )
                        );

                        if (ClientList.ContainsKey(input[2].ToLower())) {
                            ClientList.Remove(input[2].ToLower());

                            if (ClientList.ContainsKey(input[2].ToLower()))
                                Console.WriteLine("Client \"" + input[2] + "\" could not be kicked.");
                            else
                                Console.WriteLine("Client \"" + input[2] + "\" was successfully kicked.");
                        } else
                            Console.WriteLine("Client \"" + input[2] + "\" does not exist.");

						

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
		public static void SendMessage(Client client, Packet packet, bool logMessage = true) {
			try {
				byte[] buffer = Encoding.ASCII.GetBytes(packet.ToString());

				NetworkStream sendStream = client.Socket.GetStream();
				sendStream.Write(buffer, 0, buffer.Length);

				if (logMessage)
					Console.WriteLine("Server > " + client.Username + ": " + packet);
			} catch (Exception ex) {
                if (!ex.ToString().Contains("forcibly closed") &&
                    !ex.ToString().Contains("valid Vrrg Packet") &&
                    !ex.ToString().Contains("connection was aborted"))
                    Console.WriteLine("\"" + ex + "\"");

                CloseClient(client);
			}
		}

        /// <summary>
        /// Broadcasts a packet to all the connected clients.
        /// </summary>
		/// <param name="packet">The packet to broadcast</param>
        public static void Broadcast (Packet packet) {
            try {
                foreach (KeyValuePair<string, Client> pair in ClientList) {
                    try {
                        SendMessage(pair.Value, packet, false);
                        Console.WriteLine("Server > All: " + packet);
                    } catch (Exception ex) {
                        if (!ex.ToString().Contains("forcibly closed") &&
                            !ex.ToString().Contains("valid Vrrg Packet") &&
                            !ex.ToString().Contains("connection was aborted"))
                            Console.WriteLine("\"" + ex + "\"");

                        CloseClient(pair.Value);
                    }
                }
            } catch (Exception) {
                // ignored
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
                    case 0: // Set IP address and Port
                        Console.Write("IP (press ENTER to bind all available): ");
                        string result = Console.ReadLine();

                        if (result.Trim(' ') != "") {
                            // Check if Port was given, if so split the result at ":" and update the variables
                            if (result.IndexOf(':') != -1 && result.IndexOf(':') != result.Length) {
                                string[] temp = result.Split(':');
                                Ip = temp[0];

                                try {
                                    Port = Convert.ToInt16(temp[1]);
                                } catch (Exception) {
                                    Console.Clear();
                                    Console.WriteLine("\nInvalid Port \"" + temp[1] + "\".");

                                    continue;
                                }

                            } else Ip = result;
                        } else Ip = "0.0.0.0";

                        IPAddress garbage;
                        if (IPAddress.TryParse(Ip, out garbage)) optionCount++;
                        else {
                            Console.WriteLine("\nInvalid IP Address \"" + Ip + "\". Press enter to retry.");
                            Console.ReadLine();
                        }
                        break;
					case 1: // Set servername.
						Console.Write("Server name: ");

						try
						{
							ServerName = Console.ReadLine();

							if (ServerName != null && ServerName.Length < 32) optionCount++;
							else {
								Console.WriteLine("Server name too long (Max nr of characters is 32). Press enter to retry.");
								Console.ReadLine();
							}
						}
						catch (Exception) { 
                            // ignored
                        }

                        break;
                    case 2: // Set max players.
                        Console.Write("Max players (2 - 16): ");
                        string read = Console.ReadLine();

                        if (string.IsNullOrEmpty(read)) MaxPlayers = 16;
                        else {
                            try {
                                MaxPlayers = Convert.ToInt16(read);
                                if (MaxPlayers < 0) MaxPlayers = 2;
                                if (MaxPlayers > 16) MaxPlayers = 16;
                            } catch (Exception) {
                                // ignored
                            }
                        }

                        optionCount++;
                        break;
					case 3: // Set Password.
						Console.Write("Password (Optional, leave blank to keep the server open): ");

                        try {
							Password = Console.ReadLine();

							if (Password != null && Password.Length < 32) optionCount++;
							else {
                                Console.WriteLine(
                                    "Password too long (Max nr of characters is 32). Press enter to retry.");
								Console.ReadLine();
							}
						} catch (Exception) {
						    // ignored
						}

						break;
                }

                Console.Clear();
            }
        }
    }
}
