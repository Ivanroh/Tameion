using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Clase con una pequeña inteligencia artificial para el control de los enemigos
*/

public class Enemy : MonoBehaviour
{
	/* Propiedades del enemigo */
	public float radioVision;
	public float radioAtaque;
	public float speed;
	
	/* Variable para guardar el jugador */
	GameObject player;
	
	/* Variable para guardar la posicion inicial que está */
	Vector3 initialPosition;
	
	/* Animador y cuerpo cinemático con la rotación z congelada */
	Animator anim;
	Rigidbody2D rb2d;
	
	/* Vida enemigo */
	public Scrollbar barraVida;
	private float vidaResta = 0.1f;
	CircleCollider2D attackCollider;
	
	/* Experiencia que da al mater el enemigo */
	Scrollbar barraExpJugador;
	public float expObtengo = 20;	
	public Text textoExp;
	public Animator animTextoExp;
	
	/* Aumenta la barra de experiencia del jugador */
	public void expAlJugador(){
		textoExp.text = "+"+ expObtengo + " EXP";
		//la experiencia sube segun el nivel expObtengo/(Math.Pow(2, nivel) * 50)
		
		/* cuando me paso*/
		int valorExpTot = barraExpJugador.size + (expObtengo/(Math.Pow(2, nivel) * 50));
		while (valorExpTot >= 1f ){
			SubirNivel();
			valorExpTot -= 1f;
		}
		barraExpJugador.size = valorExpTot;
		/* fin cuando me paso */
		
		//barraExpJugador.size += expObtengo/(Math.Pow(2, nivel) * 50);
		textoExp.enabled = true;	
		animTextoExp.Play("Experiencia");
		PlayerPrefs.SetFloat("Experiencia", barraExpJugador.size);
		/*float playTime = animTextoExp.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if(playTime > 1.3f){
			textoExp.enabled = false;
		}*/
	}
	
	void Start()
    {		
		player = GameObject.FindGameObjectWithTag("Player"); //Objetivo a seguir 
		barraExpJugador = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();
		//Posicion inicial
		initialPosition = transform.position;
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		attackCollider = transform.GetChild(1).GetComponent<CircleCollider2D>();
		attackCollider.enabled = false;				
		textoExp.enabled = false;		
    }
    
    void Update()
    {
		if(player != null){
					
			//Por defecto nos quedamos donde esytamos
			Vector3 target = initialPosition;
		
			RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, radioVision, 1 << LayerMask.NameToLayer("Default")	);
			
			//Debug del Raycast 
			Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
			Debug.DrawRay(transform.position, forward, Color.red);
			
			//Si el Raycast encuentra al jugador lo ponemos de traget(objetivo)
			if(hit.collider != null){
				if(hit.collider.tag == "Player"){
					target = player.transform.position;
				}
			}
			
			//Calculamos la distancia entre el enemigo y el objetivo
			float distance = Vector3.Distance(target, transform.position);
			Vector3 dirMira = (target - transform.position).normalized;
			//Si muere se queda en el sitio
			
			//Si es el enemigo y está 
			if(target != initialPosition && distance < radioAtaque){			
				//Dirección del ataque
				float dirAttack = player.transform.position.x - transform.position.x;
				anim.SetFloat("MovX", dirAttack);			
				anim.SetBool("Walk", false);
				anim.SetTrigger("Enemy_Attack");
				HabilitarAtaque();
			}else{
				rb2d.MovePosition(transform.position + dirMira * speed * Time.deltaTime);			
				anim.SetFloat("MovX", dirMira.x);
				anim.SetBool("Walk", true);
			}
			
			
			if (barraVida.size <= 0f){
				//anim.Play("Zombie_Dead");
				if(barraVida.size != 0.001f){
					anim.SetTrigger("Dead");
					radioAtaque = 0f;
					radioVision = 0f;					
					//Espera el timepo de la animacion mas 0.5 segundos más para desaparecer el object enemigo
					Destroy (gameObject,anim.GetCurrentAnimatorStateInfo(0).length + 1f);
					//Para que no regrese a su posición original
					initialPosition = transform.position;
					expAlJugador();
				}				
				barraVida.size = 0.001f;
				
			}
			// 
			if(target == initialPosition && distance < 20f){
				transform.position = initialPosition;
				anim.SetBool("Walk",false);			
			}
			
			Debug.DrawLine(transform.position, target, Color.green);
			
		}else{
			player = GameObject.FindGameObjectWithTag("Player"); //Objetivo a seguir 
			barraExpJugador = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();
		}
			
    }
	
	void HabilitarAtaque(){
		//Estado actual del animador
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		bool atacando = stateInfo.IsName("Enemy_Attack");			
		if(atacando){			
			//Tiempo en que se ejecuta la animacion
			float playTime = stateInfo.normalizedTime;				
			if(playTime > 0.03f && playTime < 0.46f ){
				attackCollider.enabled = true;						
			}
			else
				attackCollider.enabled = false;												
		}	
	}
	
	
	void OnTriggerEnter2D(Collider2D col)
    {
		if(barraVida.size != 0.001f){			
			if(col.tag == "Ataque"){
				barraVida.size -= vidaResta;
			}		
		}
    }	
	
	//Dibujo de los radios 'Debug'
	void OnDrawGizmosSelected(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radioVision);
		Gizmos.DrawWireSphere(transform.position, radioAtaque);
	}
}
