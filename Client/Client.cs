using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VrRacingGameDataCollection;

namespace Client {

    static class Server {
        public static string ServerName = "";
        public static List<string> ClientList = new List<string>();
        public static int MaxPlayers = 0;
    }

    static class Client {
        private const int MAXBUFFERSIZE = 256;

        public static string Username;
        public static string Ip;
		public static int Port;
        public static bool Connected;
        public static bool isClosing;

        public static TcpClient Socket;
        public static Thread ListenToServer;

        public static void Connect(string ip = "127.0.0.1", int port = 25001) {
            Ip = ip;
            Port = port;
			Connected = false;
            isClosing = false;

            try {
                Socket = new TcpClient();
                Socket.Connect(Ip, Port);

                Packet packet = new Packet(ReceiveMessage());
                if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("isServerFull"))
                    if (packet.Variables["isServerFull"].ToLower() == "true") {
                        if (!isClosing) Program.CloseConnection("Disconnected from server: Server is full.");
                        return;
                    }

                SendMessage(
					new Packet(
						Username, 
						"Server", 
						VrrgDataCollectionType.Command, 
						new [] { "username", Username }
               		)
	            );
                if (!HandlePassword())
					return;

				ListenToServer = new Thread(Listen);
                ListenToServer.Start();
            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused"))
                    Console.WriteLine("No server found at " + Ip + ":" + Port);
                else
                    Console.WriteLine("\n" + ex + "\n");
            }
        }

		/// <summary>
		/// Sends an array of bytes to the appointed client.
		/// </summary>
		/// <param name="packet">The message to be sent to the client</param>
		/// <param name="logMessage">If the message should be logged to the console</param>
		public static void SendMessage(Packet packet, bool logMessage = true)
		{
			try
			{
				byte[] buffer = Encoding.ASCII.GetBytes(packet.ToString());

				NetworkStream sendStream = Socket.GetStream();
				sendStream.Write(buffer, 0, buffer.Length);

				if (logMessage)
					Console.WriteLine(Username + " > Server: " + packet);
			}
			catch (Exception ex) {
                if (!ex.ToString().Contains("actively refused") &&
                    !ex.ToString().Contains("forcibly closed"))
                    Console.WriteLine("\n" + ex + "\n");

                if (!isClosing)
                    Program.CloseConnection("Disconnected from server.");
            }
		}

        private static bool HandlePassword() {
            Packet p = new Packet(ReceiveMessage());

            if (p != new Packet() &&
                p.Type == VrrgDataCollectionType.Command &&
                p.Variables["usernameAvailable"] != "false") {

                if (p.Variables["passwordRequired"] == "true") {
                    Console.Clear();

                    while (true) {
                        Console.Write("Password: ");

                        string pass = Console.ReadLine();

                        SendMessage(
                            new Packet(
                                Username,
                                "Server",
                                VrrgDataCollectionType.Command,
                                new[] { "password", pass }
                            )
                        );

                        Packet password = new Packet(ReceiveMessage());

                        if (password != new Packet() &&
                            password.Type == VrrgDataCollectionType.Command &&
                            password.Variables.Count > 0 &&
                            password.Variables["passwordAccepted"] == "true") {

                            Console.WriteLine("Connected to \"" + password.Variables["serverName"] + "\"!");
                            Connected = true;

                            break;
                        }

                        Console.Clear();
                        Console.WriteLine("The password you used is incorrect.");
                    }

                } else if (p.Variables["passwordRequired"] == "false") {
                    Console.WriteLine("Connected to \"" + p.Variables["serverName"] + "\"!");
                    Connected = true;
                }

                else Console.WriteLine("Password key not found in packet");
            } else {
                Console.Clear();
                if (!isClosing)
                    Program.CloseConnection("Username not available.");
            }

            return Connected;
        }

        private static void Listen() {
			try {

				while (Socket.Connected && !isClosing)
				{
					Packet packet = new Packet(ReceiveMessage());

					switch (packet.Type)
					{
					    default:
							Console.WriteLine("Type \"" + packet.Type + "\" was not recognized by the server.");
							break;
					    case VrrgDataCollectionType.None:
					        Console.WriteLine("Server received packet with type \"None\": " + packet);
					        break;
						case VrrgDataCollectionType.Command:
					        HandlePackets.Commands(packet);
							break;
						case VrrgDataCollectionType.Message:
                            HandlePackets.Messages(packet);
                            break;
					    case VrrgDataCollectionType.ChatMessage:
                            HandlePackets.ChatMessages(packet);
                            break;
					    case VrrgDataCollectionType.MapData:
                            HandlePackets.MapDatas(packet);
                            break;
						case VrrgDataCollectionType.PlayerUpdate:
                            HandlePackets.TransformUpdates(packet);
                            break;
					}
				}
			} catch (Exception ex) {
			    if (!ex.ToString().Contains("forcibly closed") &&
                    !ex.ToString().Contains("Thread was being aborted"))
                    Console.WriteLine("\n" + ex + "\n");

                if (!isClosing)
                    Program.CloseConnection("Disconnected from server.");
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
                    Console.WriteLine("Server > " + Username + ": " + message);
                return message;
            } catch (Exception ex) {
                if (!ex.ToString().Contains("forcibly closed") &&
                    !ex.ToString().Contains("Thread was being aborted"))
                    Console.WriteLine("\n" + ex + "\n");

                if (!isClosing)
                    Program.CloseConnection("Disconnected from server.");
            }

            return null;
        }
    }
}
