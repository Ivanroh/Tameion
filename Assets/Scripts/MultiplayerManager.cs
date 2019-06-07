using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
	Clase donde se gestiona toda la parte de conexión multijugador
*/

public class MultiplayerManager : Photon.PunBehaviour
{
	// Versión del multijugador (Para hacer el match las apliaciones deben de coincidir con la versión)
	public string version = "0.1";	
	bool falloJoin = false;
	bool unJugador = false;
	
	// Variables para comprobar tiempo de espera de segundo jugador
	static int waitJugador = 0;
	bool enEsperaJugador = false;
	static int numJugadores = 0;
	
	/* Método sonbreescrito que muestra el estado que se encuentra el lobby*/
	public override void OnLobbyStatisticsUpdate()
    {
        string countPlayersOnline;
        countPlayersOnline = PhotonNetwork.countOfPlayers.ToString() + " Jugadores en la sala jugando";
		numJugadores = PhotonNetwork.countOfPlayers;
		Debug.Log(countPlayersOnline);
    }
	
	void Update(){
		if(enEsperaJugador){		
			waitJugador++;
			if(waitJugador > 200){
				if(PhotonNetwork.room.open)
					PhotonNetwork.Instantiate("BossFinal", new Vector3(600f, 260f, 0f), Quaternion.identity, 0);
				PhotonNetwork.room.open = false;
				Debug.Log("Fin de la espera jugador, modo offline... y le mando el enemigo final");	
				enEsperaJugador = false;
				waitJugador = 0;								
			}
		}
		
		//Debug.Log("Open ?? -> "+PhotonNetwork.room.open);
		
	}
	
		
	/* Cuando se instacnia se conecta usando la versión */
    void Start()
    {		
		PhotonNetwork.ConnectUsingSettings(version);
		// Mantiene el objecto aunque cambie de escena
		DontDestroyOnLoad(gameObject);		
    }
	
	/* Método sobreescrito donde se conecta al Master */
	private void OnConnectedToMaster(){
		Debug.Log("Conectado master... ");	
		// Compruebo si la escena no es la final
		string nombreEscena = SceneManager.GetActiveScene().name;
		if(nombreEscena == "Tierra"){
			unJugador = false;
			// Intenta unirse a una partida, si no encuentra va al metodo OnPhotonRandomJoinFailed
			PhotonNetwork.JoinRandomRoom();					
			Debug.Log("Joined random 2 Jug...");
			
		}else{
			// Si es la escena mazmorra crea una sala para el jugador (modo un jugador)
			PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 1},null);
			unJugador = true;
			Debug.Log("Creada la sala un jugador");
		}
	}
	
	/* Método sobreescrito, se ejecuta si da un error al realizar el JoinRandomRoom */
	public void OnPhotonRandomJoinFailed()
	{						
		// Si dio error al unirse a una partida creo una nueva para dos jugadores
		Debug.Log("Fallo en el joined");
		PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 2},null);
		Debug.Log("Creada la sala dos jugadores");	
		falloJoin = true;
		enEsperaJugador	= true;
	}

	/* Método sobreescrito, si se desconecta intenta reconectar */
	private void OnDisconnectedFromPhoton(){
		Debug.Log("Desconcectado");		
		PhotonNetwork.Reconnect();
	}
	
	/* Método sobreescrito, cuando se une a una partida crea el jugador */
	private void OnJoinedRoom(){
			
		Vector3 trans;
		// Si es modo un judador se instancia al medio de la pantalla
		if(unJugador)
			trans =new Vector3(400f, 240f, 0f);		
		else if(falloJoin)  // Si fallo el join me posiciono a la izquierda del mapa en caso cotrario a la derecha
			trans = new Vector3(130f, 260f, 0f);
		else
			trans = new Vector3(670f, 260f, 0f);
		
		PhotonNetwork.Instantiate("Jugador", trans, Quaternion.identity, 0);
		Debug.Log("OnJoinedRoom...");	
		
	}
	
}
