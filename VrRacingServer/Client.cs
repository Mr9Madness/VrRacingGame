﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using VrRacingGameDataCollection;

namespace Server {
    class Client {
        private const int MAXBUFFERSIZE = 256;
        public string Username;
        public bool isClosing = false;
        public TcpClient Socket;
        public Thread ListenToClient;
        private NetworkStream receiveStream;

        public Client(TcpClient socket) {
            Console.WriteLine("New connection request.");
            Socket = socket;

            CheckNewClientInfo();
        }

        private Packet ReceiveMessage(bool logMessage = false) {
            try {
                receiveStream = Socket.GetStream();
                byte[] buffer = new byte[MAXBUFFERSIZE];

                int readCount = receiveStream.Read(buffer, 0, buffer.Length);
                List<byte> actualRead = new List<byte>(buffer).GetRange(0, readCount);

				if (logMessage) Console.WriteLine(Username + " > Server: " + Encoding.ASCII.GetString(actualRead.ToArray()));
				return new Packet(Encoding.ASCII.GetString(actualRead.ToArray()));
            } catch (Exception ex) {
                if (!ex.ToString().Contains("forcibly closed") && 
                    !ex.ToString().Contains("valid Vrrg Packet") &&
                    !ex.ToString().Contains("A blocking operation") &&
                    !ex.ToString().Contains("connection was aborted"))
                    Console.WriteLine("\"" + ex + "\"");
                
                if (!isClosing) Program.CloseClient(this);
            }
            return null;
        }

        public static string FirstCharToUpper (string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        private void Listen() {
            while (Socket != null && Socket.Connected) {
                Packet packet = ReceiveMessage();
                if (packet == null || packet == new Packet()) break;

                if (packet.Type != VrrgDataCollectionType.Command) {
                    Program.Broadcast(packet);
                    continue;
                }

                if (packet.Variables.Count == 0) continue;
                foreach (KeyValuePair<string, string> variable in packet.Variables) {
                    switch (variable.Key) {
                        default:
                            Console.WriteLine("WARNING: Unhandled variable in Vrrg Command Packet: \"" + variable.Key + "\"");
                            break;
                        case "disconnectRequest":
                            if (!isClosing)
                                Program.CloseClient(this);
                            break;
                    }
                }
            }
            if (!isClosing)
                Program.CloseClient(this);
        }

        private void CheckNewClientInfo() {
            bool isServerFull = Program.ClientList.Count >= Program.MaxPlayers;

            Program.SendMessage(this,
                new Packet(
                    "Server",
                    "Client",
                    VrrgDataCollectionType.Command,
                    new [] { "isServerFull", isServerFull.ToString() }
                )
            );

            if (isServerFull) {
                Console.WriteLine("Client rejected: Server is full.");
                return;
            }

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

            if (!Program.ClientList.ContainsKey(Username.ToLower())) {
                if (Program.Password == "") {
                    Program.ClientList.Add(Username.ToLower(), this);
                    ListenToClient = new Thread(Listen);
                    ListenToClient.Start();

                    Console.WriteLine(Username + " has joined the server.\n");
                }

				isAccepted = "true";
            } else Console.WriteLine("Connection request denied, username \"" + Username + "\" was already in use.\n");

            string[] variables;
            if (Program.Password != "") {
                variables = new[] {
                    "usernameAvailable", isAccepted, "passwordRequired", "true"
                };
            } else {
                variables = new [] {
                    "usernameAvailable", isAccepted, "passwordRequired", "false",
                    "serverName", Program.ServerName,
                    "maxPlayers", Program.MaxPlayers.ToString()
                };
            }

			Program.SendMessage(
				this,
				new Packet(
					"Server",
					Username,
					VrrgDataCollectionType.Command,
					variables
				)
			);

            if (Program.ClientList.Count > 0)
                Program.Broadcast(
                    new Packet(
                        "Server",
                        "All",
                        VrrgDataCollectionType.Command,
                        new[] { "clientList", string.Join("\\3\\", new List<string>(Program.ClientList.Keys).ToArray().Select(FirstCharToUpper)) }
                    )
                );

            if (isAccepted == "true") return;

            isAccepted = "false";
            while (isAccepted == "false") {
                packet = ReceiveMessage(true);

                if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("Password")) {
                    if (Program.Password == packet.Variables["Password"]) {
                        if (!Program.ClientList.ContainsKey(Username)) Program.ClientList.Add(Username, this);
                        ListenToClient = new Thread(Listen);
                        ListenToClient.Start();

                        Console.WriteLine(Username + " has joined the server.\n");
                        isAccepted = "true";
                    }

                    Program.SendMessage(
                        this,
                        new Packet(
                            "Server",
                            Username,
                            VrrgDataCollectionType.Command,
                            new[] { "passwordAccepted", isAccepted,
                                "serverName", Program.ServerName,
                                "maxPlayers", Program.MaxPlayers.ToString()
                            }
                        )
                    );
                } else Console.WriteLine("Received packet does not meet expectations of a Password-packet.");
            }

            if (Program.ClientList.Count > 0)
                Program.Broadcast(
                    new Packet(
                        "Server",
                        "All",
                        VrrgDataCollectionType.Command,
                        new[] { "clientList", string.Join("\\3\\", new List<string>(Program.ClientList.Keys).ToArray().Select(FirstCharToUpper)) }
                    )
                );
        }
    }
}