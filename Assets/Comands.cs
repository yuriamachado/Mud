using UnityEngine;
using System.Collections;
using System.Linq;

public class Comands {//: MonoBehaviour {

    public string Comando { get; set; }
	public string playerCommand { get; set; }
    public ComandsType TiposComando;
    public bool IsOpenDoor; 


    public static Comands ParseCommand(string cmd)
    {
        var retornoComando = new Comands();
        var comando = cmd.Trim().ToLower().Split(' ');
		if(comando[0] == "mover"){
			comando[0] = comando[0] + comando[1];
		}
		
        switch (comando[0])
        {
            case "ajuda":
                retornoComando.Comando = "";
                retornoComando.TiposComando = ComandsType.Ajuda;
                break;
            case "movernorte":
                retornoComando.Comando = "0";
                retornoComando.TiposComando = ComandsType.Direcao;
                break;
            case "moversul":
                retornoComando.Comando = "1";
                retornoComando.TiposComando = ComandsType.Direcao;
                break;
            case "moverleste":
                retornoComando.Comando = "2";
                retornoComando.TiposComando = ComandsType.Direcao;
                break;
            case "moveroeste":
                retornoComando.Comando = "3";
                retornoComando.TiposComando = ComandsType.Direcao;
                break;
            case "examinar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.DescricaoSala;
                break;
            case "usar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.UsarItem;
                break;
            case "pegar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.PegarItem;
                break;
            case "largar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.LargarItem;
                break;
			case "falar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.Falar;
                break;
		 	case "cochichar":
                retornoComando.Comando = comando[1];
                retornoComando.TiposComando = ComandsType.Cochichar;
				retornoComando.playerCommand = comando[2];
                break;
			case "inventorio":
                retornoComando.Comando = "";
                retornoComando.TiposComando = ComandsType.Inventorio;
                break;
			case "mapa":
                retornoComando.Comando = "";
                retornoComando.TiposComando = ComandsType.mapa;
                break;
            default:
                retornoComando.Comando = "";
                retornoComando.TiposComando = ComandsType.Invalido;
                break;
        }
        return retornoComando;
    }
	
    public static Comands ParseItemCommand(string cmd, ComandsType typeCommands, Room currentRoom, PlayerNode player)
    {
        var retornoComando = new Comands();
        var nomeItem = cmd;
        retornoComando.Comando = "Comando inválido";
        if(typeCommands == ComandsType.UsarItem)
        {
			var openDoor = false;
            foreach (var item in currentRoom.m_ItemsRoom)
            {
				
                if (item.m_NameItem == nomeItem)
                {
					var jogadorComItem = player.Inventory.Where(i => i.m_NameItem == nomeItem).Select(s => s.m_NameItem == nomeItem).FirstOrDefault();
                    if(jogadorComItem)
					{
						if (item.m_IsUsedItem)
	                    {
	                        retornoComando.Comando = "Item ja usado.";
	                        break;
	                    }
	                    else
	                    {
	                        if (item.m_GetItem)
	                        {
	                            retornoComando.Comando = "Usar item " + nomeItem;
	                            item.m_IsUsedItem = true;
								player.Inventory.Remove(item);
	                            
								break;
	                        }
	                    }
					}
					else{
				 		retornoComando.Comando = "Jogador esta sem o item.";
	                    break;
					}
                }
					
				
            }
			
			retornoComando.IsOpenDoor = currentRoom.m_ItemsRoom.All(a => a.m_IsUsedItem == true);

        }
        else if (typeCommands == ComandsType.PegarItem)
        {
            foreach (var item in currentRoom.m_ItemsRoom)
            {
                if (item.m_NameItem == nomeItem)
                {
                    if (item.m_GetItem)
                    {
                        retornoComando.Comando = "Item já foi pego.";
                        break;
                    }
                    else
                    {
                        retornoComando.Comando = "Item " + nomeItem +" adquerido.";
                        item.m_GetItem = true;
                        item.m_Collectable = false;
                        //item = GameObject.GetComponent<Room>();
                        player.Inventory.Add(item);
                        break;
                    }
                }
            }
        }
        else if (typeCommands == ComandsType.LargarItem)
        {
            foreach (var item in currentRoom.m_ItemsRoom)
            {
                if (item.m_NameItem == nomeItem)
                {
					var jogadorComItem = player.Inventory.Where(i => i.m_NameItem == nomeItem).Select(s => s.m_NameItem == nomeItem).FirstOrDefault();
                    if(jogadorComItem)
					{
	                    if (item.m_Collectable)
	                    {
	                        retornoComando.Comando = "Item já foi largado.";
	                        break;
	                    }
	                    else
	                    {
	                        retornoComando.Comando = "Deixou item " + nomeItem;
	                        item.m_GetItem = false;
	                        item.m_Collectable = true;
	                        player.Inventory.Remove(item);
	                        break;
	                    }
					}else{
						retornoComando.Comando = "O jogador esta sem esse item.";
	                    break;
					}
                }
            }
        }

        return retornoComando;
    }


}
