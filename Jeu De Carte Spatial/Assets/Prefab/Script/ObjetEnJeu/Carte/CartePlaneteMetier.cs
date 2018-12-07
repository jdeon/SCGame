using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CartePlaneteMetier : CarteMetierAbstract {

	public static int maxPVPlanete = 30;

	[SyncVar (hook = "onChangePointVie")]
	public int pointVie;

	[SyncVar (hook = "onChangeProdMetal")]
	public int prodMetal;

	[SyncVar (hook = "onChangeStockMetal")]
	public int stockMetal;

	[SyncVar (hook = "onChangeXPActuel")]
	public int xpActuel;

	[SyncVar (hook = "onChangeStockNiveau")]
	public int stockNiveau;

	[SyncVar (hook = "onChangeProdCarburant")]
	public int prodCarburant;

	[SyncVar (hook = "onChangeStockCarburant")]
	public int stockCarburant;

	private NetworkInstanceId netIdJoueur;
	private TextMesh txtPointVie;
	private TextMesh txtProdMetal;
	private TextMesh txtStockMetal;
	private TextMesh txtXPActuel;
	private TextMesh txtStockNiveau;
	private TextMesh txtProdCarburant;
	private TextMesh txtStockCarburant;

	private string pseudo;

	public static CartePlaneteMetier getPlaneteEnnemie(NetworkInstanceId idJoueur){
		CartePlaneteMetier planeteResult = null;
		CartePlaneteMetier[] listPlanete = GameObject.FindObjectsOfType<CartePlaneteMetier> ();

		if (null != listPlanete && listPlanete.Length > 0) {
			foreach (CartePlaneteMetier planete in listPlanete) {
				if (planete.netIdJoueur != idJoueur) {
					planeteResult = planete;
					break;
				}
			}
		}

		return planeteResult;
	}

	public void initPlanete (NetworkInstanceId netIdJoueur, string pseudo){
		this.pseudo = pseudo;
		initId ();

		//TODO remettre stocke base à 0
		this.netIdJoueur = netIdJoueur;
		pointVie = maxPVPlanete;
		prodMetal = 1;
		stockMetal = 20;
		xpActuel = 0;
		stockNiveau = 0;
		prodCarburant = 1;
		stockCarburant = 20;
	}

	public void productionRessourceByServer(){
		if (isServer) {
			stockMetal += prodMetal;
			stockCarburant += prodCarburant;
		}
	}

	public override CarteDTO getCarteDTORef (){
		//TODO rajouter carte palnete avec pseudo et image avatar
		return null;
	}

	public override Color getColorCarte (){
		return Color.white;
	}

	protected override void initId (){
		id = "Planete_" + pseudo;
	}

	/**Retourne si l'init est faite*/
	public override bool initCarteRef (CarteDTO carteRef){
		//TODO a implementer
		return false;
	}

	public override void OnMouseDown(){
		Joueur joueurLocal = Joueur.getJoueurLocal ();

		if (null != joueurLocal) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			//Si un joueur clique sur une carte capable d'attaquer puis sur une carte ennemie cela lance une attaque
			if (systemTour.getPhase (joueurLocal.netId) == TourJeuSystem.PHASE_ATTAQUE
			    && null != joueurLocal.carteSelectionne && joueurLocal.carteSelectionne.getJoueurProprietaire () != joueurProprietaire
			    && joueurLocal.carteSelectionne is IAttaquer && !((IAttaquer)joueurLocal.carteSelectionne).isCapableAttaquer ()) {
				//TODO vérifier aussi l'état cable d'attaquer (capacute en cours, déjà sur une autre attaque)
				((IAttaquer)joueurLocal.carteSelectionne).attaquePlanete (this);
			} else {
				base.OnMouseDown ();
			}
		} else {
			base.OnMouseDown ();
		}	
	}

	public override void generateVisualCard()
	{
		//TODO 
	}

	public override void generateGOCard(){
		GameObject goCartePlanete = new GameObject("CartePlanete_" + id);
		goCartePlanete.transform.SetParent (gameObject.transform);
		goCartePlanete.transform.localPosition = new Vector3 (0,0.1f,0);
		goCartePlanete.transform.localRotation = Quaternion.identity;

		TextMesh txtPseudo = GenerateObjectUtils.createText ("pseudo", new Vector3 (0, 0.01f, .75f), Quaternion.identity, new Vector3 (2f, 1, .5f), 14, goCartePlanete);
		txtPseudo.text = this.pseudo;

		//TODO remplacer par image
		GameObject goImage = GameObject.CreatePrimitive (PrimitiveType.Plane);
		goImage.name = "Avatar_" + id;
		goImage.transform.SetParent (goCartePlanete.transform);
		goImage.transform.localPosition = new Vector3 (0, 0.01f,0);
		goImage.transform.localRotation = Quaternion.identity;
		goImage.transform.localScale = new Vector3(.2f,1,.1f);

		txtPointVie = GenerateObjectUtils.createText ("pointVie", new Vector3 (0, 0.01f, -.75f), Quaternion.identity, new Vector3 (2f, 1, .5f), 14, goCartePlanete);
		txtPointVie.text = "PV - " + pointVie;

		initAffichageDonnePlanete ();
	}

	public void initAffichageDonnePlanete(){
		GameObject goRessource = new GameObject ("Ressource");
		goRessource.transform.SetParent (gameObject.transform);
		goRessource.transform.localPosition = new Vector3 (0,0.1f,.5f);
		goRessource.transform.localRotation = Quaternion.identity;

		txtProdMetal = GenerateObjectUtils.createText ("ProdMetal", new Vector3 (-1.5f, 0.1f, .25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);
		txtStockMetal = GenerateObjectUtils.createText ("StockMetal", new Vector3 (-1.5f, 0.1f, -.25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);
		txtXPActuel = GenerateObjectUtils.createText ("xpActuel", new Vector3 (0, 0.1f, .25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);
		txtStockNiveau = GenerateObjectUtils.createText ("StockNiveau", new Vector3 (0f, 0.1f, -.25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);
		txtProdCarburant = GenerateObjectUtils.createText ("ProdCarburant", new Vector3 (1.5f, 0.1f, .25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);
		txtStockCarburant = GenerateObjectUtils.createText ("StockCarburant", new Vector3 (1.5f, 0.1f, -.25f), Quaternion.identity, new Vector3 (1, 1, .25f), 14, goRessource);

		txtProdMetal.text = "Prod M - " + prodMetal;
		txtStockMetal.text = "Stock M - " + stockMetal;
		txtXPActuel.text = "XP - " + xpActuel;
		txtStockNiveau.text = "Stock niv - " + stockNiveau;
		txtProdCarburant.text = "Prod C - " + prodCarburant;
		txtStockCarburant.text = "Stock C - " + stockCarburant;
	}

	public bool isMetalSuffisant (int nbMetalDemand){
		bool result = false;
		if (stockMetal >= nbMetalDemand) {
			stockMetal -= nbMetalDemand;
			result = true;
		}

		return result;
	}

	public bool isCarbuSuffisant (int nbCarbuDemande){
		bool result = false;
		if (stockCarburant >= nbCarbuDemande) {
			stockCarburant -= nbCarbuDemande;
			result = true;
		}

		return result;
	}
		
	public void onChangePointVie(int PV){
		if (null != txtPointVie) {
			txtPointVie.text = "PV - " + PV;
		}
	}

	public void onChangeProdMetal(int prodMetal){
		if (null != txtProdMetal) {
			txtProdMetal.text = "Prod M - " + prodMetal;
		}
	}

	public void onChangeStockMetal(int stockMetal){
		if (null != txtStockMetal) {
			txtStockMetal.text = "Stock M - " + stockMetal;
		}
	}

	public void onChangeXPActuel(int xp){
		if (null != txtXPActuel) {
			txtXPActuel.text = "XP - " + xp;
		}
	}

	public void onChangeStockNiveau(int stockNiveau){
		if (null != txtStockNiveau) {
			txtStockNiveau.text = "Stock niv - " + stockNiveau;
		}
	}

	public void onChangeProdCarburant(int prodCarburant){
		if (null != txtProdCarburant) {
			txtProdCarburant.text = "Prod C - " + prodCarburant;
		}
	}

	public void onChangeStockCarburant(int stockCarburant){
		if (null != txtStockCarburant) {
			txtStockCarburant.text = "Stock C - " + stockCarburant;
		}
	}
}