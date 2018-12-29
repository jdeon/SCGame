using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DeckMetierAbstract : NetworkBehaviour, IConteneurCarte, IAvecCapacite, ISelectionnable {

	protected Joueur joueurProprietaire;

	[SyncVar  (hook = "onChangeNetIdJoueur")]
	protected NetworkInstanceId netIdJoueur;

	protected List<CapaciteMetier> listCapaciteDeck = new List<CapaciteMetier> ();

	public abstract void intiDeck (NetworkInstanceId joueurNetId);

	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();

	public int getNbCartePiochePointDegat(){
		int nbCartePioche = 1;

		if( null != ListCapaciteDeck){
			foreach(CapaciteMetier capaciteCourante in ListCapaciteDeck){
				if (capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_MODIF_NB_CARTE_PIOCHE)) {
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
		
	public List<CapaciteMetier>  containCapacity(int idTypCapacity){
		List<CapaciteMetier> listCapacite = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteDeck) {
			if (capacite.getIdTypeCapacite() == idTypCapacity) {
				listCapacite.Add (capacite);
			}
		}
		return listCapacite;
	}


	/*******************ISelectionnable****************/
	public void onClick (){
		//TODO selectionne

	}

	public void miseEnBrillance(){
		//TODO mise en brillance
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
