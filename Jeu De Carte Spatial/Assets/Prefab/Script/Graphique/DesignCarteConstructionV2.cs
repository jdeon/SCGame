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

	private Text txtPointAttaque;
	private Text txtPointDefense;

	private Image imageCarte;

	private GameObject paternRessourceCarburant;
	private GameObject paternPA;

	//TODO utilsé l'id de la carte pour le nommage des objet
	public DesignCarteConstructionV2 (GameObject goParent, float height, float width){

		txtTitre = createText("Titre",goParent,1,
			width*(ConstanteInGame.propDesignTitre.x-0.5f),height*(0.5f-ConstanteInGame.propDesignTitre.y),
			width*ConstanteInGame.propDesignTitre.z,height*ConstanteInGame.propDesignTitre.w);

		imageCarte = createImage(null, "Image",goParent,
			width*(ConstanteInGame.propDesignImage.x-0.5f),height*(0.5f-ConstanteInGame.propDesignImage.y),
			width*ConstanteInGame.propDesignImage.z,height*ConstanteInGame.propDesignImage.w);

		//Creation bloc des ressources
		float widthRessource = width*ConstanteInGame.propDesignRessource.z;
		float heightRessource = height*ConstanteInGame.propDesignRessource.w;

		GameObject paternRessource = createPanel("Ressource",goParent,
			width*(ConstanteInGame.propDesignRessource.x-0.5f),height*(0.5f-ConstanteInGame.propDesignRessource.y),
			widthRessource,heightRessource);

		GameObject paternRessourceMetal = createPanel("RessourceMetal",paternRessource,
			widthRessource*(ConstanteInGame.propDesignMetalRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignMetalRessource.y),
			widthRessource*ConstanteInGame.propDesignMetalRessource.z,heightRessource*ConstanteInGame.propDesignMetalRessource.w);
		txtMetal = createText ("textMetal", paternRessourceMetal,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignMetalRessource.z, heightRessource*ConstanteInGame.propDesignMetalRessource.w);

		GameObject paternRessourceNiveauActuel = createPanel("RessourceNiveauActuel",paternRessource,
			widthRessource*(ConstanteInGame.propDesignNiveauRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignNiveauRessource.y),
			widthRessource*ConstanteInGame.propDesignNiveauRessource.z,heightRessource*ConstanteInGame.propDesignNiveauRessource.w);
		txtNiveauActuel = createText ("textNiveauActuel", paternRessourceNiveauActuel,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignNiveauRessource.z, heightRessource*ConstanteInGame.propDesignNiveauRessource.w);

		paternRessourceCarburant = createPanel("RessourceCarburant",paternRessource,
			widthRessource*(ConstanteInGame.propDesignCarburantRessource.x-0.5f),heightRessource*(0.5f-ConstanteInGame.propDesignCarburantRessource.y),
			widthRessource*ConstanteInGame.propDesignCarburantRessource.z,heightRessource*ConstanteInGame.propDesignCarburantRessource.w);
		txtCarburant = createText ("textCarburant", paternRessourceCarburant,1, 0, 0,
			.9f*widthRessource*ConstanteInGame.propDesignCarburantRessource.z, heightRessource*ConstanteInGame.propDesignCarburantRessource.w);

		GameObject paternListNiveaux = createPanel("ListNiveaux",goParent,
			width*(ConstanteInGame.propDesignListNiveaux.x-0.5f),height*(0.5f-ConstanteInGame.propDesignListNiveaux.y),
			width*ConstanteInGame.propDesignListNiveaux.z,height*ConstanteInGame.propDesignListNiveaux.w);

		//TODO Transforme en bouton
		GameObject paternBouton = createPanel("BoutonAction",goParent,
			width*(ConstanteInGame.propDesignBouton.x-0.5f),height*(0.5f-ConstanteInGame.propDesignBouton.y),
			width*ConstanteInGame.propDesignBouton.z,height*ConstanteInGame.propDesignBouton.w);

		paternPA = createPanel("PointAttaque",goParent,
			width*(ConstanteInGame.propDesignPointAttaque.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointAttaque.y),
			width*ConstanteInGame.propDesignPointAttaque.z,height*ConstanteInGame.propDesignPointAttaque.w);
		txtPointAttaque = createText ("textPA", paternPA,1, 0, 0,
			.9f*width*ConstanteInGame.propDesignPointAttaque.z,height*ConstanteInGame.propDesignPointAttaque.w);

		GameObject paternPD = createPanel("PointDefense",goParent,
			width*(ConstanteInGame.propDesignPointDefense.x-0.5f),height*(0.5f-ConstanteInGame.propDesignPointDefense.y),
			width*ConstanteInGame.propDesignPointDefense.z,height*ConstanteInGame.propDesignPointDefense.w);
		txtPointDefense = createText ("textPD", paternPD,1, 0, 0,
			.9f*width*ConstanteInGame.propDesignPointDefense.z,height*ConstanteInGame.propDesignPointDefense.w);
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
