using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* DEPRECATED Reutilizado (Se cambiará) */

public class CreateObjects : Photon.MonoBehaviour
{
	public GameObject texto1;
	public GameObject texto2;
	
	public void CambiarTexto(){
		if(texto1.GetActive()){
			texto1.SetActive(false);
			texto2.SetActive(true);
		}else
			SceneManager.LoadScene("Mazmorra");
	}
	
}
