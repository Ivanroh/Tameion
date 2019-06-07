using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
	
	Vector3 direccionBala;
	Vector3 dirMira;
	Rigidbody2D rb2d;
	public float speed = 2f;
	float distance;
	void Start(){
		rb2d = GetComponent<Rigidbody2D>();
	}
	
    
    void FixedUpdate()
    {
		
        if(direccionBala != null){
			distance = Vector3.Distance(direccionBala, transform.position);
			dirMira = (direccionBala - transform.position).normalized;
			rb2d.MovePosition(transform.position + dirMira * speed);			
		}
		
		if(distance  < 20f)
			PhotonNetwork.Destroy(gameObject);
    }
	
	public void obtenerDirObjetivo(Vector3 dir){
		direccionBala = dir;
	}
	
	/* Si recibe una colisión */ 
	void OnTriggerEnter2D(Collider2D col)
    {		
		// Si la colisión es un ataque
		if(col.tag == "Ataque" || col.tag == "Player"){
			PhotonNetwork.Destroy(gameObject);
		}		
    }	
	
}
