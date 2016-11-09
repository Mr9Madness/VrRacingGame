using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;
using VrRacingGameDataCollection;

namespace Client {

    class Program {
        private static string Ip = "127.0.0.1";
        private static int Port = 25001;

        static void Main(string[] args) {
            SetOptions();

            Console.WriteLine("=================== Virtual Reality Racing Game client ===================\n");

            Client.Connect(Ip, Port);
            while (Client.Socket != null && Client.Socket.Connected) {
                if (!Client.Connected) continue;

                string input = Console.ReadLine();
                if (input != null && input.Length > 7 && input.Substring(0,7).Contains("message")) {
                    Client.SendMessage(
                        new Packet(
                            Client.Username,
                            "All",
                            VrrgDataCollectionType.Message,
                            new[] { "Message", input.Substring(7) }
                        )
                    );
                }

                if (input == "disconnect") {
                    if (!Client.isClosing)
                        CloseConnection();
                    break;
                }

                if (input == "exit") {
                    if (!Client.isClosing)
                        CloseConnection();
                    Environment.Exit(0);
                }
            }

            Console.Write("Disconnected from server!\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void CloseConnection(string message = "") {
            Client.isClosing = true;

            if (Client.Socket != null && Client.Socket.Connected)
                Client.SendMessage(
                    new Packet(
                        Client.Username,
                        "Server",
                        VrrgDataCollectionType.Command,
                        new[] {"disconnectRequest", "true"}
                    )
                );
            
            
            Client.ListenToServer.Abort();
            Client.Socket?.Close();

            if (message.Trim(' ').Length > 0) Console.WriteLine(message);
        }

        /// <summary>
        /// Initializer for the client.
        /// </summary>
        private static void SetOptions() {
            int optionCount = 0;

            while (optionCount < 2) {
                switch (optionCount) {
                    default:
                        Console.Write("Option (" + optionCount + ") does not exist.\nThe program will now close.");
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;
                    case 0: // Set max players.
                        Console.Write("Username (3 - 32 characters): ");
                        string username = Console.ReadLine()?.Trim(' ') ?? "";

                        if (username?.Length < 3) {
                            Console.Clear();

                            Console.WriteLine("The username \"" + username + "\" is too short, minimal length is 3 characters.");
                            continue;
                        }

                        if (username?.Length > 32) {
                            string temp = "";
                            for (int i = 0; i < 32; i++)
                                temp += username[i];

                            Client.Username = temp;
                        } else {
                            Client.Username = username;
                        }

                        optionCount++;
                        break;
                    case 1: // Set IP address and port
                        Console.Write("Server IP: ");
                        string result = Console.ReadLine();

                        if (result?.Trim(' ') != "") {
                            // Check if port was given, if so split the result at ":" and update the variables
                            if (result?.IndexOf(':') != -1 && result?.IndexOf(':') != result?.Length) {
                                string[] temp = result.Split(':');
                                Ip = temp[0];

                                try { Port = Convert.ToInt16(temp[1]); } catch (Exception) {
                                    Console.Clear();
                                    Console.WriteLine("\nInvalid port \"" + temp[1] + "\".");

                                    continue;
                                }

                            } else
                                Ip = result;
                        } else
                            Ip = "127.0.0.1";

                        IPAddress garbage;
                        if (IPAddress.TryParse(Ip, out garbage))
                            optionCount++;
                        else {
                            Console.Clear();
                            Console.WriteLine("\nInvalid IP Address \"" + Ip + "\".");
                        }
                        break;
                }

                Console.Clear();
            }
        }
    }
}
