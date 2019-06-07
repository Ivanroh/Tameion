using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Clase para destruir objetos
*/

public class DestroyObjects : Photon.PunBehaviour
{
	/* Variables */
	public GameObject mana = null;
	Animator anim;
	//lista de manas creados
	static List<GameObject> listaMana = new List<GameObject>();
	// Objeto cambio sala
	CambioSala nuevaSala;
	public AudioClip pickManaAudio;	
	private ControlPlayer jugador;
	
	void Start(){
		anim = GetComponent<Animator>();
		nuevaSala = GameObject.FindWithTag("Sala").GetComponent<CambioSala>();	
		if(mana == null)// igual a null objeto que ejecuta es mana				
			jugador = GameObject.FindWithTag("Player").GetComponent<ControlPlayer>();				
	}
	
	
	
	/* Método sobreescrito de colisiones */
    void OnTriggerEnter2D(Collider2D col)
    {
		if(!gameObject.name.Contains("Boss")){ // Si no es el jefe final
			if(mana == null){ // igual a null es porque el objeto que ejecuta el script es un mana				
				if(col.tag == "Player"){	
					//Debug.Log("Entra tengo mana y soy el player que colisiona");				
					jugador.ActivarAudio(pickManaAudio);								
					PhotonNetwork.Destroy(gameObject);				
				}			
			}else{
				if(col.tag == "Ataque"){
					
					PhotonNetwork.Destroy(gameObject);
					//Si rompe el obejto obtiene mana
					GenerarMana(7);											
				}
			}
		}
			
    }
	
	public void GenerarMana(int numMana){
		System.Random random = new System.Random();
		for(int i = 0; i < numMana; i++){					
			Vector3 trans;					
			if(gameObject.name.Contains("Boss")) // Si es el jefe final
				trans = new Vector3(transform.position.x + random.Next(-320,-200) , transform.position.y + random.Next(-80,80), 0f);						
			else
				trans = new Vector3(transform.position.x + random.Next(-80,80) , transform.position.y + random.Next(-80,80), 0f);						
			
			listaMana.Add((GameObject)PhotonNetwork.Instantiate("Mana",trans , Quaternion.identity, 0)); // Instancia el mana a una lista
		}
		/* Envia la lista al Objeto CambioSala que lo añade a su lista de objetos*/
		nuevaSala.RecibirObjetos(listaMana);
	}
		
}