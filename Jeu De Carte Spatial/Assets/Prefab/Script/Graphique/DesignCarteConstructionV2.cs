using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	private Joueur joueurCliquant;

	//TODO utilsé l'id de la carte pour le nommage des objet
	public DesignCarteConstructionV2 (GameObject goParent, float height, float width, int nbNiveau, Joueur joueurClick){

		this.goParent = goParent;
		this.joueurCliquant = joueurClick;

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

		for (int i = 0; i < nbNiveau; i++) {
			UICollapseElement collapseElement = paternListNiveaux.AddComponent<UICollapseElement> ();
			collapseElement.TailleTitre = 50;
			collapseElement.TailleDescription = 75;
			collapseElement.TempsDecompression = 3;
			listCollapseElement.Add (collapseElement);
		}

		collapseGroup.ListCollapseElement = listCollapseElement;


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

	void deleteVisual(){
		joueurCliquant.carteEnVisuel = false;
		GameObject.Destroy (goParent);
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

	public void setNiveau(int numNiv, string titre, string description, int cout){
		UICollapseElement collapseElement = collapseGroup.ListCollapseElement [numNiv - 1];

		collapseElement.Titre = titre;
		collapseElement.Description = description;

		if (cout > 0) {
			collapseElement.Titre += " (" + cout + ")";
		}
	}
}
