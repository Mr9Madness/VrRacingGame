using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetworkInfo : MonoBehaviour {
    public Text Status;
    public Text ServerIP;
    public Text ServerName;
    public Text PlayerCount;
    public ScrollRect PlayerList;
    public Button PlayerPrefab;

    private List<string> prevList = new List<string>();

    // Use this for initialization
    private void Start () {
	
	}
	
	// Update is called once per frame
	private void Update () {
	    if (Data.Player.Socket.Connected) {
	        if (ServerConnection.Server.ClientList != prevList) {
                for (int i = 0; i < PlayerList.transform.childCount; i++)
                    Destroy(PlayerList.content.GetChild(i).gameObject);

	            foreach (string client in ServerConnection.Server.ClientList) {
                    Button player = Instantiate(PlayerPrefab);
	                player.GetComponentInChildren<Text>().text = client;
	                player.transform.SetParent(PlayerList.transform);
	            }

	            prevList = ServerConnection.Server.ClientList;
	        }

	        PlayerCount.text = ServerConnection.Server.ClientList.Count + " / " + ServerConnection.Server.MaxPlayers;

	        if (Status.text == "Status: Connected.") return;
            Status.text = "Status: Connected.";
	        ServerIP.text = Data.Player.Socket.Client.LocalEndPoint.ToString();
            ServerName.text = ServerConnection.Server.ServerName;
        } else {
            if (Status.text == "Status: Not connected.") return;
            Status.text = "Status: Not connected.";
            ServerIP.text = "-";
            ServerName.text = "-";
            PlayerCount.text = "-";
	    }
	}
}
