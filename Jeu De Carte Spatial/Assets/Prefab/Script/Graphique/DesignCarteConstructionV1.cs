using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesignCarteConstructionV1 {

	//public GameObject paternCarteConstruction;

	private Text txtTitre;
	private Text txtMetal;
	private Text txtNiveauActuel;
	private Text txtCarburant;
	private Text txtDescription;
	private Text txtCitation;

	private Text txtPointAttaque;
	private Text txtPointDefense;

	private Image imageCarte;

	private GameObject paternRessourceCarburant;
	private GameObject paternPA;


	/*************************Propriété en proportion sur le design des carte
	 * list de propriété d'élément en partant d'en haut à gauche par rapport au parent
	 * Vector4 (propotionPositionX, propotionPositionY, propotionLargeur, propotionHauteur)
	 * /

	/******Carte construction ******/

	/******Propriété carte niveau 1****/
	public static readonly Vector4  propDesignTitre = new Vector4 (.5f, .05f, .95f, .067f);
	public static readonly Vector4  propDesignImage = new Vector4 (.5f, .233f, .95f, .25f);
	public static readonly Vector4  propDesignRessource = new Vector4 (.5f, .425f, .95f, .083f);
	public static readonly Vector4  propDesignBlocDescription = new Vector4 (.5f, .575f, .95f, .183f);
	public static readonly Vector4  propDesignListNiveaux = new Vector4 (.5f, .8f, .95f, .233f);
	public static readonly Vector4  propDesignBouton = new Vector4 (.5f, .963f, .25f, .042f);
	public static readonly Vector4  propDesignPointAttaque = new Vector4 (.075f, .963f, .125f, .058f);
	public static readonly Vector4  propDesignPointDefense = new Vector4 (.925f, .963f, .125f, .058f);


	/******Propriété carte niveau 2****/
	/******Sous propriété de ressource****/
	public static readonly Vector4  propDesignMetalRessource = new Vector4 (.224f, .5f, .263f, .4f);
	public static readonly Vector4  propDesignNiveauRessource = new Vector4 (.5f, .5f,.105f, .8f);
	public static readonly Vector4  propDesignCarburantRessource = new Vector4 (.776f, .5f, .263f, .4f);

	/******Sous propriété de ressource****/
	public static readonly Vector4  propDesignDescription = new Vector4 (.5f, .341f, .9f, .682f);
	public static readonly Vector4  propDesignCitation = new Vector4 (.5f, .841f,.9f, .318f);

	//TODO utilsé l'id de la carte pour le nommage des objet
	public DesignCarteConstructionV1 (GameObject goParent, float height, float width){
		txtTitre = createText("Titre",goParent,1,
			width*(propDesignTitre.x-0.5f),height*(0.5f-propDesignTitre.y),
			width*propDesignTitre.z,height*propDesignTitre.w);
		
		imageCarte = createImage(null, "Image",goParent,
			width*(propDesignImage.x-0.5f),height*(0.5f-propDesignImage.y),
			width*propDesignImage.z,height*propDesignImage.w);

		//Creation bloc des ressources
		float widthRessource = width*propDesignRessource.z;
		float heightRessource = height*propDesignRessource.w;

		GameObject paternRessource = createPanel("Ressource",goParent,
			width*(propDesignRessource.x-0.5f),height*(0.5f-propDesignRessource.y),
			widthRessource,heightRessource);

		GameObject paternRessourceMetal = createPanel("RessourceMetal",paternRessource,
			widthRessource*(propDesignMetalRessource.x-0.5f),heightRessource*(0.5f-propDesignMetalRessource.y),
			widthRessource*propDesignMetalRessource.z,heightRessource*propDesignMetalRessource.w);
		txtMetal = createText ("textMetal", paternRessourceMetal,1, 0, 0,
			.9f*widthRessource*propDesignMetalRessource.z, heightRessource*propDesignMetalRessource.w);

		GameObject paternRessourceNiveauActuel = createPanel("RessourceNiveauActuel",paternRessource,
			widthRessource*(propDesignNiveauRessource.x-0.5f),heightRessource*(0.5f-propDesignNiveauRessource.y),
			widthRessource*propDesignNiveauRessource.z,heightRessource*propDesignNiveauRessource.w);
		txtNiveauActuel = createText ("textNiveauActuel", paternRessourceNiveauActuel,1, 0, 0,
			.9f*widthRessource*propDesignNiveauRessource.z, heightRessource*propDesignNiveauRessource.w);

		paternRessourceCarburant = createPanel("RessourceCarburant",paternRessource,
			widthRessource*(propDesignCarburantRessource.x-0.5f),heightRessource*(0.5f-propDesignCarburantRessource.y),
			widthRessource*propDesignCarburantRessource.z,heightRessource*propDesignCarburantRessource.w);
		txtCarburant = createText ("textCarburant", paternRessourceCarburant,1, 0, 0,
			.9f*widthRessource*propDesignCarburantRessource.z, heightRessource*propDesignCarburantRessource.w);

		//Creation bloc de description
		float widthDescription = width*propDesignBlocDescription.z;
		float heightDescription = height*propDesignBlocDescription.w;
		GameObject paternDescriptionCita = createPanel("BlocDescription",goParent,
			width*(propDesignBlocDescription.x-0.5f),height*(0.5f-propDesignBlocDescription.y),
			widthDescription,heightDescription);

		txtDescription = createText ("Description",paternDescriptionCita,4,
			widthDescription*(propDesignDescription.x-0.5f),heightDescription*(0.5f-propDesignDescription.y),
			widthDescription*propDesignDescription.z,heightDescription*propDesignDescription.w);
		
		txtCitation = createText ("Citation", paternDescriptionCita,2,
			widthDescription*(propDesignCitation.x-0.5f),heightDescription*(0.5f-propDesignCitation.y),
			widthDescription*propDesignCitation.z,heightDescription*propDesignCitation.w);
		txtCitation.fontStyle = FontStyle.Italic;


		GameObject paternListNiveaux = createPanel("ListNiveaux",goParent,
			width*(propDesignListNiveaux.x-0.5f),height*(0.5f-propDesignListNiveaux.y),
			width*propDesignListNiveaux.z,height*propDesignListNiveaux.w);

		//TODO Transforme en bouton
		GameObject paternBouton = createPanel("BoutonAction",goParent,
			width*(propDesignBouton.x-0.5f),height*(0.5f-propDesignBouton.y),
			width*propDesignBouton.z,height*propDesignBouton.w);

		paternPA = createPanel("PointAttaque",goParent,
			width*(propDesignPointAttaque.x-0.5f),height*(0.5f-propDesignPointAttaque.y),
			width*propDesignPointAttaque.z,height*propDesignPointAttaque.w);
		txtPointAttaque = createText ("textPA", paternPA,1, 0, 0,
			.9f*width*propDesignPointAttaque.z,height*propDesignPointAttaque.w);

		GameObject paternPD = createPanel("PointDefense",goParent,
			width*(propDesignPointDefense.x-0.5f),height*(0.5f-propDesignPointDefense.y),
			width*propDesignPointDefense.z,height*propDesignPointDefense.w);
		txtPointDefense = createText ("textPD", paternPD,1, 0, 0,
			.9f*width*propDesignPointDefense.z,height*propDesignPointDefense.w);
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

	public void setDescription (string description){
		txtDescription.text = description;
	}

	public void setCitation (string citation){
		txtCitation.text = "\"" + citation + "\"";
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
		//paternImage.GetComponent<Image> ().sprite = carteRef.image; //TODO carte Ref doit être un sprite
	}



	//Cree un panel
	private GameObject createPanel (string name, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject panelGO = new GameObject (name);
		panelGO.AddComponent<CanvasRenderer> ();
		panelGO.transform.SetParent (goParent.transform, false);
		panelGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Image i = panelGO.AddComponent<Image> ();
		i.sprite = ConstanteInGame.spriteBackgroundCarte;
		i.color = new Color (.75f, .75f, .75f, .5f);

		panelGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		panelGO.GetComponent<RectTransform>().ForceUpdateRectTransforms();

		return panelGO;
	}

	private Image createImage (Sprite image, string nameGO, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject imageGO = new GameObject (nameGO);
		imageGO.AddComponent<CanvasRenderer> ();
		imageGO.transform.SetParent (goParent.transform, false);
		imageGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Image i = imageGO.AddComponent<Image> ();
		i.sprite = image;

		imageGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		return i;
	}

	private Text createText (string nameGO, GameObject goParent, int nbLigneAttendu, float anchorX, float anchorY, float width, float height){
		GameObject textGO = new GameObject (nameGO);
		textGO.AddComponent<CanvasRenderer> ();
		textGO.transform.SetParent (goParent.transform, false);
		textGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Text text = textGO.AddComponent<Text> ();
		text.font = ConstanteInGame.fontArial;
		text.color = Color.black;
		text.fontSize = (int)(height * .75f / nbLigneAttendu);

		textGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		return text;
	}

}
