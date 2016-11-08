using System;
using System.Collections.Generic;
using System.Linq;

namespace VrRacingGameDataCollection
{
    public enum VrrgDataCollectionType {
		None = 0,

		Command = 1,
		Message = 2,
		ChatMessage = 3,
		TransformUpdate = 4,
		MapData = 5
	}

	public static class VrrgParser {
		public static Packet Parse(string data) {
			Packet packet = new Packet();

			if (!data.Contains("\\1\\"))
                throw new Exception("String does not contain a valid Vrrg Packet.");

			string[] variables = data.Split(new [] { "\\1\\" }, StringSplitOptions.None);
			foreach (string pair in variables) {
				if (!pair.Contains("\\2\\"))
                    throw new Exception("String does not contain a valid Vrrg Packet.");

				string[] keyValue = pair.Split(new [] { "\\2\\" }, StringSplitOptions.None);
				switch (keyValue[0]) {
					default:
						packet.Variables.Add(keyValue[0], keyValue[1]);
					break;
					case "From":
						packet.From = keyValue[1];
						break;
					case "To":
						packet.To = keyValue[1];
						break;
					case "Type":
						VrrgDataCollectionType type;
						if (!Enum.TryParse(keyValue[1], true, out type))
                            throw new Exception("Type \"" + keyValue[1] + "\" is not an underlying value of VrrgDataCollectionType.");

						packet.Type = type;
						break;
				}
			}

			return packet;
		}


	}

	public class Packet {
		public string From = "";
		public string To = "";
		public VrrgDataCollectionType Type = VrrgDataCollectionType.None;
		public Dictionary<string, string> Variables = new Dictionary<string, string>();

		public Packet() { }
		public Packet(string data) {
			Packet p = VrrgParser.Parse(data);

			if (p != null) {
				From = p.From;
				To = p.To;
				Type = p.Type;
				Variables = p.Variables;
			}
			else Console.WriteLine("Packet was not parsed correctly.");
		}
		public Packet(string from, string to, VrrgDataCollectionType type, Dictionary<string, string> variables = null) {
			From = from;
			To = to;
			Type = type;
			if (variables != null) Variables = variables;
		}
		public Packet(string from, string to, VrrgDataCollectionType type, string[] variable) {
			From = from;
			To = to;
			Type = type;

		    if (variable.Length <= 0) return;
		    for (int i = 0; i < variable.Length; i += 2) {
		        Variables.Add(variable[i], variable[i + 1]);
		    }
		}

		public override string ToString() {
			string str = "From\\2\\" + From + "\\1\\To\\2\\" + To + "\\1\\Type\\2\\" + Type;

		    if (Variables.Count <= 0) return str;
		    str += "\\1\\" + string.Join("\\1\\", Variables.Select(pair => pair.Key + "\\2\\" + pair.Value).ToArray());

		    return str;
		}
	}
}
