using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VrRacingGameDataCollection {

    public enum VrrgDataCollectionType {
        None =              0,

        Command =           1,
        Message =           2,
        ChatMessage =       3,
        TransformUpdate =   4,
        MapData =           5
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
                case "transformupdate":
                    result = TransformUpdate.Parse(data);
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
        public string From = "";
        public string To = "";

        public VrrgDataCollectionType Type = VrrgDataCollectionType.None;
    }

    public class ServerPacket : Packet {

        /// <summary>
        /// Parses the string into a ServerPacket object.
        /// </summary>
        /// <param name="dataToConvert">The string to convert</param>
        /// <returns>An object containing the converted string</returns>
        public static ServerPacket Parse(string dataToConvert) {
            ServerPacket sp = new ServerPacket();

            if (!dataToConvert.Contains("\\1\\")) throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new [] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\")) throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default: throw new Exception("\"" + item[0] + "\" is not recognized as a ServerPacket variable.");
                    case "From":
                        sp.From = item[1];
                        break;
                    case "To":
                        sp.To = item[1];
                        break;
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Message":
                        sp.Msg = item[1];
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Converts the current ServerPacket object into a string.
        /// </summary>
        /// <returns>The converted string</returns>
        public override string ToString () {
            return
                "From\\2\\" + From + "\\1\\" +
                "To\\2\\" + To + "\\1\\" +
                "Type\\2\\" + Type + "\\1\\" +
                "Msg\\2\\" + Msg;
        }

        /// <summary>
        /// Transfer the temporary converted info into the ServerPacket
        /// </summary>
        /// <param name="sp">The ServerPacket to transfer</param>
        private void TransferData (ServerPacket sp) {
            From = sp.From;
            To = sp.To;
            Type = sp.Type;

            Msg = sp.Msg;
        }

        /// <summary>
        /// The message that was used to convert into a ServerPacket
        /// </summary>
        public string Msg = "";

        public ServerPacket () { }
        public ServerPacket (string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public ServerPacket (object dataToConvert) { TransferData(VrrgParser.ParseTo<ServerPacket>(dataToConvert)); }
    }

    public class Command : Packet {
        /// <summary>
        /// Parses the string into a Command object.
        /// </summary>
        /// <param name="dataToConvert">The string to convert</param>
        /// <returns>An object containing the converted string</returns>
        public static Command Parse(string dataToConvert) {
            Command sp = new Command();

            if (!dataToConvert.Contains("\\1\\"))
                throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\"))
                    throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default:
                        throw new Exception("\"" + item[0] + "\" is not recognized as a Command variable.");
                    case "From":
                        sp.From = item[1];
                        break;
                    case "To":
                        sp.To = item[1];
                        break;
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Msg":
                        sp.Msg = item[1];
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Converts the current Command object into a string.
        /// </summary>
        /// <returns>The converted string</returns>
        public override string ToString() {
            return
                "From\\2\\" + From + "\\1\\" +
                "To\\2\\" + To + "\\1\\" +
                "Type\\2\\" + Type + "\\1\\" +
                "Msg\\2\\" + Msg;
        }

        /// <summary>
        /// The message that was used to convert into a Command
        /// </summary>
        public string Msg = "";

        /// <summary>
        /// Transfer the temporary converted info into the Command
        /// </summary>
        /// <param name="sp">The Command to transfer</param>
        private void TransferData(Command sp) {
            From = sp.From;
            To = sp.To;
            Type = sp.Type;
            Msg = sp.Msg;
        }

        public Command() { }
        public Command(string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public Command(object dataToConvert) { TransferData(VrrgParser.ParseTo<Command>(dataToConvert)); }
    }

    public class Message : Packet {
        /// <summary>
        /// Parses the string into a Message object.
        /// </summary>
        /// <param name="dataToConvert">The string to convert</param>
        /// <returns>An object containing the converted string</returns>
        public static Message Parse(string dataToConvert) {
            Message sp = new Message();

            if (!dataToConvert.Contains("\\1\\"))
                throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\"))
                    throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default:
                        throw new Exception("\"" + item[0] + "\" is not recognized as a Message variable.");
                    case "From":
                        sp.From = item[1];
                        break;
                    case "To":
                        sp.To = item[1];
                        break;
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Msg":
                        sp.Msg = item[1];
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Converts the current Message object into a string.
        /// </summary>
        /// <returns>The converted string</returns>
        public override string ToString() {
            return
                "From\\2\\" + From + "\\1\\" +
                "To\\2\\" + To + "\\1\\" +
                "Type\\2\\" + Type + "\\1\\" +
                "Msg\\2\\" + Msg;
        }

        /// <summary>
        /// The message that was used to convert into a Message
        /// </summary>
        public string Msg = "";

        /// <summary>
        /// Transfer the temporary converted info into the Message
        /// </summary>
        /// <param name="sp">The Message to transfer</param>
        private void TransferData(Message sp) {
            From = sp.From;
            To = sp.To;
            Type = sp.Type;
            Msg = sp.Msg;
        }

        public Message() { }
        public Message(string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public Message(object dataToConvert) { TransferData(VrrgParser.ParseTo<Message>(dataToConvert)); }
    }

    public class ChatMessage : Packet {
        /// <summary>
        /// Parses the string into a ChatMessage object.
        /// </summary>
        /// <param name="dataToConvert">The string to convert</param>
        /// <returns>An object containing the converted string</returns>
        public static ChatMessage Parse(string dataToConvert) {
            ChatMessage sp = new ChatMessage();

            if (!dataToConvert.Contains("\\1\\"))
                throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\"))
                    throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default:
                        throw new Exception("\"" + item[0] + "\" is not recognized as a ChatMessage variable.");
                    case "From":
                        sp.From = item[1];
                        break;
                    case "To":
                        sp.To = item[1];
                        break;
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Msg":
                        sp.Msg = item[1];
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Converts the current ChatMessage object into a string.
        /// </summary>
        /// <returns>The converted string</returns>
        public override string ToString() {
            return
                "From\\2\\" + From + "\\1\\" +
                "To\\2\\" + To + "\\1\\" +
                "Type\\2\\" + Type + "\\1\\" +
                "Msg\\2\\" + Msg;
        }

        /// <summary>
        /// The ChatMessage that was used to convert into a ChatMessage
        /// </summary>
        public string Msg = "";

        /// <summary>
        /// Transfer the temporary converted info into the ChatMessage
        /// </summary>
        /// <param name="sp">The ChatMessage to transfer</param>
        private void TransferData(ChatMessage sp) {
            From = sp.From;
            To = sp.To;
            Type = sp.Type;
            Msg = sp.Msg;
        }

        public ChatMessage() { }
        public ChatMessage(string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public ChatMessage(object dataToConvert) { TransferData(VrrgParser.ParseTo<ChatMessage>(dataToConvert)); }
    }

    public class TransformUpdate : Packet {
        /// <summary>
        /// Parses the string into a TransformUpdate object.
        /// </summary>
        /// <param name="dataToConvert">The string to convert</param>
        /// <returns>An object containing the converted string</returns>
        public static TransformUpdate Parse(string dataToConvert) {
            TransformUpdate sp = new TransformUpdate();

            if (!dataToConvert.Contains("\\1\\"))
                throw new Exception("Incorrect string format (\"\\1\\\" separator not found):\n\n" + dataToConvert);
            string[] data = dataToConvert.Split(new[] { "\\1\\" }, StringSplitOptions.None);
            foreach (string variable in data) {
                if (!dataToConvert.Contains("\\2\\"))
                    throw new Exception("Incorrect string format (\"\\s2\\\" separator not found):\n\n" + dataToConvert);
                string[] item = variable.Split(new[] { "\\2\\" }, StringSplitOptions.None);

                switch (item[0]) {
                    default:
                        throw new Exception("\"" + item[0] + "\" is not recognized as a TransformUpdate variable.");
                    case "From":
                        sp.From = item[1];
                        break;
                    case "To":
                        sp.To = item[1];
                        break;
                    case "Type":
                        VrrgDataCollectionType type;
                        if (!Enum.TryParse(item[1], true, out type))
                            throw new Exception("Type \"" + item[1] + "\" is not an underlying value of VrrgDataCollectionType.");

                        sp.Type = type;
                        break;
                    case "Position":
                        string[] posItems = item[1].Split(',');
                        sp.Position = new[] { (float)Convert.ToDouble(posItems[0]), (float)Convert.ToDouble(posItems[1]), (float)Convert.ToDouble(posItems[2]) };
                        break;
                    case "Rotation":
                        string[] rotItems = item[1].Split(',');
                        sp.Rotation = new[] { (float)Convert.ToDouble(rotItems[0]), (float)Convert.ToDouble(rotItems[1]), (float)Convert.ToDouble(rotItems[2]) };
                        break;
                }
            }

            return sp;
        }

        /// <summary>
        /// Converts the current TransformUpdate object into a string.
        /// </summary>
        /// <returns>The converted string</returns>
        public override string ToString() {
            return 
                "From\\2\\" + From + "\\1\\" +
                "To\\2\\" + To + "\\1\\" +
                "Type\\2\\" + Type + "\\1\\" +
                "Position\\2\\" + string.Join(",", Position) + "\\1\\" +
                "Rotation\\2\\" + string.Join(",", Rotation);
        }

        public float[] Position = new float[3];
        public float[] Rotation = new float[3];

        /// <summary>
        /// Transfer the temporary converted info into the TransformUpdate.
        /// </summary>
        /// <param name="sp">The TransformUpdate to transfer</param>
        private void TransferData(TransformUpdate sp) {
            From = sp.From;
            To = sp.To;
            Type = sp.Type;

            Position = sp.Position;
            Rotation = sp.Rotation;
        }

        public TransformUpdate() { }
        public TransformUpdate(string dataToConvert) { TransferData(Parse(dataToConvert)); }
        public TransformUpdate(object dataToConvert) { TransferData(VrrgParser.ParseTo<TransformUpdate>(dataToConvert)); }
    }
}
