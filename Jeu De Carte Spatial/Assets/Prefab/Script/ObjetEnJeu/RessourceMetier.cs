﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RessourceMetier : MonoBehaviour, ISelectionnable, IAvecCapacite {

	[SerializeField]
	private string typeRessource;

	private Joueur joueur;

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


	public void init (Joueur joueurPossesseur){
		this.joueur = joueurPossesseur;
		Production = 1;
		Stock = 20; //TODO change to 3

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

		txtProd.text = prefixeRessourceProd + ProductionWithCapacity;
		txtStock.text = prefixeRessourceStock + StockWithCapacity;
	}

	public bool payerRessource(int nbDemande){
			bool result = false;
		if (StockWithCapacity >= nbDemande) {
			Stock -= nbDemande;
			result = true;
		}

		return result;
	}

	public void updateVisual(){
		txtProd.text = prefixeRessourceProd + ProductionWithCapacity;
		txtStock.text = prefixeRessourceProd + StockWithCapacity;
	}

	public void syncListCapacityFromServer(List<CapaciteMetier> listMetierServer){
		if(null != listMetierServer){
			this.listCapaciteRessource = listMetierServer;
			updateVisual ();
		}
	}


	/*******************ISelectionnable****************/
	public void onClick (){
		Joueur localJoueur = JoueurUtils.getJoueurLocal ();
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
		joueur.RpcSyncCapaciteListRessource (listeCapaData, TypeRessource);
	}

	/*************************Getter et setter*************/
	public NetworkInstanceId NetIdJoueur {
		get { return joueur.netId; }
	}
		
	public string TypeRessource {
		get { return typeRessource; }
	}

	public int Production { get; set; }

	public int Stock{ get; set; }

	public int ProductionWithCapacity {
		get { 
			return  CapaciteUtils.valeurAvecCapacite (Production, listCapaciteRessource, ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE); 
		}
	}

	public int StockWithCapacity {
		get { 
			return CapaciteUtils.valeurAvecCapacite (Stock, listCapaciteRessource, ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE); 
		}
	}
}