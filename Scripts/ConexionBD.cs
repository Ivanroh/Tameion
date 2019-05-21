using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	// Nombre y nivel del jugador
	static string namePlayer = "Player";
	static string nivel = "1";
	
	void Awake(){
		// Mensaje de error que implementa solo la escena de 'Menu' (Login, Registro) si no lo tiene lo ignora
		if(error != null){		
			error.enabled = false;
		}
	}
	
	/*
		Obtiene todos los valores de la base de datos
	*/
	IEnumerator Start(){
		WWW users = new WWW(URLCONSULTA);
		yield return users;
		string usersDataString = users.text;
		// Divide por ';' en líneas de cada usuario
		usersData = usersDataString.Split(';');
	}
	
	/*
		Valida si el usuario y la contraseña coinciden, y si es así carga la escena del juego
	*/
	public void ComprobarUsuario(){		
		bool autentificado = false;
		for(int i = 0; i < usersData.Length-1; i++){
			/* 
				Datos de cada uno de los usuarios
				datos[0] = nombre, datos[1] password, datos[2] nivel
			*/
			string[] datos = usersData[i].Split('|');				
			if (user.text == datos[0] && pass.text == datos[1])
			{
				autentificado = true;
				error.enabled = false;				
				namePlayer = user.text;
				nivel = datos[2];
				SceneManager.LoadScene("Mazmorra");								
				//Debug.Log("Usuario auntentificado");
			}
		}
		
		// Si no se autentifico habilito mensaje de error
		if(!autentificado){
			error.enabled = true;
		}
		
	}
	/*
		Devuelve el nombre del usuario
	*/
	public string NombreUsuario(){				
		return namePlayer;
	}
	
	/* 
		Devuelve el nivel del usuario
	*/
	public string NivelUsuario(){				
		return nivel;
	}
	
	/*
		Crea el formulario y lo envia para el registro hacer el registro en la base de datos
	*/
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
		}else
		{
			error.enabled = true;
		}
	}
	
	
}
