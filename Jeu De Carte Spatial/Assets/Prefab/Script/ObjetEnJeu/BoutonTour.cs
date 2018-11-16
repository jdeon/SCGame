using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoutonTour : NetworkBehaviour {

	public enum enumEtatBouton {terminerTour, attaque, enAttente}

	[SyncVar  (hook = "onChangeEtatBouton")]
	public enumEtatBouton etatBouton;

	private TextMesh txtEtat;

	private static readonly string STR_TERMINER_TOUR = "Terminer le tour";
	private static readonly string STR_ATTAQUE = "Attaquer";
	private static readonly string STR_EN_ATTENTE = "En attente";

	public void Start(){
		txtEtat = GenerateObjectUtils.createText ("boutonEtat", new Vector3 (0, 0.51f, 0), Quaternion.identity, Vector3.one, 14, gameObject);

		etatBouton = enumEtatBouton.enAttente;
	}

	public void onChangeEtatBouton(enumEtatBouton newEtat){
		txtEtat.text = etatToText (newEtat);
	}

	public void initTour (){
		etatBouton = enumEtatBouton.terminerTour;
	}

	//Affiche la carte si clique dessus
	public virtual void OnMouseDown()
	{
		//TODO faire evoluer cycle tour
	}

	private string etatToText(enumEtatBouton etat){
		string result;

		switch (etat) {
		case enumEtatBouton.terminerTour: 
			result = STR_TERMINER_TOUR;
			break;
		case enumEtatBouton.attaque: 
			result = STR_ATTAQUE;
			break;
		case enumEtatBouton.enAttente: 
			result = STR_EN_ATTENTE;
			break;
		default :
			result = "";
			break;
		}

		return result;
	}
	
}
