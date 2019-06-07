using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 
	Clase para cambiar del objeo acrual a uno nuevo por parámetro 
*/

public class MenuSeleccion : MonoBehaviour
{
	// Objeto que se habilitará
	public GameObject seleccion;
    
	/* Realiza el cambio de vistas actual por seleccion */
    public void VerSeleccion(){
		gameObject.SetActive(false);
		seleccion.SetActive(true);
	}
	
	/* Carga la escena Mazmorra donde se empieza una partida */
	public void Jugar(){
		SceneManager.LoadScene("Mazmorra");
	}
}