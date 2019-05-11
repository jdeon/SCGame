using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class EmplacementMetierAbstract : NetworkBehaviour, IConteneurCarte, ISelectionnable {

	protected int numColone;

	[SyncVar]
	protected NetworkInstanceId idJoueurPossesseur;

	[SyncVar]
	protected NetworkInstanceId idCarte;

	public int idSelectionnable;

	protected int etatSelectionnable;

	public void Start(){
		numColone = transform.GetSiblingIndex () + 1;
		if (isServer) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;
			RpcInitIdSelectionnable (idSelectionnable);
		}
	}
		
	public void OnMouseDown(){
		onClick ();
	}

	public void putCard(CarteMetierAbstract cartePoser, bool isNewCard, NetworkInstanceId netIdTaskEvent){
		//Si c'est une nouvelle carte, on lance les capacités pour les cartes posées
		if (isNewCard) {
			ActionEventManager.EventActionManager.CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable,ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION, netIdTaskEvent);
		} else if (this is EmplacementAttaque) {
			ActionEventManager.EventActionManager.CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE, netIdTaskEvent);
		} else {
			ActionEventManager.EventActionManager.CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART, netIdTaskEvent);
		}
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

		if (null != ressourceMetal && carteSelectionne is CarteConstructionMetierAbstract && ressourceMetal.StockWithCapacity >= ((CarteConstructionMetierAbstract)carteSelectionne).getCoutMetal ()) {
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


	/*******************ISelectionnable****************/
	public abstract void onClick ();

	public void miseEnBrillance(int etat){
		//TODO mise en brillance
	}
		
	public int IdISelectionnable {
		get { return idSelectionnable; }
	}

	public int EtatSelectionnable{
		get {return etatSelectionnable;}
	}

	[ClientRpc]
	public void RpcInitIdSelectionnable(int idSelectionnableFromServer){
		this.idSelectionnable = idSelectionnableFromServer;
	}

	/************************Getter Setter ***************/
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
