using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrRacingGameDataCollection;

namespace Client {
    static class HandlePackets {

        public static void Commands (Packet p) {
            foreach (KeyValuePair<string, string> variable in p.Variables) {
                switch (variable.Key) {
                    default:
                        Console.WriteLine("WARNING: Unhandled variable in Vrrg Command Packet: \"" + variable.Key + "\"");
                        break;
                    case "serverClosed": case "disconnectClient":
                        if (variable.Value == "true")
                            if (!Client.isClosing)
                                Program.CloseConnection("Disconnected from server.");
                        break;
                    case "clientList":
                        Console.WriteLine("In CASE");
                        Server.ClientList = new List<string>(variable.Value.Split(new [] { "\\3\\" }, StringSplitOptions.None));
                        break;
                }
            }
        }

        public static void Messages (Packet p) {

        }

        public static void ChatMessages (Packet p) {

        }

        public static void MapDatas (Packet p) {

        }

        public static void TransformUpdates (Packet p) {

        }

    }
}
