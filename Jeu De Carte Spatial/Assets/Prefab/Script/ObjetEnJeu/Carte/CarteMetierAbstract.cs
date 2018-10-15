using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CarteMetierAbstract : MonoBehaviour {

	protected string id;

	public abstract CarteAbstractDTO getCarteRef ();

	public abstract Color getColorCarte ();

	protected GameObject panelGO;

	protected GameObject faceCarteGO;

	protected abstract void initId ();

	//public abstract string initCarte (); //Besoin carte Ref

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
			i.color = getColorCarte ();

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

	public virtual void generateGOCard(){
		faceCarteGO = GameObject.CreatePrimitive (PrimitiveType.Plane);
		faceCarteGO.name = "faceCarte_" + getCarteRef ().idCarte;
		faceCarteGO.transform.SetParent (gameObject.transform);
		faceCarteGO.transform.localScale = Vector3.one;
		faceCarteGO.transform.localPosition = Vector3.zero;
		faceCarteGO.GetComponent<MeshRenderer> ().material.color = getColorCarte ();

		GameObject dosCarte = GameObject.CreatePrimitive (PrimitiveType.Plane);
		dosCarte.name = "dosCarte" + getCarteRef ().idCarte;
		dosCarte.transform.SetParent (gameObject.transform);
		dosCarte.transform.localScale = Vector3.one;
		dosCarte.transform.Rotate( new Vector3(0,0,180));
		dosCarte.transform.localPosition = Vector3.zero;


		GameObject titre = new GameObject("Titre_" + getCarteRef().idCarte);
		titre.transform.SetParent (faceCarteGO.transform);
		titre.transform.localPosition = new Vector3 (-4, 0, 4.75f);
		titre.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		titre.transform.localScale = new Vector3(.5f,.5f,.5f);
		TextMesh txtTitre = titre.AddComponent<TextMesh> ();
		txtTitre.text = getCarteRef ().titreCarte;
		txtTitre.color = Color.black;
		txtTitre.fontSize = 20;
		txtTitre.font = ConstanteInGame.fontArial;
		txtTitre.fontStyle = FontStyle.Bold;

		GameObject image = GameObject.CreatePrimitive (PrimitiveType.Plane);
		image.name = "Image_" + getCarteRef().idCarte;
		image.transform.SetParent (faceCarteGO.transform);
		image.transform.localPosition = new Vector3 (0, 0.01f,2);
		image.transform.localScale = new Vector3(.9f,1,.25f);

		Material matImage = new Material(ConstanteInGame.shaderStandart);
		Sprite sprtImage = getCarteRef ().image;

		if (null == sprtImage) {
			Debug.Log (getCarteRef ().titreCarte + " n'a pas d'image");
			sprtImage = ConstanteInGame.spriteTest;
		}

		matImage.SetTexture ("_MainTex", sprtImage.texture);
		image.GetComponent<Renderer> ().material = matImage;
	}

	public string getId(){
		return id;
	}
}
