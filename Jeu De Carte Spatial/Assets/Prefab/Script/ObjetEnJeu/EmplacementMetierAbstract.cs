using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementMetierAbstract : NetworkBehaviour {

	[SerializeField]
	protected int numColone;

	[SyncVar]
	protected NetworkInstanceId idJoueurPossesseur;

	[SyncVar]
	protected NetworkInstanceId idCarte;

	[SyncVar]
	protected bool libre = true;

	public void setIdJoueurPossesseur(NetworkInstanceId idJoueurPossesseur){
		this.idJoueurPossesseur = idJoueurPossesseur;
	}

	public void putCard(CarteConstructionMetierAbstract cartePoser){
		Transform trfmCard = cartePoser.transform;

		cartePoser.OnBoard = true;

		trfmCard.SetParent (transform);
		trfmCard.localPosition = new Vector3 (0, .01f, 0);
		trfmCard.localRotation = Quaternion.identity;
		trfmCard.localScale = Vector3.one;

		libre = false;
	}

	public bool isCardCostPayable(CartePlaneteMetier cartePlanetJoueur, CarteMetierAbstract carteSelectionne){
		bool costPayable = false;

		if (null != cartePlanetJoueur && carteSelectionne is CarteConstructionMetierAbstract && cartePlanetJoueur.isMetalSuffisant (((CarteConstructionMetierAbstract)carteSelectionne).getCoutMetal ())) {
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
			systemTour.CmdGetPlayerPhase (joueur.netId);

			if (systemTour.Phase == TourJeuSystem.PHASE_DEPLACEMENT
			   && null != joueur.carteSelectionne && joueur.netId == joueur.carteSelectionne.getJoueurProprietaire ().netId) {
				movable = true;
			}
		}
			
		return movable;
	}

	public NetworkInstanceId NetIdCartePosee {
		get{return idCarte;}
	}
}
