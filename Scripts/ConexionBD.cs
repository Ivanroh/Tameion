using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Clase para gestionar la conexión con la base de datos
*/

public class ConexionBD : MonoBehaviour
{
	/* Variables */
	// Direcciones donde se encuentran los archivos php 
	string URLCONSULTA = "https://ivanrodrigo24.000webhostapp.com/consulta.php";
	string URLINSERT = "https://ivanrodrigo24.000webhostapp.com/insert.php";
	// Datos del usuario
	public string[] usersData;
	// Entrada de datos
	public InputField user;
	public InputField pass;
	public InputField mail;
	// Mensaje de error
	public Text error;
	// Para cambiar la vista entre login e inicio sesión
	public GameObject nuevaVista;
	
	// Historia
	public GameObject historia;
	public GameObject principal;
	
	// Nombre y nivel del jugador
	static string namePlayer = "Player";
	static string nivel = "1";
	
	// Ranking
	public Transform contenedorRanking;
	public GameObject itemRanking;
	
	
	void Awake(){
		// Mensaje de error que implementa solo la escena de 'Menu' (Login, Registro) si no lo tiene lo ignora
		if(error != null){		
			error.enabled = false;
		}
		
	}
	
	/* Obtiene todos los valores de la base de datos */
	IEnumerator Start(){
		WWW users = new WWW(URLCONSULTA);
		yield return users;
		string usersDataString = users.text;
		// Divide por ';' en líneas de cada usuario
		usersData = usersDataString.Split(';');		
		// Si el objeto que ejecuta el script es el ranking lo muestra
		if(gameObject.name == "Ranking"){			
			MostrarRanking();
		}
	}
	
	/* Valida si el usuario y la contraseña coinciden, y si es así carga la escena del juego */
	public void ComprobarUsuario(){		
		bool autentificado = false;		
		
		for(int i = 0; i < usersData.Length-1 && !autentificado; i++){
			/* 
				Datos de cada uno de los usuarios
				datos[0] = nombre, datos[1] password, datos[2] nivel
			*/
			string[] datos = usersData[i].Split('|');		
			// Convertidos a mayúsculas el nombre, para que sea insensible a mayúsculas el nombre del usuario			
			if (user.text.ToUpper() == datos[0].ToUpper() && pass.text == datos[1])
			{
				autentificado = true;
				error.enabled = false;				
				namePlayer = user.text;
				nivel = datos[2];	
				// Si accede se le activa la pantalla de opciones del menú
				principal.SetActive(false);
				historia.SetActive(true);								
				//Debug.Log("Usuario auntentificado");				
			}
		}
		
		// Si no se autentifico habilito mensaje de error
		if(!autentificado){
			error.enabled = true;
		}
		
	}
	
	/* Devuelve el nombre del usuario */
	public string NombreUsuario(){				
		return namePlayer;
	}
	
	/* Devuelve el nivel del usuario */
	public string NivelUsuario(){				
		return nivel;
	}
	
	/* Crea el formulario y lo envia para el registro hacer el registro en la base de datos */
	public void NuevoUsuario(){
		string u = user.text;
		string e = mail.text;
		string p = pass.text;
		// Comprobar campos vacios
		if(u != "" && e != "" && p != ""){		
			// Creacion de formulario
			WWWForm form = new WWWForm();
			// Agregacion de parametros y valores
			form.AddField("usuario", u);
			form.AddField("email", e);
			form.AddField("password", p);
			// Envio del formulario a la url
			WWW www = new WWW(URLINSERT, form);		
			error.enabled = false;	
			// Cuando se registra muestra la historia
			principal.SetActive(false);
			historia.SetActive(true);
			namePlayer = user.text;
		}else
		{
			error.enabled = true;
		}
	}
	
	/* Cambia de visualización entre el login y el inicio*/
	public void CambiarLoginInicio(){
		// Desactivo la vista actual y activo la nueva
		gameObject.SetActive(false);
		nuevaVista.SetActive(true);
	}
	
	/* Muestra el ranking de los usuarios ordenados por premios y nivel desde la base de datos */
	public void MostrarRanking(){				
		for(int i = 0; i < usersData.Length-1; i++){
			/* 
				Datos de cada uno de los usuarios
				datos[0] = nombre, datos[1] password, datos[2] nivel, datos[3] premios
			*/
			string[] datos = usersData[i].Split('|');				
			GameObject obj = Instantiate(itemRanking);
			// Obtiene el script Ranking usa el método ActualizarItem("nombre","nivel","premios");
			obj.GetComponent<Ranking>().ActualizarItem(datos[0],datos[2],datos[3]);			
			// Se añade el item al padre
			obj.transform.SetParent(contenedorRanking);
			obj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);				
		}
	}
	
}
