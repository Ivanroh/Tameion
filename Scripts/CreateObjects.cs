using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* DEPRECATED */

public class CreateObjects : Photon.MonoBehaviour
{
	public GameObject prefab;
	public int numberOfObjects = 5;	
		
	private int salas = 1;
	
	void Start(){
		System.Random random = new System.Random();
		for (int i = 0; i < numberOfObjects; i++) {			
			int posicionX = random.Next(80, 740);
			int posicionY = random.Next(100, 400);			
			Vector3 pos = new Vector3(posicionX, posicionY, 0f);
			Instantiate(prefab, pos, Quaternion.identity);
		}
	}
	/*
	public void CrearObjetos(){
		System.Random random = new System.Random();
		for (int i = 0; i < numberOfObjects; i++) {			
			int posicionX = random.Next(80, 740);
			int posicionY = random.Next(100, 400);			
			Vector3 pos = new Vector3(posicionX, posicionY, 0f);
			Instantiate(prefab, pos, Quaternion.identity);
		}
	}
	*/
	
	public void NuevaSala(){
		//PhotonNetwork.Disconnect();
		//SceneManager.LoadScene("Tierra");
		salas++;
		//Debug.Log("Número de salas: " + salas);
	}
	
	public int NumeroSalasHechas(){
		return salas;
	}
	
	
	public void SwitchLevel (string level)
	{		
	  StartCoroutine (DoSwitchLevel(level));
	}

	IEnumerator DoSwitchLevel (string level)
	{
	  PhotonNetwork.Disconnect();
	  while (PhotonNetwork.connected)
		yield return null;
	  SceneManager.LoadScene(level);
	}
	
	
}
