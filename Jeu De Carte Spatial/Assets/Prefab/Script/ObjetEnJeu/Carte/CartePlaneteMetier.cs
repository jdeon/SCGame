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
	private TextMesh txtProdMetal;
	private TextMesh txtStockMetal;
	private TextMesh txtXPActuel;
	private TextMesh txtStockNiveau;
	private TextMesh txtProdCarburant;
	private TextMesh txtStockCarburant;

	public CartePlaneteMetier (NetworkInstanceId netIdJoueur, GameObject goParent){
		initAffichageDonnePlanete (goParent);

		this.netIdJoueur = netIdJoueur;
		pointVie = maxPVPlanete;
		prodMetal = 1;
		stockMetal = 0;
		xpActuel = 0;
		stockNiveau = 0;
		prodCarburant = 1;
		stockCarburant = 0;
	}

	public override CarteDTO getCarteDTORef (){
		//TODO rajouter carte palnete avec pseudo et image avatar
		return null;
	}

	public override Color getColorCarte (){
		return Color.white;
	}

	protected override void initId (){
		//TODO a implementer
	}

	/**Retourne si l'init est faite*/
	public override bool initCarteRef (CarteDTO carteRef){
		//TODO a implementer
		return false;
	}

	public void initAffichageDonnePlanete(GameObject goParent){
		GameObject goRessource = new GameObject ("Ressource");
		goRessource.transform.SetParent (goParent.transform);
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
		
	public void onChangePointVie(int PV){
		//TODO add txtPointVie
	}

	public void onChangeProdMetal(int prodMetal){
		txtProdMetal.text = "Prod M - " + prodMetal;
	}

	public void onChangeStockMetal(int stockMetal){
		txtStockMetal.text = "Stock M - " + stockMetal;
	}

	public void onChangeXPActuel(int xp){
		txtXPActuel.text = "XP - " + xp;
	}

	public void onChangeStockNiveau(int stockNiveau){
		txtStockNiveau.text = "Stock niv - " + stockNiveau;
	}

	public void onChangeProdCarburant(int prodCarburant){
		txtProdCarburant.text = "Prod C - " + prodCarburant;
	}

	public void onChangeStockCarburant(int stockCarburant){
		txtStockCarburant.text = "Stock C - " + stockCarburant;
	}
}