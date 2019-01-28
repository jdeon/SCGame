using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DeckMetierAbstract : NetworkBehaviour, IConteneurCarte, IAvecCapacite, ISelectionnable {

	protected Joueur joueurProprietaire;

	[SyncVar  (hook = "onChangeNetIdJoueur")]
	protected NetworkInstanceId netIdJoueur;

	protected List<CapaciteMetier> listCapaciteDeck = new List<CapaciteMetier> ();

	protected int etatSelectionnable;


	public abstract void intiDeck (NetworkInstanceId joueurNetId);

	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();


	public void OnMouseDown(){
		onClick ();
	}

	public int getNbCartePiochePointDegat(){
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





	/*****************	IContenerCarte *****************/

	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == this.netIdJoueur;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		return new List<CarteMetierAbstract> (transform.GetComponentsInChildren<CarteMetierAbstract> ());
	}

	public void putCard(CarteMetierAbstract carte){
		//TODO ajout carte
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
		RpcSyncCapaciteList (listeCapaData);
	}

	[ClientRpc]
	public void RpcSyncCapaciteList(byte[] listeCapaData){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);
		if (null != listCapacite) {
			this.listCapaciteDeck = listCapacite;
		}
	}

	/*******************ISelectionnable****************/
	public void onClick (){
		//TODO selectionne
		Joueur localJoueur = Joueur.getJoueurLocal ();
		if (this.etatSelectionnable == 1 && null != localJoueur.PhaseChoixCible && !localJoueur.PhaseChoixCible.finChoix) {
			localJoueur.PhaseChoixCible.listCibleChoisi.Add (this);

		}
	}

	public void miseEnBrillance(int etat){
		//TODO mise en brillance
	}

	public int EtatSelectionnable{
		get { return etatSelectionnable; }
	}


	/*******************************Hook************************/
	protected void onChangeNetIdJoueur(NetworkInstanceId netIdJoueur){
		this.netIdJoueur = netIdJoueur;
		joueurProprietaire = Joueur.getJoueur (netIdJoueur);
	}

	public void setClientNetIdJoueur(NetworkInstanceId netIdJoueur){
		this.netIdJoueur = netIdJoueur;
		joueurProprietaire = Joueur.getJoueur (netIdJoueur);
	}

	[ClientRpc]
	public void RpcInitIdJoueur(NetworkInstanceId joueurNetId){
		onChangeNetIdJoueur (joueurNetId);
	}

	public NetworkInstanceId NetIdJoueur {
		get { return netIdJoueur; }
	}

	public List<CapaciteMetier> ListCapaciteDeck {
		get{ return listCapaciteDeck; }
	}
}
