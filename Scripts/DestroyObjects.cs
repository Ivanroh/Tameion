using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Clase para destruir objetos
*/

public class DestroyObjects : MonoBehaviour
{
	/* Variables */
	public GameObject mana = null;
	Animator anim;
	//lista de manas creados
	static List<GameObject> listaMana = new List<GameObject>();
	// Objeto cambio sala
	CambioSala nuevaSala;
	
	void Start(){
		anim = GetComponent<Animator>();
		nuevaSala = GameObject.FindWithTag("Sala").GetComponent<CambioSala>();
	}
	/* Método sobreescrito de colisiones */
    void OnTriggerEnter2D(Collider2D col)
    {
		System.Random random = new System.Random();
		if(mana == null){ // igual a null es porque el objeto que ejecuta el script es un mana
			if(col.tag == "Player"){
				Destroy(gameObject);
			}			
		}else{
			if(col.tag == "Ataque"){
				Destroy(gameObject);
				//Si rompe el obejto obtiene mana
				for(int i = 0; i < 7; i++){
					Vector3 trans;					
					trans = new Vector3(transform.position.x + random.Next(-80,80) , transform.position.y + random.Next(-80,80), 0f);															
					listaMana.Add((GameObject)Instantiate(mana,trans , Quaternion.identity)); // Instancia el mana a una lista
				}
				/* Envia la lista al Objeto CambioSala que lo añade a su lista de objetos*/
				nuevaSala.RecibirObjetos(listaMana);
			}
		}
			
    }
		
}