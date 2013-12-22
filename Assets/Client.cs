using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Client : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private string user;
	
	public bool usingChat = false;	//Can be used to determine if we need to stop player movement since we're chatting
	public GUISkin skin;//Skin
	public bool showChat = false;	//Show/Hide the chat
	//public Font font;
	
	//Private vars used by the script
	private string inputField = "";
	
	private Vector2 scrollPosition;
	private int width = 500;
	private int height = 600;
	private string playerName;
	private float lastUnfocusTime = 0;
	private Rect window;
	
	public List<ChatEntry> chatEntries = new List<ChatEntry>();
	
	public void Awake(){
        window = new Rect(Screen.width / 2 - width / 2, Screen.height - height + 5, width, height);
	}
	
	public void OnConnectedToServer() {
		playerName = PlayerPrefs.GetString("playerName");
		ShowChatWindow();
	}

	public void CloseChatWindow ()
	{
		showChat = false;
		inputField = "";
		chatEntries = new List<ChatEntry>();;
	}
	
	public void ShowChatWindow ()
	{
		showChat = true;
		inputField = "";
		chatEntries = new List<ChatEntry>();
	}
	
	public void OnGUI ()
	{
		if(!showChat){
			return;
		}
		
		GUI.skin = skin;
		//GUI.skin.font = font;
				
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length <= 0)
		{
			if(lastUnfocusTime+0.25<Time.time){
				usingChat=true;
				GUI.FocusWindow(5);
				GUI.FocusControl("Chat input field");
			}
		}
		
		window = GUI.Window (5, window, GlobalChatWindow, "");
	}


	public void GlobalChatWindow (int id) {
	
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
	    // it will use up any available screen space and make sure contents flow correctly.
	    // This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
	
		foreach (ChatEntry entry in chatEntries)
		{
			GUILayout.BeginHorizontal();
			if(entry.name==""){//Game message
				GUILayout.Label (entry.text);
			}else{
				GUILayout.Label (entry.name+": "+entry.text);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
	    GUILayout.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0)
		{
			HitEnter(inputField);
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);
		
		
		if(Input.GetKeyDown("mouse 0")){
			if(usingChat){
				usingChat=false;
				GUI.UnfocusWindow ();//Deselect chat
				lastUnfocusTime=Time.time;
			}
		}
	}

	public void HitEnter(string msg){
		msg = msg.Replace("\n", "");
		networkView.RPC("SendMsgToServer", RPCMode.Server, Network.player, msg);
		inputField = ""; //Clear line
		GUI.UnfocusWindow ();//Deselect chat
		lastUnfocusTime=Time.time;
		usingChat=false;
	}

	public void AddChatText(string name, string msg)
	{
		var entry = new ChatEntry();
		entry.name = name;
		entry.text = msg;
	
		chatEntries.Add(entry);
		
		//Remove old entries
		if (chatEntries.Count > 4){
			chatEntries.RemoveAt(0);
		}
	
		scrollPosition.y = 1000000;	
	}

    [RPC]
    public void TellServerOurName(string name, NetworkMessageInfo info) //dummy
    {
    }
	
    [RPC]
	public void SendMsgToServer(NetworkPlayer player, string msg) //dummy
	{
	} 	
	
	[RPC]
	public void SendMsgToClient(NetworkPlayer player, string msg)
	{
		Debug.Log("Entrou10");
		AddChatText("",msg);
		Debug.Log("Entrou11");
	}	
}
