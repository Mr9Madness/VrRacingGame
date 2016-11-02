using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VrRacingGameDataCollection;

namespace Server {
    class Client {
        private const int MAXBUFFERSIZE = 256;
        public string Username;
        public TcpClient Socket;

        public Client(TcpClient socket) {
            try {
                Socket = socket;
				Packet packet = ReceiveMessage();

                Console.WriteLine("Packet: " + packet);
				Console.WriteLine("From: " + packet.From);
				Console.WriteLine("To: " + packet.To);
				Console.WriteLine("Type: " + packet.Type);
				Console.WriteLine("Variables: " + string.Join(" | ", packet.Variables.ToArray()));

                if (packet.Type != VrrgDataCollectionType.Command) {
                    Console.WriteLine("Unexpected packet type \"" + packet.Type + "\".");
                    return;
                }
				if (!packet.Variables.ContainsKey("username")) {
                    Console.WriteLine("Packet does not contain the expected \"username\" key.");
                    return;
                }

				Username = packet.Variables["username"];
				string isAccepted = "false";

				if (!Program.clientList.ContainsKey(Username.ToLower()))
				{
					Program.clientList.Add(Username.ToLower(), this);
					isAccepted = "true";

					if (Program.password != "") Console.WriteLine(Username + " joined the server.\n");
				}
				else Console.WriteLine("Connection request denied, username \"" + Username + "\" was already in use.\n");

				string[] variables = { "usernameAvailable", isAccepted };
				if (Program.password != "" && isAccepted == "true") variables = new string[] { "usernameAvailable", isAccepted, "password", Program.password };

				Program.SendMessage(
					this,
					new Packet(
						"Server",
						Username,
						VrrgDataCollectionType.Command,
						variables
					)
				);

				if (isAccepted == "true")
				{
					packet = ReceiveMessage();
					if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("password"))
					{
						if (Program.password == packet.Variables["password"]) Console.WriteLine(Username + " joined the server.\n");
						else isAccepted = "false";

						Program.SendMessage(
							this,
							new Packet(
								"Server",
								Username,
								VrrgDataCollectionType.Command,
								new string[] { "passwordAccepted", isAccepted }
							)
						);
					}
					else Console.WriteLine("Received packet does not meet expectations of a password-packet.");
				}


            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused"))
                    return;

                Console.WriteLine("\n" + ex + "\n");
            }
        }

        private Packet ReceiveMessage() {
            try {
                NetworkStream getStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = getStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

				Console.WriteLine(Encoding.ASCII.GetString(actualRead.ToArray()));
				return new Packet(Encoding.ASCII.GetString(actualRead.ToArray()));
            } catch (Exception ex) {
                Console.WriteLine("\"" + ex + "\"");
            }
            return null;
        }
    }
}
hoi