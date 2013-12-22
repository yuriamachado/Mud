using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item// : MonoBehaviour
{
    public string m_NameItem;
    public string m_DescItem;
    public List<Room> m_RoomExistItem;
    //Pegou o item.
    public bool m_GetItem;
    //Largou o item e fica true por padrão fica false.
    public bool m_Collectable;
    //usa o item 
    public bool m_IsUsedItem;

}
