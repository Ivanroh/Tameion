using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
	Clase donde se gestiona toda la parte de conexión multijugador
*/

public class MultiplayerManager : MonoBehaviour
{
	// Versión del multijugador (Para hacer el match las apliaciones deben de coincidir con la versión)
	public string version = "0.1";		 	
		
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
			// Intenta unirse a una partida, si no encuentra va al metodo OnPhotonRandomJoinFailed
			PhotonNetwork.JoinRandomRoom();					
			Debug.Log("Joined random 2 Jug...");	
		}else{
			// Si es la escena mazmorra crea una sala para el jugador (modo un jugador)
			PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 1},null);	
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
		
	}

	/* Método sobreescrito, si se desconecta intenta reconectar */
	private void OnDisconnectedFromPhoton(){
		Debug.Log("Desconcectado");		
		PhotonNetwork.Reconnect();
	}
	
	/* Método sobreescrito, cuando se une a una partida crea el jugador */
	private void OnJoinedRoom(){
			
		/*System.Random random = new System.Random();
		int posicionX = random.Next(80, 740);
		int posicionY = random.Next(100, 400);			
		*/
		Vector3 trans;
		trans = new Vector3(600, 250, 0f);
		
		PhotonNetwork.Instantiate("Jugador", trans, Quaternion.identity, 0);
		Debug.Log("OnJoinedRoom...");	
		
	}
	
}
