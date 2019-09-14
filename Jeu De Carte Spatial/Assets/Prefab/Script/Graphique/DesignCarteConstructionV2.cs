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
	private int premierNivDescription;

	//TODO utilsé l'id de la carte pour le nommage des objet
	public DesignCarteConstructionV2 (CarteConstructionMetierAbstract carteSource, GameObject goParent, float height, float width, Joueur joueurGenerateur){

		this.goParent = goParent;
		this.joueurGenerateur = joueurGenerateur;
		this.carteSource = carteSource;
		string typCarte = CarteUtils.getTypeCard (carteSource);

		Dictionary<string,Sprite> dictionnarySpriteCard = UIUtils.dictionnaryOfCardSprite[typCarte];

		Image imageConteneur = UIUtils.createImage(dictionnarySpriteCard[UIUtils.KEY_IMAGE], "ImageConteneur",goParent,
			width*(ConstanteInGame.propDesignImage.x-0.5f),height*(0.5f-ConstanteInGame.propDesignImage.y),
			width*ConstanteInGame.propDesignImage.z,height*ConstanteInGame.propDesignImage.w);

		imageCarte = UIUtils.createMaskImage(dictionnarySpriteCard[UIUtils.KEY_IMAGE], ConstanteInGame.spriteTest, "Image",goParent,
			width*(ConstanteInGame.propDesignImage.x-0.5f),height*(0.5f-ConstanteInGame.propDesignImage.y),
			.95f*width*ConstanteInGame.propDesignImage.z,.95f*height*ConstanteInGame.propDesignImage.w);


		GameObject goTitre = UIUtils.createPanel("Titre",goParent, dictionnarySpriteCard[UIUtils.KEY_TITRE],
			width*(ConstanteInGame.propDesignTitre.x-0.5f),height*(0.5f-ConstanteInGame.propDesignTitre.y),
			width*ConstanteInGame.propDesignTitre.z,height*ConstanteInGame.propDesignTitre.w);

		txtTitre = UIUtils.createText("txtTitre",goTitre,2, 0, 0, .95f*width*ConstanteInGame.propDesignTitre.z, .95f*height*ConstanteInGame.propDesignTitre.w);

		//Creation bloc des ressources
		float widthRessource = width*ConstanteInGame.propDesignRessource.z;
		float heightRessource = height*ConstanteInGame.propDesignRessource.w;

		GameObject paternRessource = UIUtils.createPanel("Ressource",goParent, null,
			width*(ConstanteInGame.propDesignRessource.x-0.5f),height*(0.5f-ConstanteInGame.propDesignRessource.y),
			widthRessource,heightRessource);

		GameObject paternRessourceMetal = UIUtils.createPanel("RessourceMetal",paternRessource, dictionnarySpriteCard[UIUtils.KEY_RESSOURCE],
			widthRessource*(ConstanteInGame.propDesignMetalRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignMetalRessource.y),
			widthRessource*ConstanteInGame.propDesignMetalRessource.z,heightRessource*ConstanteInGame.propDesignMetalRessource.w);
		paternRessourceMetal.transform.localScale = new Vector3 (-1, -1, 1);
		txtMetal = UIUtils.createText ("textMetal", paternRessourceMetal,1, 0, 0,
			.75f*widthRessource*ConstanteInGame.propDesignMetalRessource.z, .75f*heightRessource*ConstanteInGame.propDesignMetalRessource.w);
		txtMetal.gameObject.transform.localScale = new Vector3 (-1, -1, 1);
		txtMetal.alignment = TextAnchor.MiddleCenter;

		paternRessourceCarburant = UIUtils.createPanel("RessourceCarburant",paternRessource, dictionnarySpriteCard[UIUtils.KEY_RESSOURCE],
			widthRessource*(ConstanteInGame.propDesignCarburantRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignCarburantRessource.y),
			widthRessource*ConstanteInGame.propDesignCarburantRessource.z,heightRessource*ConstanteInGame.propDesignCarburantRessource.w);
		paternRessourceCarburant.transform.localScale = new Vector3 (1, -1, 1);
		txtCarburant = UIUtils.createText ("textCarburant", paternRessourceCarburant,1, 0, 0,
			.75f*widthRessource*ConstanteInGame.propDesignCarburantRessource.z, .75f*heightRessource*ConstanteInGame.propDesignCarburantRessource.w);
		txtCarburant.alignment = TextAnchor.MiddleCenter;
		txtCarburant.gameObject.transform.localScale = new Vector3 (1, -1, 1);

		GameObject paternListNiveaux = UIUtils.createPanel("ListNiveaux",goParent, dictionnarySpriteCard[UIUtils.KEY_DESCRIPTION],
			width*(ConstanteInGame.propDesignListNiveaux.x-0.5f),height*(0.5f-ConstanteInGame.propDesignListNiveaux.y),
			width*ConstanteInGame.propDesignListNiveaux.z,height*ConstanteInGame.propDesignListNiveaux.w);

		collapseGroup = paternListNiveaux.AddComponent<UICollapseGroup> ();
		List<UICollapseElement> listCollapseElement = new List<UICollapseElement> ();

		this.premierNivDescription = 1;
		for (int i = 1; i <= carteSource.getCarteRef ().ListNiveau.Count; i++) {
			//Le premier niveau n'est pas affiché si le titre est vide
			if (i > 1 || carteSource.getCarteRef ().ListNiveau [0].TitreNiveau != "") {
				UICollapseElement collapseElement;
				List<CapaciteMannuelleDTO> listCapaManuelleCarte = carteSource.getCarteRef ().ListNiveau [i - 1].CapaciteManuelle;

				if (null != listCapaManuelleCarte && listCapaManuelleCarte.Count > 0) {
					collapseElement = paternListNiveaux.AddComponent<UICollapseUseCapa> ();
					((UICollapseUseCapa)collapseElement).NumLvl = i;
					((UICollapseUseCapa)collapseElement).ListCapaManuelle = listCapaManuelleCarte;
					((UICollapseUseCapa)collapseElement).CarteSource = carteSource;
				} else {
					collapseElement = paternListNiveaux.AddComponent<UICollapseElement> ();
				}

				collapseElement.TailleTitre = 50;
				collapseElement.TailleDescription = 75;
				collapseElement.TempsDecompression = 3;
				collapseElement.Titre = carteSource.getCarteRef ().ListNiveau [i-1].TitreNiveau;
				collapseElement.Description = carteSource.getCarteRef ().ListNiveau [i-1].DescriptionNiveau;
				listCollapseElement.Add (collapseElement);
			} else {
				this.premierNivDescription = 2;
			}
		}

		collapseGroup.ListCollapseElement = listCollapseElement;
		collapseGroup.initializeGroup ();

		gestionAffichageDesBoutons ();

		/**
		GameObject paternBouton = UIUtils.createPanel("BoutonAction",goParent,
			width*(ConstanteInGame.propDesignBouton.x-0.5f),height*(0.5f-ConstanteInGame.propDesignBouton.y),
			width*ConstanteInGame.propDesignBouton.z,height*ConstanteInGame.propDesignBouton.w);
		Button buttonAction = paternBouton.AddComponent<Button> ();
		*/

		GameObject paternRessourceNiveauActuel = UIUtils.createPanel("RessourceNiveauActuel",paternRessource, dictionnarySpriteCard[UIUtils.KEY_NIVEAU],
			widthRessource*(ConstanteInGame.propDesignNiveauRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignNiveauRessource.y),
			widthRessource*ConstanteInGame.propDesignNiveauRessource.z,heightRessource*ConstanteInGame.propDesignNiveauRessource.w);
		txtNiveauActuel = UIUtils.createText ("textNiveauActuel", paternRessourceNiveauActuel,1, 0, 0,
			.75f*widthRessource*ConstanteInGame.propDesignNiveauRessource.z, .75f*heightRessource*ConstanteInGame.propDesignNiveauRessource.w);
		txtNiveauActuel.alignment = TextAnchor.MiddleCenter;

		paternPA = UIUtils.createPanel("PointAttaque",goParent, dictionnarySpriteCard[UIUtils.KEY_POINT_DEF_ATT],
			width*(ConstanteInGame.propDesignPointAttaque.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointAttaque.y),
			width*ConstanteInGame.propDesignPointAttaque.z,height*ConstanteInGame.propDesignPointAttaque.w);
		txtPointAttaque = UIUtils.createText ("textPA", paternPA,1, 0, 0,
			.75f*width*ConstanteInGame.propDesignPointAttaque.z,.75f*height*ConstanteInGame.propDesignPointAttaque.w);
		txtPointAttaque.alignment = TextAnchor.MiddleCenter;

		GameObject paternPD = UIUtils.createPanel("PointDefense",goParent, dictionnarySpriteCard[UIUtils.KEY_POINT_DEF_ATT],
			width*(ConstanteInGame.propDesignPointDefense.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointDefense.y),
			width*ConstanteInGame.propDesignPointDefense.z,height*ConstanteInGame.propDesignPointDefense.w);
		txtPointDefense = UIUtils.createText ("textPD", paternPD,1, 0, 0,
			.75f*width*ConstanteInGame.propDesignPointDefense.z,.75f*height*ConstanteInGame.propDesignPointDefense.w);
		txtPointDefense.alignment = TextAnchor.MiddleCenter;

		GameObject paternBoutonCancel = UIUtils.createPanel("BoutonFermeture",goParent, ConstanteInGame.spriteCroixCancel,
			width*(ConstanteInGame.propBoutonRetour.x-0.5f),height*(0.5f-ConstanteInGame.propBoutonRetour.y),
			width*ConstanteInGame.propBoutonRetour.z,height*ConstanteInGame.propBoutonRetour.w);
		Button buttonFermeture = paternBoutonCancel.AddComponent<Button> ();
		buttonFermeture.onClick.AddListener (deleteVisual);
	}

	private void deleteVisual(){
		joueurGenerateur.CarteEnVisuel = false;
		goParent.SetActive (false);
	}

	private void showConfirmAddLevel(){

		int nbMetalNecessaire = carteSource.getCoutMetal (carteSource.NiveauActuel + 1);

		if (!(carteSource.getConteneur() is EmplacementMetierAbstract)) {
			UIDialogInfo infoDialog = new UIDialogInfo ("Carte ne peut evoluer que sur le terrain");
			infoDialog.showDialog ();
		}else if (nbMetalNecessaire > joueurGenerateur.RessourceMetal.StockWithCapacity) {
			UIDialogInfo infoDialog = new UIDialogInfo ("Vous n'avez pas assez de metal pour faire evoluer la carte");
			infoDialog.showDialog ();
		} else if (joueurGenerateur.RessourceXP.StockWithCapacity <= 0) {
			UIDialogInfo infoDialog = new UIDialogInfo ("Vous n'avez plus de stock de niveau");
			infoDialog.showDialog ();
		} else if (carteSource.NiveauActuel >= 5) {
			UIDialogInfo infoDialog = new UIDialogInfo ("La carte est deja au niveau maximum");
			infoDialog.showDialog ();
		} else {
			UIConfirmDialog confirmDialog = new UIConfirmDialog ("Souhaitez-vous augmenter le niveau de la carte contre " + nbMetalNecessaire + " metal");
			confirmDialog.BtnValid.onClick.AddListener (evolCard);
			confirmDialog.BtnValid.onClick.AddListener (confirmDialog.hideDialog);
			confirmDialog.showDialog ();
		}

	}

	private void evolCard(){
		carteSource.CmdCreateEvolTask (joueurGenerateur.netId, 1);
	}

	private void gestionAffichageDesBoutons(){
		for (int i = 0; i < collapseGroup.ListCollapseElement.Count; i++) {
			gestionBoutonNiveau (collapseGroup.ListCollapseElement[i], i + premierNivDescription);
		}
	}

	private void gestionBoutonNiveau(UICollapseElement element, int lvl){
		Text textBtnLvl = getTextBouton (element.BoutonAction);
		bool isJoueurPossesseur = carteSource.NetIdJoueurPossesseur == joueurGenerateur.netId;
		element.BoutonAction.onClick.RemoveAllListeners ();

		if (!isJoueurPossesseur || lvl > carteSource.NiveauActuel) {
			textBtnLvl.text = "M-" + carteSource.getCoutMetal (lvl);

			if (isJoueurPossesseur && lvl == carteSource.NiveauActuel + 1) {
				element.BoutonAction.onClick.AddListener (showConfirmAddLevel);
				element.BoutonAction.interactable = true;
			} else {
				element.BoutonAction.interactable = false;
			}
		} else if (element is UICollapseUseCapa) {
			((UICollapseUseCapa)element).gestionBoutonNiveau (textBtnLvl);

		} else {
			element.BoutonAction.gameObject.SetActive (false);
		}
	}

	public void reinitDebutTour(){
		foreach (UICollapseElement element in collapseGroup.ListCollapseElement) {
			if (element is UICollapseUseCapa) {
				((UICollapseUseCapa)element).reinitDebutTour ();
			}
		}
	}

	private Text getTextBouton(Button bouton){
		Text textBtnLvl;

		if (null != bouton.gameObject.GetComponentInChildren<Text> ()) {
			textBtnLvl = bouton.gameObject.GetComponentInChildren<Text> ();
		} else {
			textBtnLvl = UIUtils.createTextStretch ("Txt" + bouton.gameObject.name, bouton.gameObject, 10, 0, 0, 0, 0);
		}

		return textBtnLvl;
	}

	public void setTitre (string titre){
		txtTitre.text = titre;
	}

	public void setMetal (int numMetal){
		txtMetal.text = "M-" + numMetal;
	}

	public void setNiveauActuel (int numNiveau){
		txtNiveauActuel.text = "" + numNiveau;
		gestionAffichageDesBoutons ();
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
