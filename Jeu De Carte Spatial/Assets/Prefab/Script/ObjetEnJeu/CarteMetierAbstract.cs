using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CarteMetierAbstract : MonoBehaviour {

	protected string id;

	public abstract CarteAbstractDTO getCarteRef ();

	protected GameObject panelGO;

	//Affiche la carte si clique dessus
	public void OnMouseDown()
	{
		string pseudo = "pseudo"; //TODO rechercher pseudo dans player pref
		GameObject canvasGO = GameObject.Find("Canvas_" + pseudo);
		Text text;

		if (null == canvasGO) {
			canvasGO = new GameObject ("Canvas_" + pseudo);
			Canvas canvas = canvasGO.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler> ();
			canvasGO.AddComponent<GraphicRaycaster> ();
			canvas.pixelPerfect = true;

			panelGO = new GameObject ("Panel_" + pseudo);
			panelGO.AddComponent<CanvasRenderer> ();
			panelGO.transform.SetParent (canvasGO.transform, false);

			Image i = panelGO.AddComponent<Image> ();
			i.sprite = ConstanteInGame.spriteBackgroundCarte;
			i.color = ConstanteInGame.colorVaisseau;

			float height = Screen.height * 3 / 4;
			float width = height / 1.5f > Screen.width ? Screen.width : height / 1.5f;
			panelGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		} else {
			panelGO = canvasGO.transform.Find ("Panel_" + pseudo).gameObject;
			text = canvasGO.GetComponentInChildren<Text>();
		}

		/**
		(GameObject)Instantiate(Resources.Load("CarteConstructionGraphique"))
*/


		//TODO pour l'instant on ne fait apparaitre que le text de la carte
		/*CarteAbstractDTO carteRef = getCarteRef ();
		string strTextCarte = "Titre : " + carteRef.titreCarte;
		strTextCarte += "\nLibelle : " + carteRef.libelleCarte;
		strTextCarte += "\nCitation : \"" + carteRef.citationCarte + "\"";

		text.text = strTextCarte;*/
	}
}
