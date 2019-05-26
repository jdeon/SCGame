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

	[SerializeField]
	[SyncVar]
	protected int idSelectionnable;

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

	void OnMouseOver()
	{
		EtatSelectionnable = SelectionnableUtils.ETAT_MOUSE_OVER;
	}

	void OnMouseExit()
	{
		EtatSelectionnable = SelectionnableUtils.ETAT_RETOUR_ATTIERE;
	}

	public void putCard(CarteMetierAbstract cartePoser, bool isNewCard, NetworkInstanceId netIdTaskEvent){
		//Si c'est une nouvelle carte, on lance les capacités pour les cartes posées
		if (isNewCard) {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable,ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION, netIdTaskEvent, false);
		} else if (this is EmplacementAttaque) {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE, netIdTaskEvent, false);
		} else {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (cartePoser.netId, cartePoser.getJoueurProprietaire().netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART, netIdTaskEvent, false);
		}
	}
		
	public void putCard(CarteMetierAbstract cartePoser){

		if (null != cartePoser && null != cartePoser.getJoueurProprietaire () && cartePoser.getJoueurProprietaire ().isLocalPlayer) {
			Transform trfmCard = cartePoser.transform;
			trfmCard.parent = transform;;

			cartePoser.CmdChangeParent (this.netId, "");


			trfmCard.localPosition = new Vector3 (0, .01f, 0);
			trfmCard.localRotation = Quaternion.identity;
			trfmCard.localScale = Vector3.one;

			cartePoser.getJoueurProprietaire ().CarteSelectionne = null;
		}
	}

	[ClientRpc]
	public void RpcPutCard(NetworkInstanceId netIdCartePoser){

		CarteMetierAbstract cartePoser = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCartePoser, true);
		putCard (cartePoser);
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

	public int IdISelectionnable {
		get { return idSelectionnable; }
	}

	public int EtatSelectionnable{
		get {return etatSelectionnable;}
		set {
			if (value == SelectionnableUtils.ETAT_RETOUR_ATTIERE) {
				SelectionnableUtils.miseEnBrillance (etatSelectionnable, transform);
			} else {
				SelectionnableUtils.miseEnBrillance (value, transform);
				if (value != SelectionnableUtils.ETAT_MOUSE_OVER) {
					etatSelectionnable = value;
				}
			}
		}
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
