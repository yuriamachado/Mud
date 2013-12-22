using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//the Serializable attribute is key to what makes the custom class visible in the editor !
[System.Serializable]
public class Room {//: MonoBehaviour {
	
	//public Room[] m_Links;
    //[SerializeField]
    public int m_IdRoom;
    //[SerializeField]
	public string m_NameRoom;
    //[SerializeField]
	public string m_DescRoom;
    //[SerializeField]
	public List<Item> m_ItemsRoom;
    //[SerializeField]
    public List<PlayerNode> m_PlayerSalaRoom;
    //[SerializeField]
    public Enemy[] m_EnemysSalaRoom;
    //[SerializeField]
    public Dictionary<Directions, Room> m_DirectionsRoom;
    //[SerializeField]
    public bool m_IsOpen;

    //public static Room CreateRoomOne()
    //{
    //    var room = new Room();
    //    room.m_IdRoom = 1;
    //    room.m_Name = "Entrada da caverna.";
    //    room.m_Desc = "Essa é a entrada da caverna que você encontrou";
    //    room.m_alreadyExist = true;
    //    room.m_Directions.Add(Directions.Norte, new Room());

    //    return room;
    //}
}
