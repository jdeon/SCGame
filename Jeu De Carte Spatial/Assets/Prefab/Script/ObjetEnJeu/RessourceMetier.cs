using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RessourceMetier : NetworkBehaviour, ISelectionnable, IAvecCapacite {

	[SerializeField]
	private string typeRessource;

	[SyncVar]
	private NetworkInstanceId netIdJoueur;

	[SyncVar (hook = "onChangeProd")]
	private int production;

	[SyncVar (hook = "onChangeStock")]
	private int stock;

	private TextMesh txtProd;

	private TextMesh txtStock;

	private string prefixeRessourceProd;

	private string prefixeRessourceStock;

	private List<CapaciteMetier> listCapaciteRessource = new List<CapaciteMetier> ();

	private int etatSelectionne;

	//TODO cree des constante
	private static string getPrefixeProd(string typeRessource){
		string result;

		if(typeRessource == "Metal"){
			result = "Prod M -";
		} else if(typeRessource == "Carburant"){
			result = "Prod C - ";
		} else if(typeRessource == "XP"){
			result = "XP - ";
		} else {
			result = "";
		}

		return result;
	}

	private static string getPrefixeStock(string typeRessource){
		string result;

		if(typeRessource == "Metal"){
			result = "Stock M -";
		} else if(typeRessource == "Carburant"){
			result = "Stock C - ";
		} else if(typeRessource == "XP"){
			result = "Niv - ";
		} else {
			result = "";
		}

		return result;
	}

	public void init (NetworkInstanceId netIdJoueur){
		this.netIdJoueur = netIdJoueur;
		production = 1;
		stock = 20; //TODO change to 3

		this.prefixeRessourceProd = getPrefixeProd (typeRessource);
		this.prefixeRessourceStock = getPrefixeStock (typeRessource);

		Transform tProd = transform.Find("Prod");
		if(null != tProd){
			txtProd = tProd.GetComponentInChildren<TextMesh>();
		}

		Transform tStock = transform.Find("Stock");
		if(null != tStock){
			txtStock = tStock.GetComponentInChildren<TextMesh>();
		}

		txtProd.text = prefixeRessourceProd + Production;
		txtStock.text = prefixeRessourceStock + Stock;
	}

	public void productionDeRessourceByServer(){
		stock += Production;
	}

	public bool payerRessource(int nbDemande){
			bool result = false;
		if (stock >= nbDemande) {
			stock -= nbDemande;
			result = true;
		}

		return result;
	}

	/*******************ISelectionnable****************/
	public void onClick (){
		Joueur localJoueur = Joueur.getJoueurLocal ();
		if (this.etatSelectionne == 1 && null != localJoueur.PhaseChoixCible && !localJoueur.PhaseChoixCible.finChoix) {
			localJoueur.PhaseChoixCible.listCibleChoisi.Add (this);
		}
	}

	public void miseEnBrillance(int etat){
		//TODO mise en brillance
	}

	public int EtatSelectionnable {
		get { return etatSelectionne; }
	}

	/*******************IAvecCapacity*****************/

	public void addCapacity (CapaciteMetier capaToAdd){
		listCapaciteRessource.Add (capaToAdd);
		//TODO recalculate visual
	}

	public void removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteRessource) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapaciteRessource.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}

	public void capaciteFinTour (){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteRessource) {
			bool existeEncore = capacite.endOfTurn ();
			if (!existeEncore) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapaciteRessource.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}

	public List<CapaciteMetier> containCapacityOfType(int idTypCapacity){
		List<CapaciteMetier> listCapacite = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteRessource) {
			if (capacite.IdTypeCapacite == idTypCapacity) {
				listCapacite.Add (capacite);
			}
		}
		return listCapacite;
	}

	public bool containCapacityWithId (int idCapacityDTO){
		bool contain = false;

		foreach (CapaciteMetier capacite in listCapaciteRessource) {
			if (capacite.IdCapaciteProvenance == idCapacityDTO) {
				contain = true;
				break;
			}
		}
		return contain;
	}

	public void synchroniseListCapacite (){
		byte[] listeCapaData = SerializeUtils.SerializeToByteArray(this.listCapaciteRessource);
		RpcSyncCapaciteList (listeCapaData);
	}

	[ClientRpc]
	public void RpcSyncCapaciteList(byte[] listeCapaData){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);
		if (null != listCapacite) {
			this.listCapaciteRessource = listCapacite;
		}
		txtProd.text = prefixeRessourceProd + Production;
		txtStock.text = prefixeRessourceProd + Stock;
	}

	/*************************Hook*********************/
	public void onChangeProd(int prod){
		if (null != txtProd) {
			txtProd.text = prefixeRessourceProd + prod;
		}
	}

	public void onChangeStock(int stock){
		if (null != txtStock) {
			txtStock.text = prefixeRessourceStock + stock;
		}
	}


	/*************************Getter et setter*************/
	public NetworkInstanceId NetIdJoueur {
		get { return netIdJoueur; }
	}


	public string TypeRessource {
		get { return typeRessource; }
	}

	public int Production {
		get { 
			return  CapaciteUtils.valeurAvecCapacite (this.production, listCapaciteRessource, ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE); 
		}
		set { production = value; }
	}

	public int Stock {
		get { 
			return CapaciteUtils.valeurAvecCapacite (this.stock, listCapaciteRessource, ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE); 
		}
		set { stock = value; }
	}


}
