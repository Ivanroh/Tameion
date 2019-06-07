using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Clase para actualizar los datos del jugador en el ranking 
*/

public class Ranking : MonoBehaviour
{
	/* Componetes tipo Text donde se escrbirán los datos del jugador */
	public Text nombre;
	public Text nivel;
	public Text premios;
   
   /* Actualizar los datos por los obtenidos en los parámetros */
   public void ActualizarItem(string nombreJug, string nivelJug, string premiosJug){
	   nombre.text = nombreJug;
	   nivel.text = nivelJug;
	   premios.text = premiosJug;
   }
}
