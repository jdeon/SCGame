using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CartePlaneteMetier : CarteMetierAbstract, IConteneurCarte, IVulnerable {

	public static int maxPVPlanete = 30;

	[SyncVar (hook = "onChangePointVie")]
	public int pointVie;

	private NetworkInstanceId netIdJoueur;
	private TextMesh txtPointVie;


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

	}


	/*****************	IContenerCarte *****************/

	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == this.netIdJoueur;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		List<CarteMetierAbstract> listCartesContenues = new List<CarteMetierAbstract> ();
		listCartesContenues.Add (this);
		return listCartesContenues;
	}

	/****************** IVulnerable **********************/


	public IEnumerator recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat){
		pointVie -= nbDegat;

		if (pointVie <= 0) {
			destruction ();
		}

		yield return null;
	}

	public IEnumerator destruction (){
		//TODO fonction victoire

		yield return null;
	}


	/*************Getter et Setter ************************/


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
			    && null != joueurLocal.CarteSelectionne && joueurLocal.CarteSelectionne.getJoueurProprietaire () != joueurProprietaire
			    && joueurLocal.CarteSelectionne is IAttaquer && !((IAttaquer)joueurLocal.CarteSelectionne).isCapableAttaquer ()) {
				//TODO vérifier aussi l'état cable d'attaquer (capacute en cours, déjà sur une autre attaque)
				ActionEventManager.EventActionManager.CmdAddNewCoroutine();
				StartCoroutine(((IAttaquer)joueurLocal.CarteSelectionne).attaquePlanete (this, ActionEventManager.EventActionManager.nextIdCoroutine));
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
	}


	public void putCard(CarteMetierAbstract carte){
		//TODO ajout de module
	}
		
	public void onChangePointVie(int PV){
		if (null != txtPointVie) {
			txtPointVie.text = "PV - " + PV;
		}
	}
}