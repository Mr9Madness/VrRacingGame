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
            Console.WriteLine("New connection request.");
            
            Socket = socket;
			Packet packet = ReceiveMessage();

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

				if (Program.password == "") Console.WriteLine(Username + " joined the server.\n");
			}
			else Console.WriteLine("Connection request denied, username \"" + Username + "\" was already in use.\n");

			string[] variables = { "usernameAvailable", isAccepted, "passwordRequired", "false" };
			if (Program.password != "" && isAccepted == "true") variables = new [] { "usernameAvailable", isAccepted, "passwordRequired", "true" };

			Program.SendMessage(
				this,
				new Packet(
					"Server",
					Username,
					VrrgDataCollectionType.Command,
					variables
				)
			);

            if (isAccepted != "true") return;

            isAccepted = "false";
            while (isAccepted == "false") {
                packet = ReceiveMessage();

                if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("password")) {
                    if (Program.password == packet.Variables["password"]) {
                        Console.WriteLine(Username + " joined the server.\n");
                        isAccepted = "true";
                    }

                    Program.SendMessage(
                        this,
                        new Packet(
                            "Server",
                            Username,
                            VrrgDataCollectionType.Command,
                            new [] {"passwordAccepted", isAccepted}
                            )
                        );
                } else Console.WriteLine("Received packet does not meet expectations of a password-packet.");
            }
        }

        private Packet ReceiveMessage(bool logMessage = true) {
            NetworkStream getStream = Socket.GetStream();
            byte[] buffer = new byte[MAXBUFFERSIZE];

            int readCount = getStream.Read(buffer, 0, buffer.Length);
            List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);
            string str = Encoding.ASCII.GetString(actualRead.ToArray());

            if (logMessage) Console.WriteLine(str);
			return new Packet(str);
        }
    }
}