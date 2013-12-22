﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class Connect : MonoBehaviour {
	
	public string connectToIP = "127.0.0.1";
	public int connectPort = 25001;
	public string nameOfCharacter = "";
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void OnGUI(){
		
		if (Network.peerType == NetworkPeerType.Disconnected){
		//We are currently disconnected: Not a client or host
		GUILayout.Label("Connection status: Disconnected");
		
		connectToIP = GUILayout.TextField(connectToIP, GUILayout.MinWidth(100));
		connectPort = Convert.ToInt32(GUILayout.TextField(connectPort.ToString()));
		nameOfCharacter = GUILayout.TextField(nameOfCharacter);
		
		
		GUILayout.BeginVertical();
		
			
			
			if (GUILayout.Button ("Connect as client"))
			{
				//Connect to the "connectToIP" and "connectPort" as entered via the GUI
				//Ignore the NAT for now
				Network.useNat = false;
				Network.Connect(connectToIP, connectPort);
				PlayerPrefs.SetString("playerName", nameOfCharacter);
				
			}
			
			if (GUILayout.Button ("Start Server"))
			{
				//Start a server for 32 clients using the "connectPort" given via the GUI
				//Ignore the nat for now
				PlayerPrefs.SetString("playerName", nameOfCharacter);
				
				Network.useNat = false;
				Network.InitializeServer(32, connectPort);
			}
		
		GUILayout.EndVertical();
		
		
		}else{
			//We've got a connection(s)!
			
			if (Network.peerType == NetworkPeerType.Connecting){
			
				GUILayout.Label("Connection status: Connecting");
				
			} else if (Network.peerType == NetworkPeerType.Client){
				
				GUILayout.Label("Connection status: Client!");
				GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );
                //networkView.RPC("TellServerOurName", RPCMode.Server, nameOfCharacter);
				
			} else if (Network.peerType == NetworkPeerType.Server){
				
				GUILayout.Label("Connection status: Server!");
				GUILayout.Label("Connections: " + Network.connections.Length);
				if(Network.connections.Length>=1){
					GUILayout.Label("Ping to first player: "+Network.GetAveragePing(  Network.connections[0] ) );
				}			
			}
	
			if (GUILayout.Button ("Disconnect"))
			{
				Network.Disconnect(200);
			}
		}
	}

    //[RPC]
    //public void TellServerOurName(string name, NetworkMessageInfo info) //dummy
    //{
    //}

	//Client functions called by Unity
	public void OnConnectedToServer() {
		Debug.Log("This CLIENT has connected to a server");	
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("This SERVER OR CLIENT has disconnected from a server");
	}

	public void OnFailedToConnect(NetworkConnectionError error){
		Debug.Log("Could not connect to server: "+ error);
	}


	//Server functions called by Unity
	public void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player connected from: " + player.ipAddress +":" + player.port);
	}

	public void OnServerInitialized() {
		Debug.Log("Server initialized and ready");
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
		Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
	}


	// OTHERS:
	// To have a full overview of all network functions called by unity
	// the next four have been added here too, but they can be ignored for now

	public void OnFailedToConnectToMasterServer(NetworkConnectionError info){
		Debug.Log("Could not connect to master server: "+ info);
	}

	public void OnNetworkInstantiate (NetworkMessageInfo info) {
		Debug.Log("New object instantiated by " + info.sender);
	}

	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		//Custom code here (your code!)
	}
	
	
}
