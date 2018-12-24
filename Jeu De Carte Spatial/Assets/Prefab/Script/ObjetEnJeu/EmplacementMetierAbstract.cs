using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementMetierAbstract : NetworkBehaviour, IConteneurCarte {

	protected int numColone;

	[SyncVar]
	protected NetworkInstanceId idJoueurPossesseur;

	[SyncVar]
	protected NetworkInstanceId idCarte;

	public void Start(){
		numColone = transform.GetSiblingIndex () + 1;
	}
		

	public void putCard(CarteMetierAbstract cartePoser){
		Transform trfmCard = cartePoser.transform;

		if (cartePoser is CarteConstructionMetierAbstract) {
			((CarteConstructionMetierAbstract)cartePoser).OnBoard = true;
		}

		trfmCard.SetParent (transform);
		trfmCard.localPosition = new Vector3 (0, .01f, 0);
		trfmCard.localRotation = Quaternion.identity;
		trfmCard.localScale = Vector3.one;

		cartePoser.getJoueurProprietaire ().CarteSelectionne = null;
	}

	public bool isCardCostPayable(RessourceMetier ressourceMetal, CarteMetierAbstract carteSelectionne){
		bool costPayable = false;

		if (null != ressourceMetal && carteSelectionne is CarteConstructionMetierAbstract && ressourceMetal.payerRessource (((CarteConstructionMetierAbstract)carteSelectionne).getCoutMetal ())) {
			costPayable = true;
		}

		return costPayable;
	}

	/**
	 * Retourne si la carte est déplaçable par le joueur
	 * */
	protected bool isMovableByPlayer(Joueur joueur){
		bool movable = false;

		if (null != joueur && joueur.isLocalPlayer) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			if (systemTour.getPhase (joueur.netId) == TourJeuSystem.PHASE_DEPLACEMENT
			   && null != joueur.CarteSelectionne && joueur.netId == joueur.CarteSelectionne.getJoueurProprietaire ().netId) {
				movable = true;
			}
		}
			
		return movable;
	}

	/*****************	IContenerCarte *****************/

	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == this.idJoueurPossesseur;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		return new List<CarteMetierAbstract> (transform.GetComponentsInChildren<CarteMetierAbstract> ());
	}


	public NetworkInstanceId IdJoueurPossesseur {
		get{return this.idJoueurPossesseur;}
		set{this.idJoueurPossesseur = value;}
	}

	public NetworkInstanceId NetIdCartePosee {
		get{return idCarte;}
	}

	public int NumColonne {
		get{ return numColone; }
	}
}
