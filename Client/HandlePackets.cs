using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VrRacingGameDataCollection;

namespace Client {
    static class HandlePackets {

        public static void Commands (Packet p) {
            if (p.Variables.ContainsKey("serverClosed") && p.Variables["serverClosed"] == "true" ||
                p.Variables.ContainsKey("disconnectClient") && p.Variables["disconnectClient"] == "true")
                Program.CloseConnection("Disconnected from server.");
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
