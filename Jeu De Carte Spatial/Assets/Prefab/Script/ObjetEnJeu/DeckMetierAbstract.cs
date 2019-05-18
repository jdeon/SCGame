using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DeckMetierAbstract : NetworkBehaviour, IConteneurCarte, IAvecCapacite, ISelectionnable {

	protected Joueur joueurProprietaire;

	protected List<CapaciteMetier> listCapaciteDeck = new List<CapaciteMetier> ();

	protected int idSelectionnable;

	protected int etatSelectionnable;




	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();

	public void Start(){
		if (joueurProprietaire.isServer) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;

			if (this is DeckConstructionMetier) {
				joueurProprietaire.RpcInitDeckIdSelectionnable (idSelectionnable, "Construction");
			} /*else if (this is DeckAmelirationMetier) {
			joueurProprietaire.RpcSyncCapaciteListDeck (listeCapaData, "Amelioration");
			}*/
		}
	}

	public void OnMouseDown(){
		onClick ();
	}

	public virtual void intiDeck (Joueur joueurInitiateur, bool isServer){
		this.joueurProprietaire = joueurInitiateur;
	}

	public int getNbCartePioche(){
		int nbCartePioche = 1;

		if( null != ListCapaciteDeck){
			foreach(CapaciteMetier capaciteCourante in ListCapaciteDeck){
				if (capaciteCourante.IdTypeCapacite.Equals (ConstanteIdObjet.ID_CAPACITE_MODIF_NB_CARTE_PIOCHE)) {
					nbCartePioche = capaciteCourante.getNewValue (nbCartePioche);
				}
			}
		}

		return nbCartePioche;
	}

	public void syncListCapacityFromServer(List<CapaciteMetier> listMetierServer){
		if(null != listMetierServer){
			this.listCapaciteDeck = listMetierServer;
		}
	}

	/*****************	IContenerCarte *****************/
	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == joueurProprietaire.netId;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		return new List<CarteMetierAbstract> (transform.GetComponentsInChildren<CarteMetierAbstract> ());
	}
		
	public void putCard(CarteMetierAbstract carte){

		if (null != carte && null != carte.getJoueurProprietaire () && carte.getJoueurProprietaire ().isLocalPlayer) {
			//TODO ungenerated card on client

			Transform trfmCard = carte.transform;
			trfmCard.parent = transform;

			//TODO délpacer à un index au hasard
			carte.CmdChangeParent (this.NetIdJoueur, JoueurUtils.getPathJoueur (this));

			carte.getJoueurProprietaire ().CarteSelectionne = null;
		}
	}

	[ClientRpc]
	public void RpcPutCard(NetworkInstanceId netIdcarte){

		CarteMetierAbstract carte = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdcarte, true);
		putCard (carte);
	}


	/*********************************IAvecCapacite*********************/
	public void addCapacity (CapaciteMetier capaToAdd){
		listCapaciteDeck.Add (capaToAdd);
		//TODO recalculate visual
	}

	public void removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteDeck) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapaciteDeck.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}

	public void capaciteFinTour (){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteDeck) {
			bool existeEncore = capacite.endOfTurn ();
			if (!existeEncore) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapaciteDeck.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}
		
	public List<CapaciteMetier>  containCapacityOfType(int idTypCapacity){
		List<CapaciteMetier> listCapacite = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteDeck) {
			if (capacite.IdTypeCapacite == idTypCapacity) {
				listCapacite.Add (capacite);
			}
		}
		return listCapacite;
	}

	public bool containCapacityWithId (int idCapacityDTO){
		bool contain = false;

		foreach (CapaciteMetier capacite in listCapaciteDeck) {
			if (capacite.IdCapaciteProvenance == idCapacityDTO) {
				contain = true;
				break;
			}
		}
		return contain;
	}

	public void synchroniseListCapacite (){
		byte[] listeCapaData = SerializeUtils.SerializeToByteArray(this.listCapaciteDeck);
		string typeDeck = "";

		if (this is DeckConstructionMetier) {
			joueurProprietaire.RpcSyncCapaciteListDeck (listeCapaData, "Construction");
		} /*else if (this is DeckAmelirationMetier) {
			joueurProprietaire.RpcSyncCapaciteListDeck (listeCapaData, "Amelioration");
		}*/

	}

	/*******************ISelectionnable****************/
	public void onClick (){
		//TODO selectionne
		EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
		if (this.etatSelectionnable == 1 && null != eventTask && eventTask is EventTaskChoixCible) {
			((EventTaskChoixCible) eventTask).ListCibleChoisie.Add (this);
		}

	}

	public void miseEnBrillance(int etat){
		//TODO mise en brillance
	}


	public int IdISelectionnable {
		get { return idSelectionnable; }
		set {
			if (null == idSelectionnable || idSelectionnable < 1) {
				idSelectionnable = value;
			}
		}
	}

	public int EtatSelectionnable{
		get { return etatSelectionnable; }
	}


	/********************Getter et Setter************************/
	public NetworkInstanceId NetIdJoueur {
		get { return joueurProprietaire.netId; }
	}

	public List<CapaciteMetier> ListCapaciteDeck {
		get{ return listCapaciteDeck; }
	}
}
