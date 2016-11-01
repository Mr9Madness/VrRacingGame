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

				SendMessage(
					new Packet(
						Username, 
						"Server", 
						VrrgDataCollectionType.Command, 
						new string[] { "username", Username }
               		)
	            );

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
		/// <param name="packet">The message to be sent to the client</param>
		/// <param name="logMessage">If the message should be logged to the console</param>
		private static void SendMessage(Packet packet, bool logMessage = true)
		{
			try
			{
				byte[] buffer = Encoding.ASCII.GetBytes(packet.ToString());

				NetworkStream sendStream = Socket.GetStream();
				sendStream.Write(buffer, 0, buffer.Length);

				if (logMessage)
					Console.WriteLine(Username + " > Server: " + packet);
			}
			catch (Exception ex)
			{
				Console.WriteLine("\n" + ex + "\n");
			}
		}

        private static void Listen() {
			try
			{
				Packet p = new Packet(ReceiveMessage());

				if (p != null &&
					p.Type == VrrgDataCollectionType.Command &&
					p.Variables["usernameAvailable"] != "false")
				{
					if (p.Variables.ContainsKey("password") && p.Variables["password"] != "")
					{
						Console.Write("Password: ");
						string pass = Console.ReadLine();

						SendMessage(new Packet(Username, "Server", VrrgDataCollectionType.Command, new string[] { "password", pass }));
						Packet password = new Packet(ReceiveMessage());

						if (password != null &&
							password.Type == VrrgDataCollectionType.Command &&
							password.Variables.Count > 0 &&
							password.Variables["passwordAccepted"] == "true")

							Console.WriteLine("Connected to server!\nListening for server input...");

						else {
							Console.WriteLine("The password you used is incorrect.");
							Program.CloseConnection();
						}
					}
					else Console.WriteLine("Password key not found in packet");
				}
				else {
					Console.WriteLine("The username \"" + Username + "\" already in use on this server.\nClosing connection...");
					Program.CloseConnection();
				}

				while (Socket.Connected)
				{
					Packet packet = new Packet(ReceiveMessage());

					Console.WriteLine(packet);

					switch (packet.Type)
					{
						default:
							Console.WriteLine("Type \"" + packet.Type + "\" was not recognized by the server.");
							break;
						case VrrgDataCollectionType.Command:
							break;
						case VrrgDataCollectionType.Message:
							break;
						case VrrgDataCollectionType.TransformUpdate:
							break;
					}
				}
			} catch (Exception ex) { Console.WriteLine(ex); }
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
