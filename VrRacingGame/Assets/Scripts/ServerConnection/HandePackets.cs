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
            Data.Level.ToJsonFile(p.Variables["json"]);
            Game.TileMap.MapName = p.Variables["levelName"];
            // Use p.Variables["json"] and convert it to a map-object
        }

        public static void PlayerUpdates(Packet p) {
            if (!Data.Network.Players.ContainsKey(p.Variables["playerName"])) {
                UnityEngine.Debug.Log(p.Variables["playerName"] + " is not connected, therefor it's transform cannot be updated.");
                return;
            }

            Game.CarController player = Data.Network.Players[p.Variables["playerName"]];
            UnityEngine.Vector3 position = CreateVector3FromString(p.Variables["position"]);
            UnityEngine.Quaternion rotation = CreateQuaternionFromString(p.Variables["rotation"]);

            player.transform.position = position;
            player.transform.rotation = rotation;

            Data.Network.Players[p.Variables["playerName"]] = player;
        }

        private static UnityEngine.Vector3 CreateVector3FromString(string str) {
            float[] pos = str.Split(',').Select(Convert.ToDouble).Cast<float>().ToArray();

            return new UnityEngine.Vector3(pos[0], pos[1], pos[2]);
        }

        private static UnityEngine.Quaternion CreateQuaternionFromString(string str) {
            float[] pos = str.Split(',').Select(Convert.ToDouble).Cast<float>().ToArray();

            return new UnityEngine.Quaternion(pos[0], pos[1], pos[2], pos[3]);
        }

    }
}
