using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CartePlaneteMetier : CarteMetierAbstract, IVulnerable, IConteneurCarte {

	public static int maxPVPlanete = 30;

	[SyncVar (hook = "onChangePointVie")]
	public int pointVie;

	private TextMesh txtPointVie;


	private string pseudo;

	public static CartePlaneteMetier getPlaneteEnnemie(NetworkInstanceId idJoueur){
		CartePlaneteMetier planeteResult = null;
		CartePlaneteMetier[] listPlanete = GameObject.FindObjectsOfType<CartePlaneteMetier> ();

		if (null != listPlanete && listPlanete.Length > 0) {
			foreach (CartePlaneteMetier planete in listPlanete) {
				if (planete.idJoueurProprietaire != idJoueur) {
					planeteResult = planete;
					break;
				}
			}
		}

		return planeteResult;
	}

	public void initPlaneteServer (NetworkInstanceId netIdJoueur, string pseudo){
		this.pseudo = pseudo;
		initId ();

		//TODO remettre stocke base à 0
		this.idJoueurProprietaire = netIdJoueur;
		onChangeNetIdJoueur (netIdJoueur);
		pointVie = maxPVPlanete;

	}
	[Command]
	public override void CmdPiocheCard (){
		//TODO
	}


	/*****************	IContenerCarte *****************/
	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == this.idJoueurProprietaire;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		List<CarteMetierAbstract> listCartesContenues = new List<CarteMetierAbstract> ();
		listCartesContenues.Add (this);
		return listCartesContenues;
	}
		
	public void putCard(CarteMetierAbstract carte){

		if (null != carte && null != carte.getJoueurProprietaire () && carte.getJoueurProprietaire ().isLocalPlayer) {
		//TODO ajout de module ou changer héros?
		}
	}

	[ClientRpc]
	public void RpcPutCard(NetworkInstanceId netIdCarte){

		CarteMetierAbstract carte = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarte, true);
		putCard (carte);
	}

	/****************** IVulnerable **********************/
	public void recevoirAttaque (CarteMetierAbstract sourceDegat, NetworkInstanceId netIdEventTask, bool attaqueSimultane){
		JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, sourceDegat.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netIdEventTask, attaqueSimultane);
	}

	public int recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat, NetworkInstanceId netIdEventTask){
		pointVie -= nbDegat;

		if (pointVie <= 0) {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, sourceDegat.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE, netIdEventTask, false);
		}

		return pointVie;
	}

	public void destruction (NetworkInstanceId netdTaskEvent){
		//TODO fonction victoire

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

	public override void onClick(){
		Joueur joueurLocal = JoueurUtils.getJoueurLocal ();

		if (null != joueurLocal) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			//Si un joueur clique sur une carte capable d'attaquer puis sur une carte ennemie cela lance une attaque
			if (systemTour.getPhase (joueurLocal.netId) == TourJeuSystem.PHASE_ATTAQUE
			    && null != joueurLocal.CarteSelectionne && joueurLocal.CarteSelectionne.getJoueurProprietaire () != joueurProprietaire
			    && joueurLocal.CarteSelectionne is IAttaquer && ((IAttaquer)joueurLocal.CarteSelectionne).isCapableAttaquer ()) {
				//TODO vérifier aussi l'état cable d'attaquer (capacute en cours, déjà sur une autre attaque)
				JoueurUtils.getJoueurLocal ().CmdCreateTask (joueurLocal.CarteSelectionne.netId, joueurLocal.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE, NetworkInstanceId.Invalid, false);
			} else {
				base.onClick ();
			}
		} else {
			base.onClick ();
		}	
	}

	public override void generateVisualCard()
	{
		//TODO 
	}

	public override void generateGOCard(){
		BoxCollider colidCarte = gameObject.AddComponent<BoxCollider> ();
		colidCarte.size = new Vector3 (1.5f, .1f, 2f);
		colidCarte.center = new Vector3 (0, .15f, 0);

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
		
	public void onChangePointVie(int PV){
		if (null != txtPointVie) {
			txtPointVie.text = "PV - " + PV;
		}
	}
}