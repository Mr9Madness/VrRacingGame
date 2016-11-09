//using UnityEngine;
//using System.Collections;
//using System.Net.Sockets;
//using System.Net;
//using VrRacingGameDataCollection;
//
//public class Controller : MonoBehaviour
//{
//	void Start () 
//    {
//        string[] address = PlayerPrefs.GetString( "IP", "" ).Split( ':', 1 );
//
//        TcpClient client = new TcpClient();
//        client.Connect( address[ 0 ], int.Parse( address[ 1 ] ) );
//
//        SendMessage(
//            new Packet(
//                "test", 
//                "Server", 
//                VrrgDataCollectionType.Command, 
//                new [] { "username", "test" }
//            )
//        );	
//    }
//	
//	void Update () 
//    {
//	    s
//	}
//}