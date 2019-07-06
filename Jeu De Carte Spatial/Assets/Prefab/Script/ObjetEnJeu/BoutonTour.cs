using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoutonTour : NetworkBehaviour {

	public enum enumEtatBouton {terminerTour, attaque, enAttente}

	[SyncVar  (hook = "onChangeEtatBouton")]
	private enumEtatBouton etatBouton;

	private TextMesh txtEtat;

	private static readonly string STR_TERMINER_TOUR = "Terminer le tour";
	private static readonly string STR_ATTAQUE = "Attaquer";
	private static readonly string STR_EN_ATTENTE = "En attente";

	public void Start(){
		txtEtat = GenerateObjectUtils.createText ("boutonEtat", new Vector3 (0, 0.51f, 0), Quaternion.identity, Vector3.one, 14, gameObject);

		CmdSetEtatBouton(enumEtatBouton.enAttente);
		onChangeEtatBouton (enumEtatBouton.enAttente);
	}

	public void onChangeEtatBouton(enumEtatBouton newEtat){
		this.etatBouton = newEtat;
		txtEtat.text = etatToText (newEtat);
	}

	//Affiche la carte si clique dessus
	public virtual void OnMouseDown()
	{
		if(etatBouton == enumEtatBouton.attaque){
			CmdProgressStep(TourJeuSystem.PHASE_ATTAQUE);
			CmdSetEtatBouton(enumEtatBouton.terminerTour);
		} else if (etatBouton == enumEtatBouton.terminerTour){
			CmdProgressStep(TourJeuSystem.FIN_TOUR);
		}
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

	[Command]
	public void CmdSetEtatBouton (enumEtatBouton etatBouton){
		this.etatBouton = etatBouton;
	}
		
	public void setEtatBoutonServer (enumEtatBouton etatBouton){
		if (isServer) {
			this.etatBouton = etatBouton;
		}
	}

	public enumEtatBouton getEtatBouton(){
		return this.etatBouton;
	}
		
	[Command]
	public void CmdProgressStep(int actionPlayer){
		TourJeuSystem.getTourSystem ().progressStepServer (actionPlayer);
	}
}
