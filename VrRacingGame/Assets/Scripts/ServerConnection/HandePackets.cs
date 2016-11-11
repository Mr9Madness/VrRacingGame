using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrRacingGameDataCollection;

namespace ServerConnection {
    static class HandlePackets {
        public static void Commands(Packet p) {
            foreach (KeyValuePair<string, string> variable in p.Variables) {
                switch (variable.Key) {
                    default:
                        Console.WriteLine("WARNING: Unhandled variable in Vrrg Command Packet: \"" + variable.Key + "\"");
                        break;
                    case "serverClosed": case "disconnectClient":
                        if (variable.Value == "true")
                            if (!Client.isClosing)
                                Client.CloseConnection("Disconnected from server.", false);
                        break;
                    case "clientList":
                        Server.ClientList = new List<string>(variable.Value.Split(new[] { "\\3\\" }, StringSplitOptions.None));
                        break;
                }
            }
        }

        public static void Messages(Packet p) {
            // Show this in the in-game chatbox/debugbox/noticebox/toasts
        }

        public static void ChatMessages(Packet p) {
            // Show this in the in-game chatbox
        }

        public static void MapDatas(Packet p) {
            // Use p.Variables["json"] and convert it to a map-object
        }

        public static void PlayerUpdates(Packet p) {
            // Use p.From, p.Variables["position"] and p.Variables["rotation"] (additionally p.Variables["speed"] too)
            // to make other players move.
        }

    }
}
