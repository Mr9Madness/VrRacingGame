using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using VrRacingGameDataCollection;

namespace Server {
    class Client {
        private const int MAXBUFFERSIZE = 256;
        public string Username;
        public TcpClient Socket;

        public Client(TcpClient socket) {
            Console.WriteLine("New connection request.");
            Socket = socket;

            CheckNewClientInfo();
        }

        private Packet ReceiveMessage(bool logMessage = false) {
            try {
                NetworkStream getStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = getStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

				if (logMessage) Console.WriteLine(Username + " > Server: " + Encoding.ASCII.GetString(actualRead.ToArray()));
				return new Packet(Encoding.ASCII.GetString(actualRead.ToArray()));
            } catch (Exception ex) {
                if (!ex.ToString().Contains("forcibly closed")) Console.WriteLine("\"" + ex + "\"");

                Console.WriteLine("Client.cs");
                Program.CloseClient(this);
            }
            return null;
        }

        private void CheckNewClientInfo() {
            try {
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

                if (!Program.clientList.ContainsKey(Username.ToLower())) {
                    Program.clientList.Add(Username.ToLower(), this);
                    isAccepted = "true";

                    if (Program.password != "") Console.WriteLine(Username + " joined the server.\n");
                } else Console.WriteLine("Connection request denied, username \"" + Username + "\" was already in use.\n");

                string[] variables = { "usernameAvailable", isAccepted, "passwordRequired", Program.password != "" ? "true" : "false" };
                Program.SendMessage(
                    this,
                    new Packet(
                        "Server",
                        Username,
                        VrrgDataCollectionType.Command,
                        variables
                    )
                );

                if (isAccepted == "true") return;

                isAccepted = "false";
                while (isAccepted == "false") {
                    packet = ReceiveMessage(true);

                    if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("password")) {
                        if (Program.password == packet.Variables["password"]) {
                            isAccepted = "true";
                        }

                        Program.SendMessage(
                            this,
                            new Packet(
                                "Server",
                                Username,
                                VrrgDataCollectionType.Command,
                                new[] { "passwordAccepted", isAccepted }
                            )
                        );

                        Console.WriteLine(Username + " joined the server.\n");
                    } else Console.WriteLine("Received packet does not meet expectations of a password-packet.");
                }
            } catch (Exception ex) {
                if (!ex.ToString().Contains("actively refused")) Console.WriteLine("\n" + ex + "\n");

                Console.WriteLine("Client.cs");
                Program.CloseClient(this);
                
            }
        }
    }
}