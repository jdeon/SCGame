using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCardUtils {

	public static GameObject generateCardBase(CarteMetierAbstract carteBean, string idCarte){
		carteBean.transform.localScale = ConstanteInGame.tailleCarte;

		carteBean.gameObject.AddComponent<BoxCollider> ().size = new Vector3 (10, .025f, 10);

		GameObject faceCarteGO = GameObject.CreatePrimitive (PrimitiveType.Plane);
		faceCarteGO.name = "faceCarte_" + idCarte;
		faceCarteGO.transform.SetParent (carteBean.transform);
		faceCarteGO.transform.localScale = Vector3.one;
		faceCarteGO.transform.localPosition = Vector3.zero;
		faceCarteGO.transform.localRotation = Quaternion.identity;
		faceCarteGO.GetComponent<MeshRenderer> ().material.color = carteBean.getColorCarte ();
		Debug.Log ("Creation de " + faceCarteGO.name);


		GameObject dosCarte = GameObject.CreatePrimitive (PrimitiveType.Plane);
		dosCarte.name = "dosCarte" + idCarte;
		dosCarte.transform.SetParent (carteBean.transform);
		dosCarte.transform.localScale = Vector3.one;
		dosCarte.transform.localRotation = Quaternion.identity;
		dosCarte.transform.Rotate (new Vector3 (0, 0, 180));
		dosCarte.transform.localPosition = Vector3.zero;
		dosCarte.GetComponent<Renderer> ().material = ConstanteInGame.materialCarteBackground;

		GameObject cadreTitre = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadreTitre.name = "CadreTitre_" + idCarte;
		cadreTitre.transform.SetParent (faceCarteGO.transform);
		cadreTitre.transform.localPosition = new Vector3 (0, 0.01f, 3.54f);
		cadreTitre.transform.localRotation = Quaternion.identity;
		cadreTitre.transform.Rotate (new Vector3 (0, 180, 0));
		cadreTitre.transform.localScale = new Vector3 (1, 1, .3f);
		cadreTitre.GetComponent<Renderer> ().material = ConstanteInGame.materialVaisseauCardreTitre;

		GameObject titre = new GameObject ("Titre_" + idCarte);
		titre.transform.SetParent (cadreTitre.transform);
		titre.transform.localPosition = new Vector3 (0, 0.01f, 0);
		titre.transform.localRotation = Quaternion.identity;
		titre.transform.Rotate (new Vector3 (90, 0, 180));		//Le titre apparait face à z
		titre.transform.localScale = new Vector3 (1, 1, 1);

		TextMesh txtTitre = titre.AddComponent<TextMesh> ();
		txtTitre.text = carteBean.getCarteDTORef ().TitreCarte;
		txtTitre.color = Color.black;
		txtTitre.fontSize = 20;
		txtTitre.alignment = TextAlignment.Center;
		txtTitre.anchor = TextAnchor.MiddleCenter;
		txtTitre.font = ConstanteInGame.fontChintzy;
		txtTitre.fontStyle = FontStyle.Bold;
		titre.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;


		GameObject image = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject image = Instantiate<GameObject>( ConstanteInGame.planePrefab);
		image.name = "Image_" + idCarte;
		image.transform.SetParent (faceCarteGO.transform);
		image.transform.localPosition = new Vector3 (0f, 0.005f, 1.18f);
		image.transform.localRotation = Quaternion.identity;
		image.transform.Rotate (ConstanteInGame.rotationImage);
		image.transform.localScale = new Vector3 (1, 1, .4f);

		Material matImage = new Material (ConstanteInGame.materialConstructionImage);

		Sprite sprtImageSecondaire = Resources.Load<Sprite> (carteBean.getCarteDTORef ().ImagePath);


		if (null == sprtImageSecondaire) {
			Debug.Log (carteBean.getCarteDTORef ().TitreCarte + " n'a pas d'image");
			sprtImageSecondaire = ConstanteInGame.spriteTest;
		}

		matImage.SetTexture ("_DetailAlbedoMap", sprtImageSecondaire.texture);
		image.GetComponent<Renderer> ().material = matImage;

		return faceCarteGO;
	}
}
