using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour {

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

	//Cree un panel
	public static GameObject createPanel (string name, GameObject goParent, float anchorX, float anchorY, float width, float height){
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
}
