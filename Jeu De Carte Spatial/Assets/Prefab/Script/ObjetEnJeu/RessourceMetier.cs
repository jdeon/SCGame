using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RessourceMetier : MonoBehaviour, ISelectionnable, IAvecCapacite {

	private string typeRessource;

	private Joueur joueur;

	private int production;

	private TextMesh txtProd;

	private TextMesh txtStock;

	private string prefixeRessourceProd;

	private string prefixeRessourceStock;

	private List<CapaciteMetier> listCapaciteRessource = new List<CapaciteMetier> ();

	private int idSelectionnable;

	private int etatSelectionne;

	void OnMouseOver()
	{
		if (!joueur.CarteEnVisuel) {
			EtatSelectionnable = SelectionnableUtils.ETAT_MOUSE_OVER;
		}
	}

	void OnMouseExit()
	{
		if (!joueur.CarteEnVisuel) {
			EtatSelectionnable = SelectionnableUtils.ETAT_RETOUR_ATTIERE;
		}
	}

	public void Start(){
		if (joueur.isServer) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;
			joueur.RpcInitRessourceIdSelectionnable (idSelectionnable, typeRessource);
		}
	}
			
	public void init (Joueur joueurPossesseur, string typeRessource){
		this.joueur = joueurPossesseur;
		this.typeRessource = typeRessource;

		Production = RessourceUtils.getValeurProdInitialeRessource(typeRessource);
		Stock = RessourceUtils.getValeurStockInitialeRessource(typeRessource);

		this.prefixeRessourceProd = RessourceUtils.getPrefixeProd (typeRessource);
		this.prefixeRessourceStock = RessourceUtils.getPrefixeStock (typeRessource);

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

	public void updateVisual(){
		txtProd.text = prefixeRessourceProd + ProductionWithCapacity;
		txtStock.text = prefixeRessourceStock + StockWithCapacity;
	}

	public void syncListCapacityFromServer(List<CapaciteMetier> listMetierServer){
		if(null != listMetierServer){
			this.listCapaciteRessource = listMetierServer;
			updateVisual ();
		}
	}


	/*******************ISelectionnable****************/
	public void onClick (){
		EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
		if (!joueur.CarteEnVisuel && this.etatSelectionne == 1 && null != eventTask && eventTask is EventTaskChoixCible) {
			((EventTaskChoixCible)eventTask).ListCibleChoisie.Add (this);
		}

	}

	public int EtatSelectionnable {
		get { return etatSelectionne; }
		set {
			if (value == SelectionnableUtils.ETAT_RETOUR_ATTIERE) {
				SelectionnableUtils.miseEnBrillance (etatSelectionne, transform);
			} else {
				SelectionnableUtils.miseEnBrillance (value, transform);
				if (value != SelectionnableUtils.ETAT_MOUSE_OVER) {
					etatSelectionne = value;
				}
			}
		}
	}

	public int IdISelectionnable {
		get { return idSelectionnable; }
		set {
			if (null == idSelectionnable || idSelectionnable < 1) {
				idSelectionnable = value;
			}
		}
	}

	public NetworkInstanceId Possesseur { 
		get { return NetIdJoueurPossesseur; }
	}

	/*******************IAvecCapacity*****************/

	public void addCapacity (CapaciteMetier capaToAdd){

		if (capaToAdd.IdTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE) {
			capaToAdd.transformToAddMode (ProductionWithCapacity);
		} else if (capaToAdd.IdTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE){
			capaToAdd.transformToAddMode (StockWithCapacity);
		}

		listCapaciteRessource.Add (capaToAdd);
	}

	public int removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapaciteRessource) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapaciteRessource.Remove(capaciteToDelete);
		}

		if (capacitesToDelete.Count > 0) {
			synchroniseListCapacite ();
		}

		return capacitesToDelete.Count;
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
		
	public NetworkInstanceId NetIdJoueurPossesseur {
		get { return joueur.netId; }
	}

	/*************************Getter et setter*************/
	public NetworkInstanceId NetIdJoueur {
		get { return joueur.netId; }
	}
		
	public string TypeRessource {
		get { return typeRessource; }
	}

	public int Production { 
		get { return production; }
		set {
			production = value;
			if (TypeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_XP && production >= 10) {
				int gainNiveau = production / 10;
				Stock += gainNiveau;
				production -= gainNiveau * 10;
			}
		}
	}

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