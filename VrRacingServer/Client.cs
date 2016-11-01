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
                ServerPacket username = ReceiveMessage();

                Console.WriteLine(username.ToString());
                Console.WriteLine(username.Type);
                Console.WriteLine(username.Msg);

                if (username.Type != VrrgDataCollectionType.Command) {
                    Console.WriteLine("Unexpected message type \"" + username.Type + "\".");
                    return;
                }
                if (!username.Msg.Contains("username=")) {
                    Console.WriteLine("Message \"" + username.Msg + "\" does not contain expected \"username=\".");
                    return;
                }

                Username = username.Msg.Split('=')[1];
            } catch (Exception ex) {
                if (ex.ToString().Contains("actively refused"))
                    return;

                Console.WriteLine("\n" + ex + "\n");
            }
        }

        private ServerPacket ReceiveMessage() {
            try {
                NetworkStream getStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = getStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

                return new ServerPacket(actualRead);
            } catch (Exception ex) {
                Console.WriteLine("\"" + ex + "\"");
            }
            return null;
        }
    }
}
