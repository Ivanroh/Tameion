using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
	Clase para actualizar los textos tanto del nombre como del nivel
*/

public class BDPlayer : MonoBehaviour
{
	/* Variables */
	ConexionBD con;
	public Text namePlayer;
	public Text nivelPlayer;
	string nivel;
	
	/* 
		Método que se ejecuta cuando se carga el objeto 
	*/
	void Start(){		
		// Objeto con la conexión a la base de datos
		con = GameObject.FindGameObjectWithTag("ManagerMultijugador").GetComponent<ConexionBD>();
		// Cambio el nombre del jugador por el que obtengo del método NombreUsuario(); (ConexionBD script)
		namePlayer.text = con.NombreUsuario();
		string nombreEscena = SceneManager.GetActiveScene().name; // Nombre de la escena actual
		
		// Si a cambiado a la escena "Tierra" (escena final) no sobreescribo el nivel del jugador
		if(nombreEscena != "Tierra"){
			// Objeto donde se encuentra el texto del nivel del usuario
			nivelPlayer = GameObject.FindGameObjectWithTag("Nivel").GetComponent<UnityEngine.UI.Text>();			
			nivel = con.NivelUsuario(); // Nivel de usuario
			nivelPlayer.text = "lvl " + nivel; // Cambio del texto de nivel 
		}
	}	  
}
