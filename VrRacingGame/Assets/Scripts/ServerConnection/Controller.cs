using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using VrRacingGameDataCollection;

namespace ServerConnection
{
    public class Controller : MonoBehaviour
    {
        private Thread _listenThread = null;
    	void Start () 
        {
            string[] address = PlayerPrefs.GetString( "IP", ":" ).Split( ':' );

            Data.Player.Socket.Connect( address[ 0 ], int.Parse( address[ 1 ] ) );

            Debug.Log( Client.ReceiveMessage() );

    	    Packet packet = new Packet( Client.ReceiveMessage() );
            if (packet.Type == VrrgDataCollectionType.Command && packet.Variables.ContainsKey("isServerFull"))
                if (packet.Variables["isServerFull"].ToLower() == "true") {
                    Client.CloseConnection("Disconnected from server: Server is full.");
                    return;
                }

            Client.SendMessage(
                new Packet(
                    "test", 
                    "Server", 
                    VrrgDataCollectionType.Command, 
                    new [] { "username", "test" }
                )
            );

            if( !Client.HandlePassword() )
                return;

            _listenThread = new Thread( Client.Listen );
        }
    }
}