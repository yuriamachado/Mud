using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Map {
	
	public string mapString { get; set; }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static Map ReadMap(PlayerNode player)
	{
		var mapa = new Map();
		string stringMapa = "";
		try 
			{
				//Pass the file path and file name to the StreamReader constructor
				//StreamReader sr = new StreamReader("J:\\map2.txt", System.Text.Encoding.ASCII);

				//Read the first line of text
				//var line = sr.ReadLine();
	
				
				// create reader and open file
				using (StreamReader reader = new StreamReader("J:\\map2.txt", System.Text.Encoding.ASCII))
				{
					// read all contents
					stringMapa = reader.ReadToEnd();
				}
				
				//Continue to read until you reach end of file
//				while (line != null) 
//				{
//					//write the lie to console window
//					//Console.WriteLine(line);
//					stringMapa += line;	
//					//networkView.RPC("SendMsgToClient", player.networkPlayer, player.networkPlayer, line);
//					//Read the next line
//					line = sr.ReadLine();
//				}

				//close the file
				//reader.Close();
				//Console.ReadLine();
				mapa.mapString = stringMapa;
			}
			catch(Exception e)
			{
				//networkView.RPC("SendMsgToClient", player.networkPlayer, player.networkPlayer, "Exception: " + e.Message);
				//Console.WriteLine("Exception: " + e.Message);
			}
			finally 
			{
			 	//networkView.RPC("SendMsgToClient", player.networkPlayer, player.networkPlayer, "Executing finally block.");
				//Console.WriteLine("Executing finally block.");
			}
		return mapa;
		//System.IO.StreamReader reader = new System.IO.StreamReader(fileName,  System.Text.Encoding.ASCII);
	}
	
}
