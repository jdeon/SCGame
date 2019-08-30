using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DesignCarteConstructionV2 {

	//public GameObject paternCarteConstruction;

	private Text txtTitre;
	private Text txtMetal;
	private Text txtNiveauActuel;
	private Text txtCarburant;

	private UICollapseGroup collapseGroup;

	private Text txtPointAttaque;
	private Text txtPointDefense;

	private Image imageCarte;

	private GameObject paternRessourceCarburant;
	private GameObject paternPA;

	private GameObject goParent;
	private Joueur joueurGenerateur;

	private CarteConstructionMetierAbstract carteSource;

	//TODO utilsé l'id de la carte pour le nommage des objet
	public DesignCarteConstructionV2 (CarteConstructionMetierAbstract carteSource, GameObject goParent, float height, float width, bool isJoueurPossesseur, Joueur joueurGenerateur){

		this.goParent = goParent;
		this.joueurGenerateur = joueurGenerateur;
		this.carteSource = carteSource;

		GameObject paternBoutonCancel = UIUtils.createPanel("BoutonFermeture",goParent,
			width*(ConstanteInGame.propBoutonRetour.x-0.5f),height*(0.5f-ConstanteInGame.propBoutonRetour.y),
			width*ConstanteInGame.propBoutonRetour.z,height*ConstanteInGame.propBoutonRetour.w);
		paternBoutonCancel.GetComponent<Image> ().sprite = ConstanteInGame.spriteCroixCancel;
		Button buttonFermeture = paternBoutonCancel.AddComponent<Button> ();
		buttonFermeture.onClick.AddListener (deleteVisual);

		txtTitre = UIUtils.createText("Titre",goParent,1,
			width*(ConstanteInGame.propDesignTitre.x-0.5f),height*(0.5f-ConstanteInGame.propDesignTitre.y),
			width*ConstanteInGame.propDesignTitre.z,height*ConstanteInGame.propDesignTitre.w);

		imageCarte = UIUtils.createImage(null, "Image",goParent,
			width*(ConstanteInGame.propDesignImage.x-0.5f),height*(0.5f-ConstanteInGame.propDesignImage.y),
			width*ConstanteInGame.propDesignImage.z,height*ConstanteInGame.propDesignImage.w);

		//Creation bloc des ressources
		float widthRessource = width*ConstanteInGame.propDesignRessource.z;
		float heightRessource = height*ConstanteInGame.propDesignRessource.w;

		GameObject paternRessource = UIUtils.createPanel("Ressource",goParent,
			width*(ConstanteInGame.propDesignRessource.x-0.5f),height*(0.5f-ConstanteInGame.propDesignRessource.y),
			widthRessource,heightRessource);

		GameObject paternRessourceMetal = UIUtils.createPanel("RessourceMetal",paternRessource,
			widthRessource*(ConstanteInGame.propDesignMetalRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignMetalRessource.y),
			widthRessource*ConstanteInGame.propDesignMetalRessource.z,heightRessource*ConstanteInGame.propDesignMetalRessource.w);
		txtMetal = UIUtils.createText ("textMetal", paternRessourceMetal,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignMetalRessource.z, heightRessource*ConstanteInGame.propDesignMetalRessource.w);

		GameObject paternRessourceNiveauActuel = UIUtils.createPanel("RessourceNiveauActuel",paternRessource,
			widthRessource*(ConstanteInGame.propDesignNiveauRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignNiveauRessource.y),
			widthRessource*ConstanteInGame.propDesignNiveauRessource.z,heightRessource*ConstanteInGame.propDesignNiveauRessource.w);
		txtNiveauActuel = UIUtils.createText ("textNiveauActuel", paternRessourceNiveauActuel,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignNiveauRessource.z, heightRessource*ConstanteInGame.propDesignNiveauRessource.w);
		if (isJoueurPossesseur) {
			Button buttonNiveau = paternRessourceNiveauActuel.AddComponent<Button> ();
			buttonNiveau.onClick.AddListener (showConfirmAddLevel);
		}

		paternRessourceCarburant = UIUtils.createPanel("RessourceCarburant",paternRessource,
			widthRessource*(ConstanteInGame.propDesignCarburantRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignCarburantRessource.y),
			widthRessource*ConstanteInGame.propDesignCarburantRessource.z,heightRessource*ConstanteInGame.propDesignCarburantRessource.w);
		txtCarburant = UIUtils.createText ("textCarburant", paternRessourceCarburant,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignCarburantRessource.z, heightRessource*ConstanteInGame.propDesignCarburantRessource.w);

		GameObject paternListNiveaux = UIUtils.createPanel("ListNiveaux",goParent,
			width*(ConstanteInGame.propDesignListNiveaux.x-0.5f),height*(0.5f-ConstanteInGame.propDesignListNiveaux.y),
			width*ConstanteInGame.propDesignListNiveaux.z,height*ConstanteInGame.propDesignListNiveaux.w);

		collapseGroup = paternListNiveaux.AddComponent<UICollapseGroup> ();
		List<UICollapseElement> listCollapseElement = new List<UICollapseElement> ();

		int premierNiveauAffiche = 1;
		for (int i = 1; i <= carteSource.getCarteRef ().ListNiveau.Count; i++) {
			//Le premier niveau n'est pas affiché si le titre est vide
			if (i > 1 || carteSource.getCarteRef ().ListNiveau [0].TitreNiveau != "") {
				UICollapseElement collapseElement = paternListNiveaux.AddComponent<UICollapseElement> ();
				collapseElement.TailleTitre = 50;
				collapseElement.TailleDescription = 75;
				collapseElement.TempsDecompression = 3;
				collapseElement.Titre = carteSource.getCarteRef ().ListNiveau [i-1].TitreNiveau;
				collapseElement.Description = carteSource.getCarteRef ().ListNiveau [i-1].DescriptionNiveau;
				listCollapseElement.Add (collapseElement);
			} else {
				premierNiveauAffiche = 2;
			}
		}

		collapseGroup.ListCollapseElement = listCollapseElement;
		collapseGroup.initializeGroup ();

		for (int i = 0; i < listCollapseElement.Count; i++) {
			gestionBoutonNiveau (listCollapseElement[i].BoutonAction, premierNiveauAffiche, isJoueurPossesseur);
			premierNiveauAffiche++;
		}

		GameObject paternBouton = UIUtils.createPanel("BoutonAction",goParent,
			width*(ConstanteInGame.propDesignBouton.x-0.5f),height*(0.5f-ConstanteInGame.propDesignBouton.y),
			width*ConstanteInGame.propDesignBouton.z,height*ConstanteInGame.propDesignBouton.w);
		Button buttonAction = paternBouton.AddComponent<Button> ();


		paternPA = UIUtils.createPanel("PointAttaque",goParent,
			width*(ConstanteInGame.propDesignPointAttaque.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointAttaque.y),
			width*ConstanteInGame.propDesignPointAttaque.z,height*ConstanteInGame.propDesignPointAttaque.w);
		txtPointAttaque = UIUtils.createText ("textPA", paternPA,1, 0, 0,
			.9f*width*ConstanteInGame.propDesignPointAttaque.z,height*ConstanteInGame.propDesignPointAttaque.w);

		GameObject paternPD = UIUtils.createPanel("PointDefense",goParent,
			width*(ConstanteInGame.propDesignPointDefense.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointDefense.y),
			width*ConstanteInGame.propDesignPointDefense.z,height*ConstanteInGame.propDesignPointDefense.w);
		txtPointDefense = UIUtils.createText ("textPD", paternPD,1, 0, 0,
			.9f*width*ConstanteInGame.propDesignPointDefense.z,height*ConstanteInGame.propDesignPointDefense.w);
	}

	private void deleteVisual(){
		joueurGenerateur.CarteEnVisuel = false;
		GameObject.Destroy (goParent);
	}

	private void showConfirmAddLevel(){

		int nbMetalNecessaire = carteSource.getCoutMetal (carteSource.NiveauActuel + 1);

		if (nbMetalNecessaire > joueurGenerateur.RessourceMetal.StockWithCapacity) {
			UIDialogInfo infoDialog = new UIDialogInfo ("Vous n'avez pas assez de metal pour faire evoluer la carte");
			infoDialog.showDialog ();
		} else if (carteSource.NiveauActuel >= 5) {
			UIDialogInfo infoDialog = new UIDialogInfo ("La carte est deja au niveau maximum");
			infoDialog.showDialog ();
		} else if (!(carteSource.getConteneur() is EmplacementMetierAbstract)) {
			UIDialogInfo infoDialog = new UIDialogInfo ("Carte ne peut evoluer que sur le terrain");
			infoDialog.showDialog ();
		} else {
			UIConfirmDialog confirmDialog = new UIConfirmDialog ("Souhaitez-vous augmenter le niveau de la carte contre " + nbMetalNecessaire + " metal");
			confirmDialog.BtnValid.onClick.AddListener (evolCard);
			confirmDialog.BtnValid.onClick.AddListener (confirmDialog.hideDialog);
			confirmDialog.showDialog ();
		}

	}

	private void evolCard(){
		EventTask eventTask =  ActionEventManager.EventActionManager.CreateTask (carteSource.netId, joueurGenerateur.netId, carteSource.IdISelectionnable,
			ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE, NetworkInstanceId.Invalid, false);
		eventTask.InfoComp = 1;
		
	}

	private void gestionBoutonNiveau(Button bouton, int lvl, bool isJoueurPossesseur){
		Text textBtnLvl = getTextBouton (bouton);

		if (!isJoueurPossesseur || lvl > carteSource.NiveauActuel) {
			textBtnLvl.text = "M-" + carteSource.getCoutMetal (lvl);

			if (isJoueurPossesseur && lvl == carteSource.NiveauActuel + 1) {
				bouton.onClick.AddListener (showConfirmAddLevel);
				bouton.interactable = true;
			} else {
				bouton.interactable = false;
			}
		} else if (lvl > 0 && lvl < carteSource.getCarteRef ().ListNiveau.Count
		           && carteSource.getCarteRef ().ListNiveau [lvl - 1].CapaciteManuelle.Count > 0) {
			List<CapaciteMannuelleDTO> listCapaManuelle = carteSource.getCarteRef ().ListNiveau [lvl - 1].CapaciteManuelle;

			bool actionTrouve = false;
			foreach (CapaciteMannuelleDTO capaManuelle in listCapaManuelle) {
				if(capaManuelle.PeriodeUtilisable.Contains("5-A")){ //TODO vérifier l'ID periode
					actionTrouve = true;
					break;
				}
			}

			if (actionTrouve) {
				textBtnLvl.text = "Use";
				bouton.onClick.RemoveAllListeners ();
				bouton.onClick.AddListener (useCapa);
				bouton.interactable = true;
			} else {
				bouton.gameObject.SetActive (false);
			}

		} else {
			bouton.gameObject.SetActive (false);
		}

	}

	private Text getTextBouton(Button bouton){
		Text textBtnLvl;

		if (null != bouton.gameObject.GetComponent<Text> ()) {
			textBtnLvl = bouton.gameObject.GetComponentInChildren<Text> ();
		} else {
			textBtnLvl = UIUtils.createTextStretch ("Txt" + bouton.gameObject.name, bouton.gameObject, 10, 0, 0, 0, 0);
		}

		return textBtnLvl;
	}

	private void useCapa(){
		//TODO
	}

	public void setTitre (string titre){
		txtTitre.text = titre;
	}

	public void setMetal (int numMetal){
		txtMetal.text = "M-" + numMetal;
	}

	public void setNiveauActuel (int numNiveau){
		txtNiveauActuel.text = "" + numNiveau;
	}

	public void setCarburant (int numCarburant){
		if(numCarburant>0){
			disableCarbu (false);
		} else {
			disableCarbu (true);
		}

		txtCarburant.text = "C-" + numCarburant;
	}

	public void setPA (int numPA){
		if(numPA>0){
			disablePA(false);
		} else {
			disablePA (true);
		}

		txtPointAttaque.text = "" + numPA;
	}

	public void setPD (int numPD){
		txtPointDefense.text = "" + numPD;
	}

	public void disableCarbu (bool disableCarbu){
		paternRessourceCarburant.SetActive(!disableCarbu);
	}

	public void disablePA (bool disableCarbu){
		paternPA.SetActive(!disableCarbu);
	}

	public void setImage(Sprite imageSource){
		imageCarte.sprite = imageSource; 
	}
}
