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
		//production = 1;
		//stock = 20; //TODO change to 3

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

		txtProd.text = prefixeRessourceProd + production;
		txtStock.text = prefixeRessourceStock + stock;
	}

	public void productionDeRessourceByServer(){
		stock += production;
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
		//TODO selectionne

	}

	public void miseEnBrillance(){
		//TODO mise en brillance
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
		get { return production; }
		set { production = value; }
	}

	public int Stock {
		get { return stock; }
		set { stock = value; }
	}


}
