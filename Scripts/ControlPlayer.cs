using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
	Clase para gestionar todo lo referente al personaje
*/

public class ControlPlayer : Photon.PunBehaviour
{
		
	/* Variables */
	
	// Propiedades del personaje
	public float speed = 4f;
	Animator anim;	
	Vector2 mov;
	Rigidbody2D rg2d;
	Vector3 pocisionInicial;
	
	// Controles del personaje
	Joystick joystick;
	GameObject controles;
		
	// Vida
	Scrollbar barraVida;
	private float vidaResta = 0.05f;
	
	// Ataque
	GameObject boton;
    EventTrigger trigger;
	CircleCollider2D attackCollider;
	
	// Nombre jugador
	public Text namePlayer;
	// Nivel 
	Text nivelPlayer;
	
	public Scrollbar mana;
	public Scrollbar experiencia;
	
	// Dirección donde se ejecuta el php de actualización
	string URL_UPDATE = "https://ivanrodrigo24.000webhostapp.com/update.php";
		
    void Start()
    {	
		/*Inicialización de variables */		
		DontDestroyOnLoad(gameObject);
        anim = GetComponent<Animator>();
		rg2d = GetComponent<Rigidbody2D>();
		controles = GameObject.FindWithTag("Controles");
		attackCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
		attackCollider.enabled = false;
		pocisionInicial = transform.position;
		barraVida = GameObject.FindWithTag("SaludJugador").GetComponent<Scrollbar>();		
		
		/* Controles */
		boton = GameObject.FindWithTag("BotonAtaque");		
		trigger = boton.GetComponentInParent<EventTrigger>();
		ActivarTriggger();
		joystick = controles.GetComponent<Joystick>();
		
		/* Mana - Experiencia, se actualiza conservando el estado */
		mana = GameObject.FindWithTag("Mana").GetComponent<Scrollbar>();		
		experiencia = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();		
		
		/* Actualizo los valores guardados en las preferencias del jugador */
		experiencia.size = PlayerPrefs.GetFloat("Experiencia");
		mana.size = PlayerPrefs.GetFloat("mana");				
		
		nivelPlayer = GameObject.FindGameObjectWithTag("Nivel").GetComponent<UnityEngine.UI.Text>(); // Nivel del jugador
    }
	    
    void Update()
    {		
		// Opción para salir de la aplicación
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit();
		
		if(experiencia.size == 1f){	// Comprueba si la experiencia del jugador esta al 100% (100% = 1f)
			experiencia.size = 0f; // Reinicio experiencia
			PlayerPrefs.SetFloat("Experiencia", 0f); // Guardo el valor en las preferencias del jugador 
			actulizarLvl();
		}

		// Comprobación en la red para saber si es mi jugador y no actuar en el resto
		if(photonView.isMine){
			
			mov = new Vector2(joystick.Horizontal,joystick.Vertical); 			
			// Comprobar si esta en movimiento y activar la animación de andar
			if(mov != Vector2.zero){		
				anim.SetBool("Walk", true);				
			}else{
				anim.SetBool("Walk", false);				
			}
			
			// Animación ataque 
			if(transform.position == pocisionInicial){
				anim.SetFloat("MovX",-1f);
			}
			// Animación parado(Idle) izquierda o derecha
			//Izquierda
			if(mov.x < 0 )
				anim.SetFloat("MovX",1f);		
			//Derecha
			if(mov.x > 0.1f )
				anim.SetFloat("MovX",-1f);
			
			// Activar la colision de ataque a través de la red
			photonView.RPC("HabilitarAtaque", PhotonTargets.All);			
		}
    }
	
	/* Método sobreescrito que actualiza el movimiento del jugador */
	void FixedUpdate(){
		rg2d.MovePosition(rg2d.position + (mov * speed));
	}
	
	/* Método sobreescrito trigger de colisiones */
	void OnTriggerEnter2D(Collider2D col)
    {
		// Si recibe daño de un enemigo o de otro jugador
		if(col.tag == "AtaqueEnemigo" || col.tag == "Ataque"){			
			photonView.RPC("RecibirDamage", PhotonTargets.All);						
		}
		// Comprueba si colisiona con el Mana
		if(col.tag == "Mana"){
			mana.size += 0.01f; // Aumenta la barra de mana del jugador
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador
		}
    }
	
	/* Método sobreescrito de Photon para actualizar a través de la red los atributos necesarios */
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){			
			//Sincronizar la vida y el nombre
			stream.SendNext(barraVida.size);
			stream.SendNext(namePlayer.text);						
		}else{			
			barraVida.size = (float)stream.ReceiveNext();
			namePlayer.text = (string)stream.ReceiveNext();			
		}
	}
	
	/* Método que se envia a través de la red, habilita la colision de ataque del jugador */
	[PunRPC]
	void HabilitarAtaque(){
		if(photonView.isMine){
			//Estado actual del animador
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			bool atacando = stateInfo.IsName("Attack_Jugador");			
			if(atacando){				
				//Tiempo en que se ejecuta la animacion
				float playTime = stateInfo.normalizedTime;				
				if(playTime > 0.23 && playTime < 0.76 )
					attackCollider.enabled = true;
				else
					attackCollider.enabled = false;				
			}
		}
	}
	
	/* Aciva la animación de ataque */
	public void Atacar(){
		anim.SetTrigger("Attacking");
	}
	
	/* Método para activar el trigger de ataque */
	void ActivarTriggger()
    {		
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener( (eventData) => { Atacar(); } );			
		trigger.triggers.Add(entry);		
    }
		
	/* Actualiza la vida del jugador en la red */
	[PunRPC]
	void RecibirDamage(){
		if(photonView.isMine){
			barraVida.size -= vidaResta;
			if( barraVida.size <= 0){
				anim.SetTrigger("Dead");			
				Destroy (gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
			}
		}
	}
	
	/* Actualiza el nivel del jugador */
	void actulizarLvl(){
		WWWForm form = new WWWForm(); // Creación del formulario		
		form.AddField("editUser", namePlayer.text); // Agregación parametro y valor		
		WWW www = new WWW(URL_UPDATE, form); // Envio del formulario		
		int pos = nivelPlayer.text.IndexOf(" "); // posicion donde se separan por espacio " "
		int nuevoLvl = int.Parse(nivelPlayer.text.Substring(pos + 1))+1; // suma 1 al nivel actual
		nivelPlayer.text = "Lvl "+nuevoLvl.ToString(); 
		//Debug.Log("Un nivel más");
	}
	
}