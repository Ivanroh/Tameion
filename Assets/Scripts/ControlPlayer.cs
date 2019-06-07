using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/*
	Clase para gestionar todo lo referente al personaje
*/

public class ControlPlayer : Photon.PunBehaviour
{
		
	/* Variables */
	
	// Propiedades del personaje
	public static float speed = 4f;
	Animator anim;	
	Vector2 mov;
	Rigidbody2D rg2d;
	Vector3 pocisionInicial;
	
	// Controles del personaje
	Joystick joystick;
	static GameObject controles;
	GameObject joy;
		
	// Vida
	Scrollbar barraVida;
	static float vidaResta = 0.01f;
	
	// Ataque
	GameObject boton;
    EventTrigger trigger;
	static CircleCollider2D attackCollider;
	
	// Nombre jugador
	public Text namePlayer;
	// Nivel 
	Text nivelPlayer;
	
	// Barra de mana y experiencia
	static Scrollbar mana;
	Scrollbar experiencia;
	
	// Dirección donde se ejecuta el php de actualización
	string URL_UPDATE = "https://ivanrodrigo24.000webhostapp.com/update.php";
		
	// Ataque especial	
	public GameObject ataqueEsp;	

	// Variables para habilidades especiales
	static bool skillVelocidad = false;  
	static bool skillAtaque = false;  
	static bool skillDefensa = false;  
	int waitVelocidad = 0;
	int waitAtaque = 0;
	int waitDefensa = 0;	
	
	// Lista de los objetos del ataque especial 
	static List<GameObject> listaAtaqueEsp = new List<GameObject>();
	
	// Manager Multijugador 
	static GameObject managerMultijugador;
		
	static bool atacando = false;
	// Animaciones audio
	private AudioSource audioJugador;	
	public AudioClip audioDead;
	public AudioClip audioLevelUP;
	public AudioClip audioSpecialAttack;
	public AudioClip audioPotionVelocity;
	public AudioClip audioPotionHealth;
	public AudioClip audioGettingHit;
	public AudioClip audioAttackSwordHit;
	public AudioClip audioAttackSwoshSword;
	public AudioClip audioWalking;
	
	
	bool finPartida = false;
	int tiempoFinPartida;
	
    void Start()
    {	

		/*Inicialización de variables */		
		DontDestroyOnLoad(gameObject);
        anim = GetComponent<Animator>();
		rg2d = GetComponent<Rigidbody2D>();
		joy = GameObject.FindWithTag("Joystick");
		controles = GameObject.FindWithTag("Controles");
		attackCollider = transform.GetChild(0).GetComponent<CircleCollider2D>();
		attackCollider.enabled = false;
		pocisionInicial = transform.position;
		barraVida = GameObject.FindWithTag("SaludJugador").GetComponent<Scrollbar>();		
		nivelPlayer = GameObject.FindGameObjectWithTag("Nivel").GetComponent<UnityEngine.UI.Text>(); // Nivel del jugador		
		ataqueEsp = GameObject.FindWithTag("Circulo");	
		
		/* Controles */
		boton = GameObject.FindWithTag("BotonAtaque");		
		trigger = boton.GetComponentInParent<EventTrigger>();
		ActivarTriggger();
		joystick = joy.GetComponent<Joystick>();
		
		/* Mana - Experiencia, se actualiza conservando el estado */
		mana = GameObject.FindWithTag("Mana").GetComponent<Scrollbar>();		
		experiencia = GameObject.FindWithTag("Experiencia").GetComponent<Scrollbar>();		
		
		/* Actualizo los valores guardados en las preferencias del jugador */
		experiencia.size = PlayerPrefs.GetFloat("Experiencia");
		mana.size = PlayerPrefs.GetFloat("mana");	

		// controlador multiplayer
		managerMultijugador = GameObject.FindWithTag("ManagerMultijugador");	
		// Control del audio
		audioJugador = GetComponent<AudioSource>();
    }

	/* Velocidad. Espera 100 frames antes de volver a los valores originales */
	void EsperaVelocidad(){
		waitVelocidad++;
		if(waitVelocidad == 1)
			ActivarAudio(audioPotionVelocity);
		if(waitVelocidad > 100){					
			speed /= 2;						
			skillVelocidad = false;
			waitVelocidad = 0;				
		}
	}
	
	/* Ataque. Espera 100 frames antes de volver a los valores originales */	
	void EsperarAtaque(){
		waitAtaque++;
		if(waitAtaque == 1)
			ActivarAudio(audioSpecialAttack);	
		if(waitAtaque > 100){			
			EliminarAtaqueEspecial();				
			skillAtaque = false;
			waitAtaque = 0;				
		}
	}
	
	/* Defensa(vida). Espera 30 frames antes de volver a los valores originales */	
	void EsperarDefensa(){
		waitDefensa++;
		if(waitDefensa == 1)
			ActivarAudio(audioPotionHealth);
		if(waitDefensa > 30){								
			skillDefensa = false;
			waitDefensa = 0;				
		}
	}
	
	public void ActivarAudio(AudioClip clip){
		audioJugador.Stop();
		audioJugador.clip = clip;
		audioJugador.Play();
	}
	private static int waitWalk = 0;
	void WaitAudioWalk(){		
		if(waitWalk > 200){											
			waitWalk = 0;				
		}
	}
	private static int waitAttackHit  = 0;
	bool walkingAu = false; 
	
	
	private static int waitFinPartida = 0;
	void WaitFinPartida(){		
		if(waitFinPartida >= tiempoFinPartida){											
			ReinicioJuego();
		}
		waitFinPartida++;
	}
	
	
    void Update()
    {		
		if(finPartida){
			WaitFinPartida();
		}
		
		// Comprobación en la red para saber si es mi jugador y no actuar en el resto
		if(photonView.isMine){
			
			if(atacando)
				waitAttackHit++;
			else
				waitAttackHit = 0;
			
			mov = new Vector2(joystick.Horizontal,joystick.Vertical); 			
			// Comprobar si esta en movimiento y activar la animación de andar
			if(mov != Vector2.zero){
				waitWalk++;
				if(waitWalk == 1){
					ActivarAudio(audioWalking);
					walkingAu = true;
				}
				WaitAudioWalk();
				anim.SetBool("Walk", true);
				
			}else{
				anim.SetBool("Walk", false);				
				if(walkingAu){
					audioJugador.Stop();
					walkingAu = false;
				}
				waitWalk = 0;
			}
								
			// Animación ataque 
			if(transform.position == pocisionInicial){
				anim.SetFloat("MovX",-1f);
			}
			// Animación parado(Idle) izquierda o derecha
			//Izquierda
			if(mov.x < 0 )
				anim.SetFloat("MovX",-1f);		
			//Derecha
			if(mov.x > 0.1f )
				anim.SetFloat("MovX",1f);
						 		
			// Activar la colision de ataque a través de la red
			photonView.RPC("HabilitarAtaque", PhotonTargets.All);
			
			/* Skills */
			// Si está activo el ataque
			if(skillAtaque){				
				EsperarAtaque();				
			}
			// Si está activa la velocidad
			if(skillVelocidad){
				EsperaVelocidad();
			}
			// Si está activo la defensa
			if(skillDefensa){				
				this.barraVida.size += 0.01f;
				EsperarDefensa();
				//Debug.Log(" vida activada ");
			}
			// Opción para salir de la aplicación
			if (Input.GetKeyDown(KeyCode.Escape)) 
				Application.Quit();
			
			if(experiencia.size == 1f){	// Comprueba si la experiencia del jugador esta al 100% (100% = 1f)
				experiencia.size = 0f; // Reinicio experiencia
				PlayerPrefs.SetFloat("Experiencia", 0f); // Guardo el valor en las preferencias del jugador 
				ActulizarLvl();
			}	
		}
    }
	
	/* Método sobreescrito que actualiza el movimiento del jugador */
	void FixedUpdate(){
		rg2d.MovePosition(rg2d.position + (mov * speed));
	}
	
	/* ontrigger lo ejecuta a todos los que colisionan */
	
	/* Método sobreescrito trigger de colisiones *
	void OnTriggerEnter2D(Collider2D other)
    {		
		if (!photonView.isMine)
        {
            return;
        }        
		
		//PhotonView photonView = this.photonView;
		// Si recibe daño de un enemigo o de otro jugador
		if(other.tag == "AtaqueEnemigo" || other.tag == "Ataque"){
			this.photonView.RPC("RecibirDamage", PhotonTargets.All);			
			//RecibirDamage();
		}
		// Comprueba si colisiona con el Mana
		if(other.tag == "Mana"){			
			mana.size += 0.01f; // Aumenta la barra de mana del jugador
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador
		}		
	
    }*/
	
	void OnTriggerEnter2D(Collider2D other)
	{		
		//Debug.Log("Entra una colision");
		
		if(waitAttackHit == 1){
			ActivarAudio(audioAttackSwordHit);
		}
		
		if(other.tag == "AtaqueEnemigo" || other.tag == "Ataque"){								
			this.photonView.RPC("RecibirDamage", PhotonTargets.All);			
		}
		if(other.tag == "AtaqueEnemigo"){
			vidaResta = 0.01f;			
			this.photonView.RPC("RecibirDamage", PhotonTargets.All);
		}
		
		if(other.tag == "AtaqueBoss"){
			vidaResta = 0.1f;
			this.photonView.RPC("RecibirDamage", PhotonTargets.All);
		}
		
		
		// Comprueba si colisiona con el Mana
		if(other.tag == "Mana"){			
			mana.size += 0.01f; // Aumenta la barra de mana del jugador
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador
		}					

	}
	
	
	/* Método sobreescrito de Photon para actualizar a través de la red los atributos necesarios */
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){		
		
		if(stream.isWriting){			
			//Sincronizar la vida y el nombre
			//Debug.Log("Lo que envio al resto -> " + barraVida.size + " | " + namePlayer.text);
			stream.SendNext(this.barraVida.size);
			stream.SendNext(namePlayer.text);						
		}else{			
			this.barraVida.size = (float)stream.ReceiveNext();
			namePlayer.text = (string)stream.ReceiveNext();	
			//Debug.Log("Recibo -> " + barraVida.size + " | " + namePlayer.text);						
		}
	}
	
	/* Método que se envia a través de la red, habilita la colision de ataque del jugador */
	[PunRPC]
	void HabilitarAtaque(){									
		//Estado actual del animador
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		atacando = stateInfo.IsName("Attack_Jugador");			
		if(atacando){						
			//Tiempo en que se ejecuta la animacion
			float playTime = stateInfo.normalizedTime;				
			if(playTime > 0.01 && playTime < 0.29 )
				attackCollider.enabled = true;									
			else{
				attackCollider.enabled = false;									
			}									
		}
		
	}
		
	/* Aciva la animación de ataque */
	public void AtacarAnim(){
		if(photonView.isMine){			
			//Estado actual del animador
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			atacando = stateInfo.IsName("Attack_Jugador");			
			if(!atacando){ // Si no está atacando puedo atacar
				anim.SetTrigger("Attacking");
				ActivarAudio(audioAttackSwoshSword);
			}				
		}		
	}
	
	/* Método para activar el trigger de ataque */
	void ActivarTriggger()
    {		
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener( (eventData) => { AtacarAnim(); } );			
		trigger.triggers.Add(entry);		
    }
		
	/* Actualiza la vida del jugador en la red */
	[PunRPC]
	void RecibirDamage(){				
		//Debug.Log("Es el mio me resta vida ");		
		this.barraVida.size -= vidaResta;
		ActivarAudio(audioGettingHit);
		//Debug.Log("Vida que me quita -> " + vidaResta);
		/*if( barraVida.size <= 0){
			anim.SetTrigger("Dead");			
			Destroy (gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
			Debug.Log("tiempo  anim -> " + anim.GetCurrentAnimatorStateInfo(0).length);
			
			Destroy(controles);			
			Destroy(managerMultijugador);			
			PhotonNetwork.automaticallySyncScene = true;
			PhotonNetwork.LoadLevel("Menu");
			PhotonNetwork.Disconnect();
			//SceneManager.LoadScene("Menu");
			Debug.Log("En teoria cambio de descena ");			
		}*/
	}
	
	public void FinPartida(int seg){		
		finPartida = true;
		tiempoFinPartida = seg * 100;
	}
	
	void ReinicioJuego(){
		Destroy(controles);			
		Destroy(managerMultijugador);			
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.LoadLevel("MenuEleccion");
		PhotonNetwork.Disconnect();
	}
	
	
	
	/* Actualiza el nivel del jugador */
	public void ActulizarLvl(){
		ActivarAudio(audioLevelUP);
		WWWForm form = new WWWForm(); // Creación del formulario		
		form.AddField("editUserNivel", namePlayer.text); // Agregación parametro y valor		
		WWW www = new WWW(URL_UPDATE, form); // Envio del formulario		
		int pos = nivelPlayer.text.IndexOf(" "); // posicion donde se separan por espacio " "
		int nuevoLvl = int.Parse(nivelPlayer.text.Substring(pos + 1))+1; // suma 1 al nivel actual
		nivelPlayer.text = "Lvl "+nuevoLvl.ToString();						
	}
	
	/* Actualiza los premios del jugador */
	public void ActulizarPremios(){
		WWWForm form = new WWWForm(); // Creación del formulario		
		form.AddField("editUserPremio", namePlayer.text); // Agregación parametro y valor		
		WWW www = new WWW(URL_UPDATE, form); // Envio del formulario				
	}
	
	

	/* Devuelve el nivel del jugador */
	public int NivelJugador(){
		int pos = nivelPlayer.text.IndexOf(" "); // posicion donde se separan por espacio " "		
		return int.Parse(nivelPlayer.text.Substring(pos + 1)); // nivel actual
	}
	
	/* Velocidad extra (Gasta mana) */
	public void VelocidadExtra(){
		// Comprueba si hay mana suficiente y tambien que no haya ninguna habilidad especial activada
		if(mana.size >= 0.02f && !skillAtaque && !skillDefensa && !skillVelocidad){				
			mana.size -= 0.2f;
			speed *= 2;	
			skillVelocidad = true;
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador		
		}		
	}
		
	/* Defensa extra te regenera vida (Gasta mana) */
	public void DefensaExtra(){
		// Comprueba si hay mana suficiente y que no este ya activada la habilidad o la de velocidad
		if(mana.size >= 0.02f && !skillDefensa && !skillVelocidad){	
			mana.size -= 0.2f;		
			skillDefensa = true;
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador
		}
	}
	
	/* Ataque especial (Gasta mana) */	
	public void AtaqueEspecial(){	
		// Busco la posición actual del jugador para crear las llamas a su lado
		GameObject jug = GameObject.FindWithTag("Player");		
		// Comprueba si hay mana suficiente y que no este ya activada la habilidad o la de velocidad
		if(mana.size >= 0.02f && !skillAtaque && !skillVelocidad){
					
			mana.size -= 0.2f;		
			Vector3 trans;
			// posicion Y del ataque a crear 
			float posY = -90;
			// Crea 5 ataques 
			for(int i = 0; i < 5; i++){
				/* La posición se crea respecto a la posición del jugador */
				// Posición del ataque, eje Y desde -90 hata 90 con distancia de 30 en cada ataque, y el eje x se mantiene 90 (a la derecha del jugador)
				trans = new Vector3(jug.transform.position.x + 90 , jug.transform.position.y + posY, 0f);	
				// Añado los ataques creados a la lista, los instancio a través de la red 
				listaAtaqueEsp.Add((GameObject)PhotonNetwork.Instantiate("Ataque Especial", trans, Quaternion.identity, 0));
				posY += 30;				
			}
			// Reinicio la posición en Y a -90 y el eje x se mantien en -90 (a la izquierda del jugador)
			posY = -90;
			for(int i = 0; i < 5; i++){
				trans = new Vector3(jug.transform.position.x + -90 , jug.transform.position.y + posY, 0f);																		
				listaAtaqueEsp.Add((GameObject)PhotonNetwork.Instantiate("Ataque Especial", trans, Quaternion.identity, 0));
				posY += 30;				
			}
			skillAtaque = true;
			PlayerPrefs.SetFloat("mana", mana.size); // Actualiza las preferencias del jugador
		}
	}
	
	/* Elimina todos los objetos que se crearon */		
	void EliminarAtaqueEspecial(){

		if(listaAtaqueEsp.Count != 0){
			foreach(GameObject ataque in listaAtaqueEsp){
				// Elimino los objetos en red para todos
				PhotonNetwork.Destroy(ataque);
			}
			// Reinicio la lista para que este vacía 
			listaAtaqueEsp = new List<GameObject>();
		}
	}

		
}