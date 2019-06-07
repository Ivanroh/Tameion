using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
	
	/* Experiencia que da al matar el enemigo */
	Scrollbar barraExpJugador;
	public float expObtengo = 20;	
	//public Text textoExp;
	public Animator animTextoExp;
	
	ControlPlayer controlPlayer;
	
	bool muerto = false;
	
	BossFinalController boss;
	int waitAtaqueBoss = 0;
	float timeAttack = 180;		
	public GameObject premio;
	bool obtuvoPremio = false;
	DestroyObjects generadorMana;
	/* Aumenta la barra de experiencia del jugador */
	public void expAlJugador(GameObject jug){
		
		//textoExp.text = "+"+ expObtengo + " EXP";
		
		// Obtengo el nivel actual del jugador
		int nivel = jug.GetComponent<ControlPlayer>().NivelJugador();		
		
		// Valor de la experiencia obtenida más la que tenia
		// La experiencia sube según el nivel. Fórmula -> expObtengo/(Math.Pow(2, nivel) * 50)
		float valorExpTot = (float)(barraExpJugador.size + (expObtengo/(Math.Pow(2, nivel) * 50)));
		
		/* Si consigo más experiencia de la que necesito para subir de nivel */
		while (valorExpTot >= 1f ){			
			// Subo de nivel
			jug.GetComponent<ControlPlayer>().ActulizarLvl();					
			// Resto al total 1 el equivalente a un nivel
			valorExpTot -= 1f;
		}
		
		// Actuliza la barra por el nuevo valor
		barraExpJugador.size = valorExpTot;
				
		//barraExpJugador.size += expObtengo/(Math.Pow(2, nivel) * 50);
		//textoExp.enabled = true;	
		//animTextoExp.Play("Experiencia");
		
		// Guarda en las preferencias del player la nueva experiencia
		PlayerPrefs.SetFloat("Experiencia", barraExpJugador.size);
		
		//float playTime = animTextoExp.GetCurrentAnimatorStateInfo(0).normalizedTime;
		//if(playTime > 1.3f){
			//textoExp.enabled = false;
		//}
	}
	
	void Start()
    {		
		player = GameObject.FindGameObjectWithTag("Player"); //Objetivo a seguir 
		//controlPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<ControlPlayer>();
		barraExpJugador = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();
		//Posicion inicial
		initialPosition = transform.position;
		anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
		attackCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
		attackCollider.enabled = false;				
		//textoExp.enabled = false;	
		if(gameObject.name.Contains("Boss")){
			boss = GetComponent<BossFinalController>();		
			vidaResta = 0.02f;	
			generadorMana = GetComponent<DestroyObjects>();				
		}
    }
    
    void Update()
    {
		// Si está muerto desactivo el boleano para que no se repita la animación
		if(muerto){
			anim.SetBool("Dead", false);	
			if(gameObject.name.Contains("Boss")){ // Si mató al boss final obtiene un premio
				Instantiate(premio, new Vector3(400f, 250f, 0f), Quaternion.identity);
				AudioSource sala = GameObject.FindWithTag("GameController").GetComponent<AudioSource>();	
				sala.volume = 0.05f;
				player.GetComponent<ControlPlayer>().FinPartida(5); // parametro 5 segundos espere antes del fin de la partida
				
				if(!obtuvoPremio)
					player.GetComponent<ControlPlayer>().ActulizarPremios(); // Actualizo los premios obtenidos									
				obtuvoPremio  = true;
			}
		}
		// Comprueba que el jugador no sea nulo y no este muerto
		if(player != null && !muerto){
				
			// Por defecto el enemigo se queda donde estaba
			Vector3 target = initialPosition;
		
			// Rayo para averiguar si colisiona el jugador con el enemigo en su radio de visión
			RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, radioVision, 1 << LayerMask.NameToLayer("Jugador")	);
			
			//Debug del Raycast 
			Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
			// Dibuja la línea del raycast de color rojo (No esta dentro del radio de visión)
			Debug.DrawRay(transform.position, forward, Color.red);
			
			// Si hay una colisión con el rayo 
			if(hit.collider != null){
				//Si el Raycast encuentra al jugador lo ponemos de target(objetivo)
				if(hit.collider.tag == "Player"){
					target = player.transform.position;				
				}
			}
			
			//Calculamos la distancia entre el enemigo y el objetivo
			float distance = Vector3.Distance(target, transform.position);			
			Vector3 dirMira = (target - transform.position).normalized;
						
			// Si está dentro de la distancia de ataque, ataca
			if(target != initialPosition && distance < radioAtaque){			
				//Dirección del ataque
				float dirAttack = player.transform.position.x - transform.position.x;
				anim.SetFloat("MovX", dirAttack);			
				anim.SetBool("Walk", false);
				
				if(boss != null){	
					if(barraVida.size < 0.5f && barraVida.size > 0.3f){
						timeAttack = 120;
					}
					if(barraVida.size < 0.3f){						
						timeAttack = 90;
					}
					
					if(waitAtaqueBoss == 0){
						boss.AtacarBala(player.transform.position);						
						anim.SetTrigger("Enemy_Attack");
					}
					waitAtaqueBoss++;
					
					if(waitAtaqueBoss == timeAttack)
						waitAtaqueBoss = 0;					
				}
				else
					HabilitarAtaque();
			}else{
				// Si no esta en el radio de Ataque sigue al objetivo
				rb2d.MovePosition(transform.position + dirMira * speed);							
				anim.SetFloat("MovX", dirMira.x);
				anim.SetBool("Walk", true);
			}
			
			// Si se queda sin vida
			if (barraVida.size <= 0f){
				muerto = true;				
				// Reinicio valores
				attackCollider.enabled = false;					
				anim.SetBool("Dead", true);											
				//Espera el timepo de la animación + 1 segundo para desaparecer el object enemigo
				Destroy (gameObject,anim.GetCurrentAnimatorStateInfo(0).length - 0.25f);				
				// Subo la experiencia del jugador
				expAlJugador(player);				
			}
			
			// Si regresa a su posición inicial deja de 'andar' 
			if(target == initialPosition && distance < 20f){
				transform.position = initialPosition;
				anim.SetBool("Walk",false);			
			}
			// Dibujo de una línea cuando encuentra el objetivo en verde
			Debug.DrawLine(transform.position, target, Color.green);
			
		}else{	
			// Si no encuentra el jugador lo vuelve a buscar
			player = GameObject.FindGameObjectWithTag("Player"); //Objetivo a seguir 
			barraExpJugador = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();			
		}
			
    }
	
	/* Habilita la colisión de ataque */
	void HabilitarAtaque(){
		anim.SetTrigger("Enemy_Attack");
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
	
	/* Si recibe una colisión */ 
	void OnTriggerEnter2D(Collider2D col)
    {		
		// Si la colisión es un ataque
		if(col.tag == "Ataque"){
			barraVida.size -= vidaResta;
			if(barraVida.size < 0.3f && boss != null){
				generadorMana.GenerarMana(3);
			}
			
		}		
    }	
	
	// Método sobreescrito que dibuja los radios de visión y de ataque (Debug)
	void OnDrawGizmosSelected(){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radioVision);
		Gizmos.DrawWireSphere(transform.position, radioAtaque);
	}
}
