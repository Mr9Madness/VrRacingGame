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
        private System.Threading.Thread _listenThread = null;
    	void Start () 
        {
            string[] address = PlayerPrefs.GetString( "IP", ":" ).Split( ':' );

            Game.PlayerData.Socket.Connect( address[ 0 ], int.Parse( address[ 1 ] ) );

            Debug.Log( new Packet("dingen", "dignen", VrrgDataCollectionType.Command ) );

            Client.SendMessage(
                new Packet(
                    "test", 
                    "Server", 
                    VrrgDataCollectionType.Command, 
                    new [] { "username", "test" }
                )
            );

            if( !Client.HandlePassWord() )
                return;

            _listenThread = new Thread( Client.Listen );
        }
    }
}