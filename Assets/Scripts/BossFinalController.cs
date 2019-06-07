using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFinalController : MonoBehaviour
{
    
	Vector3 posicionInicial;
	public float speed = 2f;
	GameObject bullet;
	Vector3 direccionBala;
	Rigidbody2D rb2dBala;
	GameObject muzzle;
	
    void Start()
    {
        posicionInicial = transform.position;
		muzzle = PhotonNetwork.Instantiate("Muzzle", new Vector3(posicionInicial.x - 70f, posicionInicial.y + 6f, 0f), Quaternion.identity, 0);			
    }
			
	public void AtacarBala(Vector3 dirBala){		
		bullet = (GameObject)PhotonNetwork.Instantiate("Bullet", new Vector3(posicionInicial.x - 88f, posicionInicial.y + 6f, 0f), Quaternion.identity, 0);
		bullet.GetComponent<Bala>().obtenerDirObjetivo(dirBala);		
		muzzle.GetComponent<Animator>().Play("Muzzle");
	}
	
}
