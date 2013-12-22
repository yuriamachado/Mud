using UnityEngine;
using System.Collections;

/// <summary>
/// Tipos de comando que existem.
/// </summary>
public enum ComandsType {
    //Caminhar em direção a uma sala!
    Direcao = 0,
    //Usar item.
    UsarItem = 1,
	//Pegar o item
    PegarItem = 2,
    //Largar o item
    LargarItem = 3,   
    //Descricao da sala
    DescricaoSala = 4,
    //Descricao do item
    DescricaoItem = 5,
    //Ajuda com os comandos,
    Ajuda = 6,
	//Fala para todos os jogadores.
    Falar = 7,
	//Fala para um jogador especifico.
    Cochichar = 8,
	//Inventorio do jogador.
    Inventorio = 9,
    //Quando o comando é inválido.
    Invalido = 10,
	mapa = 11,
}
