﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VrRacingGameDataCollection {

    public enum VrrgDataCollectionType {
        None = 0,

        ChatMessage = 1,
        ChatCommand = 2,
        Message = 3,
        Command = 4
    }

    public static class VrrgParser {
        public static T ParseTo<T>(object dataToConvert) {
            object result; string data;

            switch (dataToConvert.GetType().Name.ToLower()) {
                default: throw new Exception("Data type \"" + dataToConvert.GetType().Name + "\" not recognized.\nSupported formats are:\n\nstring\nbyte[] (byte array)\nList<byte> (list of bytes)");
                case "string":
                    data = (string)dataToConvert;
                    break;
                case "byte[]":
                    data = ConvertToString((byte[])dataToConvert);
                    break;
                case "list`1":
                    try { List<byte> list = (List<byte>)dataToConvert; } catch (Exception ex) { throw new Exception("Only List<byte> is supported."); }

                    data = ConvertToString((List<byte>)dataToConvert);
                    break;
            }

            switch (typeof(T).Name.ToLower()) {
                default: throw new Exception("Given type \"" + typeof(T).Name + "\" not recognized");
                case "serverpacket":
                    result = ServerPacket.Parse(data);
                    break;
                case "clientpacket":
                    result = ClientPacket.Parse(data);
                    break;
            }


            return (T)result;
        }

        private static string ConvertToString (byte[] dataToConvert) {
            return Encoding.ASCII.GetString(dataToConvert);
        }

        private static string ConvertToString (List<byte> dataToConvert) {
            return Encoding.ASCII.GetString(dataToConvert.ToArray());
        }
    }

    public class Packet {
        public VrrgDataCollectionType Type = VrrgDataCollectionType.None;
    }

    public class ServerPacket : Packet {

        public static ServerPacket Parse(string dataToConvert) {
            ServerPacket sp = new ServerPacket();

            if (!dataToConvert.Contains("\\1\\")) throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new [] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\")) throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default: throw new Exception("\"" + item[0] + "\" is not recognized as a ServerPacket variable.");
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Message":
                        sp.Message = item[1];
                        break;
                }
            }

            return sp;
        }

        public override string ToString () {
            return "Type\\2\\" + Type + "\\1\\Message\\2\\" + Message;
        }

        /// <summary>
        /// Transfer the temporary converted info into the ServerPacket
        /// </summary>
        /// <param name="sp">The ServerPacket to transfer</param>
        private void TransferData (ServerPacket sp) {
            Type = sp.Type;
            Message = sp.Message;
        }

        /// <summary>
        /// The message that was used to convert into a ServerPacket
        /// </summary>
        public string Message = "";

        public ServerPacket () { }
        public ServerPacket (string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public ServerPacket (object dataToConvert) { TransferData(VrrgParser.ParseTo<ServerPacket>(dataToConvert)); }
    }

    public class ClientPacket : Packet {
        public static ClientPacket Parse (string dataToConvert) {
            ClientPacket sp = new ClientPacket();

            if (!dataToConvert.Contains("\\1\\")) throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\")) throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default: throw new Exception("\"" + item[0] + "\" is not recognized as a ClientPacket variable.");
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Message":
                        sp.Message = item[1];
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Transfer the temporary converted info into the ClientPacket
        /// </summary>
        /// <param name="sp">The ClientPacket to transfer</param>
        private void TransferData (ClientPacket sp) {
            Type = sp.Type;
            Message = sp.Message;
        }

        /// <summary>
        /// The message that was used to convert into a ClientPacket
        /// </summary>
        public string Message = "";

        public ClientPacket () { }
        public ClientPacket (string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public ClientPacket (object dataToConvert) { TransferData(VrrgParser.ParseTo<ClientPacket>(dataToConvert)); }
    }
}
