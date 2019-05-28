using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
	Clase que controla los cambios entre las distintas salas
*/

public class CambioSala : MonoBehaviour
{	
	/* Variables */
	public GameObject sala1;
	public GameObject sala2;
	System.Random random;
	GameObject player;
	Vector3 nuevaPosicion;
	private static int salasJugadas = 1;
	
	public GameObject objetoSala;
	static List<GameObject> listaObjetos = new List<GameObject>();
	
	// Inicialización de variables
	void Start(){		
		random = new System.Random();
		player = GameObject.FindGameObjectWithTag("Player");
		// Elimina objetos en caso que los haya
		EliminarObjectosExistentes();
		// Crea los nuevos objetos de la sala
		CrearObjetosSala();
	}
	
	void Update(){
		// Busca el jugador hasta que lo encuentre
		if (player == null){
			player = GameObject.FindGameObjectWithTag("Player");
		}
	}
	
	bool continua = true;	
	/*
		Método sobreescrito		
	*/
	void OnTriggerEnter2D(Collider2D col)
    {		
		// Comprobación de la etiqueta de la colisión
		if(col.tag == "Player"){			
			EliminarObjectosExistentes();			
			
			/*if(salasJugadas > 3)
				salasJugadas = random.Next(3,6);			
			if(salasJugadas == 1){*/
			// Salas mínimas por las que tendra que pasar 3 
			if(salasJugadas > 3)
				continua = Continuacion();
			// Carga la sala final
			if(!continua){
				SceneManager.LoadScene("Tierra");
				PhotonNetwork.LeaveRoom();
				Debug.Log("Sala dejada desde cambio sala...");
				PhotonNetwork.Disconnect();
				Debug.Log("Desconectado desde cambio sala...");
			}else{// Crea una nueva sala
				NuevaSala();
				CrearObjetosSala();
				salasJugadas++;
			}
		}		
    }
	
	/*
		Devuelve un boleano aleatoriamente 
	*/
	bool Continuacion(){
		int n = random.Next(1,3);
		if(n == 2)
			return true;
		else
			return false;		
	}
	
	/*
		Cambia a una nueva sala
	*/
	void NuevaSala(){		
		int num = random.Next(0,2);				
		GameObject nuevaSala;
		// Elige aleatoriamente la sala a la que se cambiara
		if (num == 0)
			nuevaSala = sala1;
		else
			nuevaSala = sala2;			
		
		Debug.Log("Salas Jugadas " + salasJugadas);
		// Posición en la nueva sala	
		NuevaPosicion(nuevaSala);
		player.transform.position = nuevaPosicion;
		// Desactivo la sala actual y activo la nueva
		gameObject.SetActive(false);
		nuevaSala.SetActive(true);							
	}
	
	/*
		Método que recibe objetos ya creados y los agrega a la lista de objetos creados
	*/ 
	public void RecibirObjetos(List<GameObject> objetos){
		listaObjetos.AddRange(objetos);
	}
	
	/*
		Elimina todos los objetos que se crearon (Nueva sala limpia para volver a crear nuevos objetos)
	*/
	void EliminarObjectosExistentes(){
		if(listaObjetos.Count != 0){
			foreach(GameObject obj in listaObjetos){
				// Elimino los objetos
				Destroy(obj);
			}
			// Reinicio la lista para que este vacía 
			listaObjetos = new List<GameObject>();
		}
	}
	
	/* 
		Instancia los objetos es una posición aleatoria
	*/
	void CrearObjetosSala(){
		if(objetoSala != null){		
			// Posiciones de los objetos
			Vector3 pos = new Vector3(random.Next(80, 740), random.Next(100, 400), 0f);
			Vector3 pos2 = new Vector3(random.Next(80, 740), random.Next(100, 400), 0f);
			Vector3 pos3 = new Vector3(random.Next(80, 740), random.Next(100, 400), 0f);
			
			// Añade los obejtos creados a la lista 
			listaObjetos.Add((GameObject)Instantiate(objetoSala, pos, Quaternion.identity));
			listaObjetos.Add((GameObject)Instantiate(objetoSala, pos2, Quaternion.identity));
			listaObjetos.Add((GameObject)Instantiate(objetoSala, pos3, Quaternion.identity));
		}
	}
	
	
	/*
		Cambia la posición donde aparecera el personaje dependiendo de el mapa donde vaya a aparecer
	*/
	void NuevaPosicion(GameObject sala){
		
		if(sala.name == "Mapa Grass"){
				nuevaPosicion = new Vector3(80f, 360f, 0f);			
		}else if(sala.name == "Snow"){
				nuevaPosicion = new Vector3(736f, 376f, 0f);			
		}else
			nuevaPosicion = new Vector3(70f, 260f, 0f);			
				
	}
}
