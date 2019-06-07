using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Clase para actulziar la profundidad de los objetos según la posición
*/

public class Profundidad : Photon.MonoBehaviour
{
	//Variable para actualziar la profundida en cada fotográma
	public bool cambioFrame;
	SpriteRenderer spr;
    
	/*
		Método que se ejecuta cuando se carga el objeto
	*/
    void Start()
    {
        spr = GetComponent<SpriteRenderer>(); // Obtención del SpriteRenderer que tiene el objecto que depende de esta clase
		spr.sortingLayerName = "Jugador"; // Cambia el layer a Jugador
		spr.sortingOrder = Mathf.RoundToInt(transform.position.y * (-10)); // Cambia el orden y lo actualiza
    }

	/* 
		Método que se ejecuta constantemete
	*/
    void Update()
    {
		//Si esta activo el cambio por frame actulzia constantemente el sortingOrder
        if(cambioFrame){
			spr.sortingOrder = Mathf.RoundToInt(transform.position.y * (-10));			
		}
    }
	
}
