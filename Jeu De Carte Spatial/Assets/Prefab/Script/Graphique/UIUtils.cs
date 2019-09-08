using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour {

	public static readonly string KEY_FOND = "Fond";

	public static readonly string KEY_TITRE = "Titre";

	public static readonly string KEY_IMAGE = "Image";

	public static readonly string KEY_RESSOURCE = "Ressource";

	public static readonly string KEY_NIVEAU = "Niveau";

	public static readonly string KEY_DESCRIPTION = "Description";

	public static readonly string KEY_POINT_DEF_ATT = "PointAttaque/Defense";

	public static Dictionary<string,Dictionary<string,Sprite>> dictionnaryOfCardSprite = initDictionnarySpriteCard();

	private static GameObject goCanvas;

	public static GameObject getCanvas(){
		if(null == goCanvas){
			goCanvas = GameObject.Find("Canvas");
		}

		if (null == goCanvas) {
			goCanvas = new GameObject ("Canvas");
			Canvas canvas = goCanvas.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			goCanvas.AddComponent<CanvasScaler> ();
			goCanvas.AddComponent<GraphicRaycaster> ();
			canvas.pixelPerfect = true;
		}

		return goCanvas;
	}

	public static Vector2 getUICardSize (){
		float height = Screen.height * 3 / 4;
		float width = height / 1.5f > Screen.width ? Screen.width : height / 1.5f;
		return new Vector2 (width, height);
	}

	public static GameObject createPanel (string name, GameObject goParent,  float anchorX, float anchorY, float width, float height){
		GameObject result = createPanel (name, goParent, ConstanteInGame.spriteBackgroundCarte, anchorX, anchorY, width, height);
		result.GetComponent<Image> ().color = new Color (.75f, .75f, .75f, .5f);
		return result;
	}
		
	//Cree un panel
	public static GameObject createPanel (string name, GameObject goParent, Sprite background, float anchorX, float anchorY, float width, float height){
		GameObject panelGO = new GameObject (name);
		panelGO.AddComponent<CanvasRenderer> ();
		panelGO.transform.SetParent (goParent.transform, false);
		panelGO.transform.localPosition = new Vector3(anchorX, anchorY);

		if (null != background) {
			Image i = panelGO.AddComponent<Image> ();
			i.sprite = background;
		} else {
			Image i = panelGO.AddComponent<Image> ();
			i.color = new Color (0f, 0f, 0f, 0f);
		}

		panelGO.GetComponent<RectTransform>().sizeDelta = new Vector2 (width, height);
		panelGO.GetComponent<RectTransform>().ForceUpdateRectTransforms();

		return panelGO;
	}

	//Cree un panel ancrée au centre en haut 
	public static GameObject createPanelAnchorCenterHigh (string name, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject panelGO = new GameObject (name);
		panelGO.AddComponent<CanvasRenderer> ();
		panelGO.transform.SetParent (goParent.transform, false);

		Image i = panelGO.AddComponent<Image> ();
		i.sprite = ConstanteInGame.spriteBackgroundCarte;
		i.color = new Color (.75f, .75f, .75f, .5f);

		RectTransform rectPanel = panelGO.GetComponent<RectTransform> ();
		rectPanel.sizeDelta = new Vector2 (width, height);
		rectPanel.anchorMax = new Vector2 (.5f, 1);
		rectPanel.anchorMin = new Vector2 (.5f, 1);
		rectPanel.localPosition = new Vector3(anchorX, anchorY + (goParent.GetComponent<RectTransform>().rect.height - height)/2);
		rectPanel.ForceUpdateRectTransforms();

		return panelGO;
	}

	public static Image createImage (Sprite image, string nameGO, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject imageGO = new GameObject (nameGO);
		imageGO.AddComponent<CanvasRenderer> ();
		imageGO.transform.SetParent (goParent.transform, false);
		imageGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Image i = imageGO.AddComponent<Image> ();
		i.sprite = image;

		imageGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		return i;
	}

	public static Image createMaskImage (Sprite mask, Sprite image, string nameGO, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject maskGO = new GameObject ("Mask" + nameGO);
		maskGO.AddComponent<CanvasRenderer> ();
		maskGO.transform.SetParent (goParent.transform, false);
		maskGO.transform.localPosition = new Vector3(anchorX, anchorY);

		maskGO.AddComponent<Mask> ();
		Image i = maskGO.AddComponent<Image> ();
		i.sprite = mask;

		maskGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		return createImage(image,nameGO, maskGO, 0, 0, width, height);
	}

	public static Text createText (string nameGO, GameObject goParent, int nbLigneAttendu, float anchorX, float anchorY, float width, float height){
		GameObject textGO = new GameObject (nameGO);
		textGO.AddComponent<CanvasRenderer> ();
		textGO.transform.SetParent (goParent.transform, false);
		textGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Text text = textGO.AddComponent<Text> ();
		text.font = ConstanteInGame.fontChintzy;
		text.color = Color.black;
		text.fontSize = (int)(height * .75f / nbLigneAttendu);

		textGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		return text;
	}

	//cree un text qui s'étend en fonction de la taille de son parent
	public static Text createTextStretch (string nameGO, GameObject goParent, int textTaille, float leftSpace, float highSpace, float rightSpace, float lowSpace){
		GameObject textGO = new GameObject (nameGO);
		textGO.AddComponent<CanvasRenderer> ();
		textGO.transform.SetParent (goParent.transform, false);

		Text text = textGO.AddComponent<Text> ();
		text.font = ConstanteInGame.fontArial;
		text.color = Color.black;
		text.fontSize = textTaille;

		RectTransform rectText = textGO.GetComponent<RectTransform> ();
		rectText.anchorMin = new Vector2 (0, 0);
		rectText.anchorMax = new Vector2 (1, 1);
		rectText.offsetMin = new Vector2 (leftSpace, lowSpace);
		rectText.offsetMax = new Vector2 (-rightSpace, -highSpace);

		return text;
	}

	public static Button createButton (string nameGO, GameObject goParent, float anchorX, float anchorY, float width, float height){
		GameObject buttonGO = new GameObject (nameGO);
		buttonGO.AddComponent<CanvasRenderer> ();
		buttonGO.transform.SetParent (goParent.transform, false);
		buttonGO.transform.localPosition = new Vector3(anchorX, anchorY);

		Button button = buttonGO.AddComponent<Button> ();

		//buttonGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		return button;
	}

	private static Dictionary<string,Dictionary<string,Sprite>> initDictionnarySpriteCard(){
		Dictionary<string,Dictionary<string,Sprite>> resultDictionary = new Dictionary<string, Dictionary<string, Sprite>> ();

		Dictionary<string,Sprite> dictionaryBatiment = new Dictionary<string, Sprite> ();
		dictionaryBatiment.Add (KEY_FOND, ConstanteInGame.spriteBatimentFond);
		dictionaryBatiment.Add (KEY_TITRE, ConstanteInGame.spriteBatimentCardreTitre);
		dictionaryBatiment.Add (KEY_IMAGE, ConstanteInGame.spriteConstructionImage);
		dictionaryBatiment.Add (KEY_RESSOURCE, ConstanteInGame.spriteBatimentCardreRessource);
		dictionaryBatiment.Add (KEY_NIVEAU, ConstanteInGame.spriteBatimentCardreNiveau);
		dictionaryBatiment.Add (KEY_DESCRIPTION, ConstanteInGame.spriteBatimentCardreDescription);
		dictionaryBatiment.Add (KEY_POINT_DEF_ATT, ConstanteInGame.spriteBatimentPointAttDef);
		resultDictionary.Add (ConstanteInGame.strBatiment, dictionaryBatiment);

		Dictionary<string,Sprite> dictionaryDefense = new Dictionary<string, Sprite> ();
		dictionaryDefense.Add (KEY_FOND, ConstanteInGame.spriteDefenseFond);
		dictionaryDefense.Add (KEY_TITRE, ConstanteInGame.spriteDefenseCardreTitre);
		dictionaryDefense.Add (KEY_IMAGE, ConstanteInGame.spriteConstructionImage);
		dictionaryDefense.Add (KEY_RESSOURCE, ConstanteInGame.spriteDefenseCardreRessource);
		dictionaryDefense.Add (KEY_NIVEAU, ConstanteInGame.spriteDefenseCardreNiveau);
		dictionaryDefense.Add (KEY_DESCRIPTION, ConstanteInGame.spriteDefenseCardreDescription);
		dictionaryDefense.Add (KEY_POINT_DEF_ATT, ConstanteInGame.spriteDefensePointAttDef);
		resultDictionary.Add (ConstanteInGame.strDefense, dictionaryDefense);

		Dictionary<string,Sprite> dictionaryVaisseau = new Dictionary<string, Sprite> ();
		dictionaryVaisseau.Add (KEY_FOND, ConstanteInGame.spriteVaisseauFond);
		dictionaryVaisseau.Add (KEY_TITRE, ConstanteInGame.spriteVaisseauCardreTitre);
		dictionaryVaisseau.Add (KEY_IMAGE, ConstanteInGame.spriteConstructionImage);
		dictionaryVaisseau.Add (KEY_RESSOURCE, ConstanteInGame.spriteVaisseauCardreRessource);
		dictionaryVaisseau.Add (KEY_NIVEAU, ConstanteInGame.spriteVaisseauCardreNiveau);
		dictionaryVaisseau.Add (KEY_DESCRIPTION, ConstanteInGame.spriteVaisseauCardreDescription);
		dictionaryVaisseau.Add (KEY_POINT_DEF_ATT, ConstanteInGame.spriteVaisseauPointAttDef);
		resultDictionary.Add (ConstanteInGame.strVaisseau, dictionaryVaisseau);

		return resultDictionary;
	}
}
