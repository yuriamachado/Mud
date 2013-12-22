using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
//using UnityEditor;



public class ChatEntry
{
    public string name = "";
    public string text = "";
}

//this limits the editor to running on object that have type CameraLocationHolder
//[CustomEditor(typeof(Room))]
public class Server : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //public List<Room> RoomServer;

    public string playerName;

    public bool isRoomsCreate = false;
    [SerializeField]
    public Room[] rooms = new Room[5];
	[SerializeField]
	public Room room1 = new Room();
	[SerializeField]
	public Room room2 = new Room();
	[SerializeField]
	public Room room3 = new Room();
	[SerializeField]
	public Room room4 = new Room();
	[SerializeField]
	public Room room5 = new Room();
    [SerializeField]
    public List<PlayerNode> playerList = new List<PlayerNode>();

    public void OnServerInitialized()
    {
        Debug.Log("Servidor Iniciado.");
    }

    //A handy wrapper function to get the PlayerNode by networkplayer
    public PlayerNode GetPlayerNode(NetworkPlayer networkPlayer)
    {
        foreach (PlayerNode entry in playerList)
        {
            if (entry.networkPlayer == networkPlayer)
            {
                return entry;
            }
        }
        Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
        return null;
    }

    //Server function
    public void OnPlayerDisconnected(NetworkPlayer player)
    {
        //Remove player from the server list
        playerList.Remove(GetPlayerNode(player));
		if(playerList.Count == 0)
		{
			isRoomsCreate = true;
			Array.Clear(rooms, 0, 5);
		}
    }

    public void OnDisconnectedFromServer()
    {
        Debug.Log("Desconectado do servidor.");
    }

    //Server function
    public void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Entrou2");
        //Captura o Nome do personagem que conectou
        playerName = PlayerPrefs.GetString("playerName");
		var nameOfPlayer = playerName.Trim().ToLower().Replace(" ","");
		var existePlayerMesmoNome = playerList.Where(j => j.playerName == nameOfPlayer).Select(s => s.playerName == nameOfPlayer).FirstOrDefault();
        if(!existePlayerMesmoNome){
		PlayerNode newEntry = new PlayerNode();
        newEntry.playerName = nameOfPlayer;
        newEntry.networkPlayer = player;
		newEntry.Inventory = new List<Item>();
        //Adiciona a lista de player.
        playerList.Add(newEntry);
        //Cria as salas.
        if (!isRoomsCreate)
        {
            CreateRooms();
        }
        
        rooms[0].m_PlayerSalaRoom.Add(newEntry);
        
        networkView.RPC("SendMsgToClient", RPCMode.All, Network.player, "O jogador " + playerName + " acessou servidor!");
        networkView.RPC("SendMsgToClient", player, Network.player, IniciarMud());
        networkView.RPC("SendMsgToClient", player, Network.player, "Você está na sala " + rooms[0].m_NameRoom);
        
		}else{
			networkView.RPC("SendMsgToClient", player, Network.player, "Nome de personagem já existe.");
			Network.CloseConnection(player,true);
		}
        Debug.Log("Entrou3");
    }

    [RPC]
    public void SendMsgToServer(NetworkPlayer player, string msg)
    {
        var retorno = Comands.ParseCommand(msg);
        //var playerMoves = false;
        var jogadorAtual = playerList.Where(j => j.networkPlayer == player).First();
        //Procura  nas salas pelo jogador!
        for (int i = 0; i < rooms.Length; i++)
        {
            //Verifica em que sala se encontra o player!
            var existeJogadorNaSala = rooms[i].m_PlayerSalaRoom.Where(j => j.networkPlayer == jogadorAtual.networkPlayer).Select(jo => jo.networkPlayer == jogadorAtual.networkPlayer);
            if (existeJogadorNaSala.FirstOrDefault())
            {
                //Informações da sala/item
                if (retorno.TiposComando == ComandsType.DescricaoSala)
                {
					if(retorno.Comando == "sala"){
                    	networkView.RPC("SendMsgToClient", player, player, rooms[i].m_DescRoom);
					}else{
						foreach (var item in rooms[i].m_ItemsRoom) {
							if(item.m_NameItem == retorno.Comando){
								networkView.RPC("SendMsgToClient", player, player, item.m_DescItem);
								break;
							}
						}
					}
                }
                //comando andar
                else if (retorno.TiposComando == ComandsType.Direcao)
                {
                    Directions directionValue = (Directions)Enum.Parse(typeof(Directions), retorno.Comando);
                    var existRoom = rooms[i].m_DirectionsRoom.Where(d => d.Key == directionValue && d.Value.m_IsOpen == true).Select(s => s.Key == directionValue).FirstOrDefault();
                     //rooms[i].m_Directions[]
                    Debug.Log(existRoom);
                    if (existRoom)
                    {
                        rooms[i].m_DirectionsRoom[directionValue].m_PlayerSalaRoom.Add(jogadorAtual);
                        rooms[i].m_PlayerSalaRoom.Remove(jogadorAtual);
                        networkView.RPC("SendMsgToClient", player, Network.player, "Você está na sala " + rooms[i].m_DirectionsRoom[directionValue].m_NameRoom);
                        //networkView.RPC("SendMsgToClient", player, player, rooms[i].m_Directions[directionValue].m_Desc);
                        break;
                    }
                    else
                    {
                        networkView.RPC("SendMsgToClient", player, player, "Não é possivel ir nessa direção");
                    }
                }
                else if (retorno.TiposComando == ComandsType.UsarItem)
                {
                    var retornoComandoUsarItem = Comands.ParseItemCommand(retorno.Comando, ComandsType.UsarItem, rooms[i], jogadorAtual);
					networkView.RPC("SendMsgToClient", player, Network.player, retornoComandoUsarItem.Comando);
					if(retornoComandoUsarItem.IsOpenDoor){
						switch(i){
							case 0:
							rooms[0].m_DirectionsRoom[Directions.Norte].m_IsOpen = true;
							break;
							case 1:
							rooms[1].m_DirectionsRoom[Directions.Leste].m_IsOpen = true;
							break;
							case 2:
							rooms[2].m_DirectionsRoom[Directions.Norte].m_IsOpen = true;
							break;
							case 3:
							rooms[2].m_DirectionsRoom[Directions.Leste].m_IsOpen = true;
							var mensagem = "Com cada pedra em seu respectivo lugar um báu aparece e dentro tem uma chave. Será que ela pode abrir a porta tracanda??? Volte lá e confirá";
							networkView.RPC("SendMsgToClient", RPCMode.All, Network.player, mensagem);
							break;
							default:
							break;
						}
						
						networkView.RPC("SendMsgToClient", RPCMode.All, Network.player, "Conseguiu acesso a próxima sala");
					}
                    
                }
                else if (retorno.TiposComando == ComandsType.PegarItem)
                {
                    var retornoComandoUsarItem = Comands.ParseItemCommand(retorno.Comando, ComandsType.PegarItem, rooms[i], jogadorAtual);
                    networkView.RPC("SendMsgToClient", player, Network.player, retornoComandoUsarItem.Comando);
                }
                else if (retorno.TiposComando == ComandsType.LargarItem)
                {
                    var retornoComandoUsarItem = Comands.ParseItemCommand(retorno.Comando, ComandsType.LargarItem, rooms[i], jogadorAtual);
                    networkView.RPC("SendMsgToClient", player, Network.player, retornoComandoUsarItem.Comando);
				}
                else if (retorno.TiposComando == ComandsType.Falar)
                {
					networkView.RPC("SendMsgToClient", RPCMode.All, Network.player, retorno.Comando);
				} 
				else if (retorno.TiposComando == ComandsType.Cochichar)
                {
					var playerDestino = playerList.Where(p => p.playerName == retorno.playerCommand).FirstOrDefault();
					if(playerDestino != null)
					{
						networkView.RPC("SendMsgToClient", playerDestino.networkPlayer, Network.player, retorno.Comando);
					}else{
						networkView.RPC("SendMsgToClient", player, Network.player, "Player Não existe");
					}
				}
				else if (retorno.TiposComando == ComandsType.Inventorio)
				{
					foreach(var item in jogadorAtual.Inventory)
					{
						networkView.RPC("SendMsgToClient", player, player, item.m_NameItem + "\n");
					}
				}
				else if (retorno.TiposComando == ComandsType.Ajuda)
				{
					var listadeComandos = new StringBuilder();
					listadeComandos.AppendLine("Examinar [sala/objeto]");
					listadeComandos.AppendLine("Mover [N/S/L/O]");
					listadeComandos.AppendLine("Pegar [objeto]");
					listadeComandos.AppendLine("Largar [objeto]");
					listadeComandos.AppendLine("Inventório");
					listadeComandos.AppendLine("Usar [objeto]");
					listadeComandos.AppendLine("Falar [texto]");
					listadeComandos.AppendLine("Cochichar [texto] [jogador]");
					listadeComandos.AppendLine("Ajuda");
					
					networkView.RPC("SendMsgToClient", player, player, listadeComandos.ToString());
				}
				else if(retorno.TiposComando == ComandsType.mapa)
				{
					var retornomapa = Map.ReadMap(jogadorAtual);
					networkView.RPC("SendMsgToClient", player, player, retornomapa.mapString);
				}
                else
                {
                    networkView.RPC("SendMsgToClient", player, player, "Comando inválido!");
                }

            }
        }        
    }

    [RPC]
    public void SendMsgToClient(NetworkPlayer player, string msg) //dummy
    {
    }

    [RPC]
    //Sent by newly connected clients, recieved by server
    public void TellServerOurName(string name, NetworkMessageInfo info)
    {
        

        PlayerNode newEntry = new PlayerNode();
        newEntry.playerName = name;
        newEntry.networkPlayer = info.sender;
        playerList.Add(newEntry);
       
    }

    public void CreateRooms()
    {
		Room room1 = new Room();
		room1.m_NameRoom = "Entrada da caverna.";
        room1.m_DescRoom = "Além da lupa, na entrada da caverna encontram espalhados galhos secos e pedaços de fitas.";
        room1.m_DirectionsRoom = new Dictionary<Directions, Room>();
        room1.m_PlayerSalaRoom = new List<PlayerNode>();
        room1.m_ItemsRoom = new List<Item>();
		room1.m_IsOpen = true;
		
		Item galhos = new Item();
        galhos.m_NameItem = "galhos";
        galhos.m_DescItem = "Galhos secos excelente para queimar!";
        galhos.m_GetItem = false;
        galhos.m_Collectable = true;
        galhos.m_IsUsedItem = false;
        room1.m_ItemsRoom.Add(galhos);

        Item lupa = new Item();
        lupa.m_NameItem = "lupa";
        lupa.m_DescItem = "A lupa do seu avó excelente para queimar alguma coisa!";
        lupa.m_GetItem = false;
        lupa.m_Collectable = true;
        lupa.m_IsUsedItem = false;
        room1.m_ItemsRoom.Add(lupa);
        
		Item cordasVelhas = new Item();
        cordasVelhas.m_NameItem = "fitas";
        cordasVelhas.m_DescItem = "Fitas velhas mas em bom estado para amarrar galhos!";
        cordasVelhas.m_GetItem = false;
        cordasVelhas.m_Collectable = true;
        cordasVelhas.m_IsUsedItem = false;
        room1.m_ItemsRoom.Add(cordasVelhas);
		
        room2 = new Room();
        room2.m_IdRoom = 2;
        room2.m_NameRoom = "Interior da caverna.";
        room2.m_DescRoom = "Ao adentrar a caverna encontra-se um abismo e espalhado pelo ambiente estão pedaços de madeira, cordas, um machado afiado e por incrível que pareça uma cola.";
        room2.m_DirectionsRoom = new Dictionary<Directions, Room>();
        room2.m_PlayerSalaRoom = new List<PlayerNode>();
		room2.m_IsOpen = false;
		room2.m_ItemsRoom = new List<Item>();
		
		Item Madeiras = new Item();
        Madeiras.m_NameItem = "madeira";
        Madeiras.m_DescItem = "Madeiras de boa sustentação.";
        Madeiras.m_GetItem = false;
        Madeiras.m_Collectable = true;
        Madeiras.m_IsUsedItem = false;
        room2.m_ItemsRoom.Add(Madeiras);

        Item cordas = new Item();
        cordas.m_NameItem = "corda";
        cordas.m_DescItem = "Cordas em bom estado.";
        cordas.m_GetItem = false;
        cordas.m_Collectable = true;
        cordas.m_IsUsedItem = false;
        room2.m_ItemsRoom.Add(cordas);
        
		Item machado = new Item();
        machado.m_NameItem = "machado";
        machado.m_DescItem = "Machado ainda afiado e forte suficiente para contar madeira.";
        machado.m_GetItem = false;
        machado.m_Collectable = true;
        machado.m_IsUsedItem = false;
        room2.m_ItemsRoom.Add(machado);

		Item cola = new Item();
        cola.m_NameItem = "cola";
        cola.m_DescItem = "Cola de forte fixação.";
        cola.m_GetItem = false;
        cola.m_Collectable = true;
        cola.m_IsUsedItem = false;
        room2.m_ItemsRoom.Add(cola);
		
		
        room3 = new Room();
        room3.m_IdRoom = 3;
        room3.m_NameRoom = "Beco fechado.";
        room3.m_DescRoom = "Depois de criar uma ponte e ultrapassar o abismo, as crianças adetram um corredor e encontram ao que parece um beco sem saída. Porém com a ajuda da tocha ";
		room3.m_DescRoom += "percebem a existencia de uma porta que requer uma chave e ao norte uma parede e entre ela de um lado uma manivela do outro uma alavanca.";
        room3.m_DirectionsRoom = new Dictionary<Directions, Room>();
        room3.m_PlayerSalaRoom = new List<PlayerNode>();
		room3.m_IsOpen = false;
		room3.m_ItemsRoom = new List<Item>();
		
		Item manivela = new Item();
        manivela.m_NameItem = "manivela";
        manivela.m_DescItem = "Manivela antiga mas em bom estado.";
        manivela.m_GetItem = false;
        manivela.m_Collectable = true;
        manivela.m_IsUsedItem = false;
        room3.m_ItemsRoom.Add(manivela);
		
		Item alavanca = new Item();
        alavanca.m_NameItem = "alavanca";
        alavanca.m_DescItem = "Alavanca em bom estado";
        alavanca.m_GetItem = false;
        alavanca.m_Collectable = true;
        alavanca.m_IsUsedItem = false;
        room3.m_ItemsRoom.Add(alavanca);
		
		
		
		room4 = new Room();
        room4.m_IdRoom = 4;
        room4.m_NameRoom = "Salão das pedras.";
        room4.m_DescRoom = "Ao usarem a manivela e a alavanca se abre uma passagem entre elas e um corredor escuro e estreito aparece. No final dele um grande salão, em cada canto um ";
        room4.m_DescRoom += "um botão no chão em formatos geométricos diferentes e quatro pedras com os respectivos formatos quadrado, triangular, retangular e circular.";
		room4.m_DirectionsRoom = new Dictionary<Directions, Room>();
        room4.m_PlayerSalaRoom = new List<PlayerNode>();
		room4.m_IsOpen = false;
		room4.m_ItemsRoom = new List<Item>();
		
		Item quadrado = new Item();
        quadrado.m_NameItem = "quadrado";
        quadrado.m_DescItem = "Pedra quadrado.";
        quadrado.m_GetItem = false;
        quadrado.m_Collectable = true;
        quadrado.m_IsUsedItem = false;
        room4.m_ItemsRoom.Add(quadrado);
		
		Item triangulo = new Item();
        triangulo.m_NameItem = "triangulo";
        triangulo.m_DescItem = "Pedra triangulo.";
        triangulo.m_GetItem = false;
        triangulo.m_Collectable = true;
        triangulo.m_IsUsedItem = false;
        room4.m_ItemsRoom.Add(triangulo);
		
		Item retangular = new Item();
        retangular.m_NameItem = "retangulo";
        retangular.m_DescItem = "pedras retangulo.";
        retangular.m_GetItem = false;
        retangular.m_Collectable = true;
        retangular.m_IsUsedItem = false;
        room4.m_ItemsRoom.Add(retangular);
		
		Item circulo = new Item();
        circulo.m_NameItem = "circulo";
        circulo.m_DescItem = "Pedra circulo.";
        circulo.m_GetItem = false;
        circulo.m_Collectable = true;
        circulo.m_IsUsedItem = false;
        room4.m_ItemsRoom.Add(circulo);
		
		room5 = new Room();
        room5.m_IdRoom = 5;
        room5.m_NameRoom = "Sala onde encontra o seu avô";
        room5.m_DescRoom = "Ao adentrar na sala só é possivel ver a escuridão, de repente luzes se acendem e você enxerga seu avô sentando e diz que vocês passaram no desafio da caverna.";
        room5.m_DirectionsRoom = new Dictionary<Directions, Room>();
        room5.m_PlayerSalaRoom = new List<PlayerNode>();
		room5.m_IsOpen = false;
		room5.m_ItemsRoom = new List<Item>();
		
        room1.m_DirectionsRoom.Add(Directions.Norte, room2);
        room2.m_DirectionsRoom.Add(Directions.Sul, room1);
        room2.m_DirectionsRoom.Add(Directions.Leste, room3);
        room3.m_DirectionsRoom.Add(Directions.Norte, room4);
        room3.m_DirectionsRoom.Add(Directions.Oeste, room2);
        room3.m_DirectionsRoom.Add(Directions.Leste, room5);
        room4.m_DirectionsRoom.Add(Directions.Sul, room3);
        room5.m_DirectionsRoom.Add(Directions.Oeste, room3);
        
        rooms[0] = room1;
        rooms[1] = room2;
        rooms[2] = room3;
        rooms[3] = room4;
        rooms[4] = room5;

        isRoomsCreate = true;
    }


    public string IniciarMud()
    {
        string mensagemSaudacao = "Bem vindo ao MUD(Multi User Dungeon).\n";
        mensagemSaudacao += "Tudo acontece quando um avô e seus netos vão acampar, eles passam a noite contando histórias.\n";
        mensagemSaudacao += "Ao amanhacer as crianças percebem que seu avó desapareceu, saem à sua procura e encontram a lupa de seu avô em frente a uma caverna muito escura.\n";
		mensagemSaudacao += "Na entrada da caverna encontram espalhados galhos secos e pedaços de fitas.";
		return mensagemSaudacao;
    }

}
