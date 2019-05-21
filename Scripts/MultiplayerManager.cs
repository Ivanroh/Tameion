using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
	public string version = "0.1";		 	
	CreateObjects sala;    				
	
    void Start()
    {		
		PhotonNetwork.ConnectUsingSettings(version);
		DontDestroyOnLoad(gameObject);
		//sala = GameObject.FindGameObjectWithTag("Salas").GetComponent<CreateObjects>();
    }
	
	private void OnConnectedToMaster(){
		Debug.Log("Conectado master... ");	
		string nombreEscena = SceneManager.GetActiveScene().name;
		if(nombreEscena == "Tierra"){	
			PhotonNetwork.JoinRandomRoom();					
			Debug.Log("Joined random 2 Jug...");	
		}else{
			PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 1},null);	
			Debug.Log("Creada la sala un jugador");
			//PhotonNetwork.JoinRandomRoom();
			//Debug.Log("Joined ramndom...");	
		}
	}
	
	public void OnPhotonRandomJoinFailed()
	{						
		//PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 2},null);	
		//Debug.Log("Creada la sala en el fallo 2 jugadores");
		Debug.Log("Fallo en el joined");
		PhotonNetwork.CreateRoom(null, new RoomOptions(){maxPlayers = 2},null);
		Debug.Log("Creada la sala dos jugadores");			
		
	}

	private void OnDisconnectedFromPhoton(){
		
		Debug.Log("Desconcectado");		
		PhotonNetwork.Reconnect();
		/*PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();*/
	}
	
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
