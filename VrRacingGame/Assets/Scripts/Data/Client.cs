using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using VrRacingGameDataCollection;

namespace ServerConnection {

    static class Server {
        public static string ServerName = "";
        public static List<string> ClientList = new List<string>();
        public static int MaxPlayers = 0;

        public static void CleanVars() {
            ServerName = "";
            ClientList = new List<string>();
            MaxPlayers = 0;
        }
    }

public class Client : MonoBehaviour
    {
        public static bool isClosing;
        static bool Connected;

        public static void SendMessage( Packet packet, bool logMessage = true )
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes( packet.ToString() );

                NetworkStream sendStream = Game.PlayerData.Socket.GetStream();

                sendStream.Write( buffer, 0, buffer.Length );

                if(logMessage){}
                    //Console.WriteLine( Username + " > Server: " + packet );

            }
            catch( Exception ex ) 
            {
                if( !ex.ToString().Contains( "actively refused" ) && !ex.ToString().Contains( "forcibly close" ) ) 
                {} // Console.WriteLine( "\n" + ex + "\n" );
            
                if( !isClosing )
                    CloseConnection( "Disconnected from server." );
            }   
                
        }

        public static void CloseConnection( string message = "", bool safeDisconnect = true )
        {
            isClosing = true;

            if( Game.PlayerData.Socket != null && Game.PlayerData.Socket.Connected && safeDisconnect ) 
            {
                SendMessage
                (
                    new Packet
                    (
                        Game.PlayerData.UserName,
                        "Server",
                        VrrgDataCollectionType.Command,
                        new[] { "disconnectRequest", "true" }
                    )
                );
            }

            if (Game.PlayerData.Socket != null) Game.PlayerData.Socket.Close();
                        
            if( message.Trim( ' ' ).Length > 0 )
                Console.WriteLine( message );
        }

        public static void  Listen()
        {
            try
            {
                while( Game.PlayerData.Socket.Connected && !isClosing )
                {
                    Packet packet = new Packet( ReceiveMessage() );

                    switch( packet.Type ) {
                    default:
                        //Console.WriteLine("Type \"" + packet.Type + "\" was not recognized by the server.");
                        break;
                    case VrrgDataCollectionType.None:
                        //Console.WriteLine("Server received packet with type \"None\": " + packet);
                        break;

                    case VrrgDataCollectionType.Command:
                        HandlePackets.Commands( packet );
                        break;

                    case VrrgDataCollectionType.Message:
                        HandlePackets.Messages( packet );
                        break;

                    case VrrgDataCollectionType.ChatMessage:
                        HandlePackets.ChatMessages( packet );
                        break;

                    case VrrgDataCollectionType.MapData:
                        HandlePackets.MapDatas( packet );
                        break;

                    case VrrgDataCollectionType.PlayerUpdate:
                        HandlePackets.PlayerUpdates( packet );
                        break;
                    }
                }
            }
            catch( Exception ex )
            {
                if( !ex.ToString().Contains( "forcibly closed" ) &&
                    !ex.ToString().Contains( "Thread was being aborted" ) )
                {}//Console.WriteLine("\n" + ex + "\n");
                if( !isClosing )
                    CloseConnection( "Disconnected from server." );
            }
        }

        private static string ReceiveMessage( bool logMessage = true )
        {
            try
            {
                NetworkStream getStream = Game.PlayerData.Socket.GetStream();
                byte[] buffer = new byte[ 256 ];

                int readCount = getStream.Read( buffer, 0, buffer.Length );
                List< byte > actualRead = new List< byte >( buffer ).GetRange( 0, readCount );

                string message = Encoding.ASCII.GetString( actualRead.ToArray() );
                if( logMessage ){
                    Debug.Log( message );
                }
                    //Console.WriteLine( message );
                    
                return message;
            }
            catch( Exception ex ) 
            {
                if( !ex.ToString().Contains( "forcibly closed" ) && 
                    !ex.ToString().Contains("Thread was being aborted") ) 
                    { Debug.Log( "\n" + ex + "\n" ); } //Console.WriteLine( "\n" + ex + "\n" );
                if( !isClosing )
                    CloseConnection( "Disconnected from server." );
            }
            return null;   
        }

        public static bool HandlePassWord()
        {
            Packet p = new Packet( ReceiveMessage() );

            if( p != new Packet() && p.Type == VrrgDataCollectionType.Command &&
                p.Variables[ "usernameAvailable" ] != "false" ) 
            {
                if( p.Variables[ "passwordRequired" ] == "true" ) 
                {
                    
                }
                else if (p.Variables["passwordRequired"] == "false") 
                {
                    Console.WriteLine("Connected to \"" + p.Variables["serverName"] + "\"!");
                    Connected = true;
                }
                else Console.WriteLine("Password key not found in packet");
            }
            return Connected;
        }
    }
}