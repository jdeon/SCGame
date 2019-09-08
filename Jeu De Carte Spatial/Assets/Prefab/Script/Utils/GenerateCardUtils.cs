using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCardUtils {

	private static readonly string KEY_FOND = "Fond";

	private static readonly string KEY_TITRE = "Titre";

	private static readonly string KEY_IMAGE = "Image";

	private static readonly string KEY_METAL = "Metal";

	private static readonly string KEY_NIVEAU = "Niveau";

	private static readonly string KEY_CARBURANT = "Carburant";

	private static readonly string KEY_DESCRIPTION = "Description";

	private static readonly string KEY_POINT_DEF_ATT = "PointAttaque/Defense";

	private static Dictionary<string,Dictionary<string,Material>> dictionnaryOfMaterial = initDictionnaryMaterialCard();

	public static TextesCarteBean generateCardBase(CarteMetierAbstract carteBean, string idCarte){
		TextesCarteBean retour = new TextesCarteBean ();

		string typeCarte = CarteUtils.getTypeCard (carteBean);
		Dictionary<string,Material> dictionaryMaterialForCardType = dictionnaryOfMaterial [typeCarte];

		carteBean.transform.localScale = ConstanteInGame.tailleCarte;

		carteBean.gameObject.AddComponent<BoxCollider> ().size = new Vector3 (10, .025f, 10);

		GameObject faceCarteGO = GameObject.CreatePrimitive (PrimitiveType.Plane);
		faceCarteGO.name = "faceCarte_" + idCarte;
		faceCarteGO.transform.SetParent (carteBean.transform);
		faceCarteGO.transform.localScale = Vector3.one;
		faceCarteGO.transform.localPosition = Vector3.zero;
		faceCarteGO.transform.localRotation = Quaternion.identity;
		faceCarteGO.GetComponent<MeshRenderer> ().material = dictionaryMaterialForCardType [KEY_FOND];
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
		cadreTitre.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_TITRE];

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

		Material matImage = new Material (dictionaryMaterialForCardType [KEY_IMAGE]);

		Sprite sprtImageSecondaire = Resources.Load<Sprite> (carteBean.getCarteDTORef ().ImagePath);


		if (null == sprtImageSecondaire) {
			Debug.Log (carteBean.getCarteDTORef ().TitreCarte + " n'a pas d'image");
			sprtImageSecondaire = ConstanteInGame.spriteTest;
		}

		matImage.EnableKeyword("_DETAIL_MULX2");
		matImage.SetTexture ("_DetailAlbedoMap", sprtImageSecondaire.texture);

		image.GetComponent<Renderer> ().material = matImage;


		retour.goFaceCarte = faceCarteGO;
		retour.goImage = image;
		retour.txtTitre = txtTitre;

		return retour;
	}

	public static void generateConstructionPartCard (CarteConstructionMetierAbstract carteBean, string idCarte, TextesCarteBean beanTextCarte){
		string typeCarte = CarteUtils.getTypeCard (carteBean);
		Dictionary<string,Material> dictionaryMaterialForCardType = dictionnaryOfMaterial [typeCarte];

		CarteConstructionDTO carteRef = carteBean.getCarteRef ();
		GameObject faceCarteGO = beanTextCarte.goFaceCarte;

		GameObject cadreMetal = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadreMetal.name = "CadreMetal_" + idCarte;
		cadreMetal.transform.SetParent (faceCarteGO.transform);
		cadreMetal.transform.localPosition = new Vector3 (-2.39f, 0.005f, -.9f);
		cadreMetal.transform.localRotation = Quaternion.identity;
		cadreMetal.transform.localScale = new Vector3 (.4f, 0.005f, .09f);
		cadreMetal.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_METAL];;
		GameObject metal = new GameObject ("Metal_" + idCarte);
		metal.transform.SetParent (cadreMetal.transform);
		metal.transform.localPosition = new Vector3 (1f, 1f, 1f);
		metal.transform.localRotation = Quaternion.identity;
		metal.transform.Rotate (new Vector3 (90, 0, 0));		//Le titre apparait face à z
		metal.transform.localScale = new Vector3 (1, 3, 1);
		TextMesh txtmetal = metal.AddComponent<TextMesh> ();
		txtmetal.text = "M-" + carteRef.ListNiveau [0].Cout;
		txtmetal.color = Color.black;
		txtmetal.fontSize = 50;
		txtmetal.font = ConstanteInGame.fontChintzy;
		txtmetal.anchor = TextAnchor.MiddleCenter;
		metal.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		GameObject niveau = GameObject.CreatePrimitive (PrimitiveType.Plane);
		niveau.name = "Niveau_" + idCarte;
		niveau.transform.SetParent (faceCarteGO.transform);
		niveau.transform.localPosition = new Vector3 (0, 0.01f, -0.9f);
		niveau.transform.localScale = new Vector3 (.25f, .75f, 0.2f);
		niveau.transform.localRotation = Quaternion.identity;
		niveau.transform.Rotate (new Vector3 (0, 180, 0));

		Material matNiveau = new Material (dictionaryMaterialForCardType [KEY_NIVEAU]);
		matNiveau.EnableKeyword("_DETAIL_MULX2");
		matNiveau.SetTexture ("_DetailAlbedoMap", getSpriteNiveau (carteBean.NiveauActuel).texture);
		niveau.GetComponent<Renderer> ().material = matNiveau;


		GameObject description = GameObject.CreatePrimitive (PrimitiveType.Plane);
		description.name = "descriptionPart_" + idCarte;
		description.transform.SetParent (faceCarteGO.transform);
		description.transform.localPosition = new Vector3 (-0f, .005f, -2.54f);
		description.transform.localScale = new Vector3 (.75f, 1f, 0.25f);
		description.transform.localRotation = Quaternion.identity;
		description.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_DESCRIPTION];

		description.AddComponent<ClickableCardPart> ().setCarteMere (carteBean);
		BoxCollider collCadre = description.AddComponent<BoxCollider> ();
		collCadre.size = new Vector3 (10, .1f, 10);


		GameObject listNiveaux = new GameObject ("TxtListNiv_" + idCarte);
		listNiveaux.transform.SetParent (description.transform);
		listNiveaux.transform.localPosition = new Vector3 (0f, .005f, 0f);
		listNiveaux.transform.localRotation = Quaternion.identity;
		listNiveaux.transform.Rotate (new Vector3 (90, 0, 0));		//Le titre apparait face à z
		listNiveaux.transform.localScale = new Vector3 (.75f, 1f, 1f);
		TextMesh txtDescriptionNiveaux = listNiveaux.AddComponent<TextMesh> ();
		string textNiv = "";
		for (int index = 0; index < carteRef.ListNiveau.Count; index++) {
			NiveauDTO niveauDTO = carteRef.ListNiveau [index];
			if (textNiv != "") {
				textNiv += "\n";
			}

			if (niveauDTO.TitreNiveau != "") {
				textNiv += niveauDTO.TitreNiveau;
			}
		}

		txtDescriptionNiveaux.text = textNiv;
		txtDescriptionNiveaux.alignment = TextAlignment.Center;
		txtDescriptionNiveaux.anchor = TextAnchor.MiddleCenter;
		txtDescriptionNiveaux.color = Color.black;
		txtDescriptionNiveaux.fontSize = 15;
		txtDescriptionNiveaux.font = ConstanteInGame.fontChintzy;
		listNiveaux.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;


		GameObject cadrePD = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadrePD.name = "CadrePD_" + idCarte;
		cadrePD.transform.SetParent (faceCarteGO.transform);
		cadrePD.transform.localPosition = new Vector3 (3.5f, .01f, -4.15f);
		cadrePD.transform.localRotation = Quaternion.identity;
		cadrePD.transform.localScale = new Vector3 (.25f, 1, .1f);
		cadrePD.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_POINT_DEF_ATT];

		GameObject pointDefence = new GameObject ("TxtPD_" + idCarte);
		pointDefence.transform.SetParent (cadrePD.transform);
		pointDefence.transform.localPosition = new Vector3 (0, .01f, 0);
		pointDefence.transform.localRotation = Quaternion.identity;
		pointDefence.transform.Rotate (new Vector3 (90, 0, 0));		//Le titre apparait face à z
		pointDefence.transform.localScale = new Vector3 (1f, 2f, 1f);
		TextMesh txtPD = pointDefence.AddComponent<TextMesh> ();
		txtPD.text = "Def-" + carteRef.PointVieMax;	//TODO modif pour PV reelle
		txtPD.color = Color.black;
		txtPD.fontSize = 60;
		txtPD.font = ConstanteInGame.fontChintzy;
		txtPD.anchor = TextAnchor.MiddleCenter;
		pointDefence.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		beanTextCarte.txtMetal = txtmetal;
		beanTextCarte.goNiveau = niveau;
		beanTextCarte.txtDescription = txtDescriptionNiveaux;
		beanTextCarte.txtPointDefense = txtPD;
	}

	public static void generateCarburantPartCard (CarteConstructionMetierAbstract carteBean, string idCarte, TextesCarteBean beanTextCarte){
		string typeCarte = CarteUtils.getTypeCard (carteBean);
		Dictionary<string,Material> dictionaryMaterialForCardType = dictionnaryOfMaterial [typeCarte];

		CarteConstructionDTO carteRef = carteBean.getCarteRef ();
		GameObject faceCarteGO = beanTextCarte.goFaceCarte;

		GameObject cadreCarburant = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadreCarburant.name = "CadreCarbu_" + idCarte;
		cadreCarburant.transform.SetParent (faceCarteGO.transform);
		cadreCarburant.transform.localPosition = new Vector3 (2.39f, 0.005f, -.9f);
		cadreCarburant.transform.localRotation = Quaternion.identity;
		cadreCarburant.transform.localScale = new Vector3 (.4f, 0.005f, .09f);
		cadreCarburant.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_CARBURANT];
		GameObject carburant = new GameObject ("Carbu_" + idCarte);
		carburant.transform.SetParent (cadreCarburant.transform);
		carburant.transform.localPosition = new Vector3 (1f, 1f, 1f);
		carburant.transform.localRotation = Quaternion.identity;
		carburant.transform.Rotate (new Vector3 (90, 0, 0));		//Le titre apparait face à z
		carburant.transform.localScale = new Vector3 (1, 3, 1);
		TextMesh txtcarburant = carburant.AddComponent<TextMesh> ();
		txtcarburant.text = "C-" + carteRef.ConsommationCarburant;
		txtcarburant.color = Color.black;
		txtcarburant.fontSize = 50;
		txtcarburant.font = ConstanteInGame.fontChintzy;
		txtcarburant.anchor = TextAnchor.MiddleCenter;
		carburant.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		beanTextCarte.txtCarburant = txtcarburant;
	}

	public static void generateAttaquePartCard (CarteConstructionMetierAbstract carteBean, string idCarte, TextesCarteBean beanTextCarte){
		string typeCarte = CarteUtils.getTypeCard (carteBean);
		Dictionary<string,Material> dictionaryMaterialForCardType = dictionnaryOfMaterial [typeCarte];

		CarteConstructionDTO carteRef = carteBean.getCarteRef ();
		GameObject faceCarteGO = beanTextCarte.goFaceCarte;

		GameObject cadrePA = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadrePA.name = "CadrePA_" + idCarte;
		cadrePA.transform.SetParent (faceCarteGO.transform);
		cadrePA.transform.localPosition = new Vector3 (-3.5f, .01f, -4.15f);
		cadrePA.transform.localRotation = Quaternion.identity;
		cadrePA.transform.localScale = new Vector3 (.25f, 1, .1f);
		cadrePA.GetComponent<Renderer> ().material = dictionaryMaterialForCardType [KEY_POINT_DEF_ATT];

		GameObject pointAttaque = new GameObject ("TxtPA_" + idCarte);
		pointAttaque.transform.SetParent (cadrePA.transform);
		pointAttaque.transform.localPosition = new Vector3 (0, .01f, 0);
		pointAttaque.transform.localRotation = Quaternion.identity;
		pointAttaque.transform.Rotate (new Vector3 (90, 0, 0));		//Le titre apparait face à z
		pointAttaque.transform.localScale = new Vector3 (1f, 2f, 1f);
		TextMesh txtPA = pointAttaque.AddComponent<TextMesh> ();
		txtPA.text = "Att-" + carteRef.PointAttaque;
		txtPA.color = Color.black;
		txtPA.fontSize = 60;
		txtPA.font = ConstanteInGame.fontChintzy;
		txtPA.anchor = TextAnchor.MiddleCenter;
		pointAttaque.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		beanTextCarte.txtPointAttaque = txtPA;
	}
		
	public static Sprite getSpriteNiveau(int niveau){
		Sprite result = null;

		switch (niveau) {	
		case 1: 
			result = ConstanteInGame.spriteLvl1;
			break;
		case 2: 
			result = ConstanteInGame.spriteLvl2;
			break;
		case 3: 
			result = ConstanteInGame.spriteLvl3;
			break;
		case 4: 
			result = ConstanteInGame.spriteLvl4;
			break;
		case 5: 
			result = ConstanteInGame.spriteLvl5;
			break;
		}

		return result;
	}

	private static Dictionary<string,Dictionary<string,Material>> initDictionnaryMaterialCard(){
		Dictionary<string,Dictionary<string,Material>> resultDictionary = new Dictionary<string, Dictionary<string, Material>> ();

		Dictionary<string,Material> dictionaryBatiment = new Dictionary<string, Material> ();
		dictionaryBatiment.Add (KEY_FOND, ConstanteInGame.materialBatimentFond);
		dictionaryBatiment.Add (KEY_TITRE, ConstanteInGame.materialBatimentCardreTitre);
		dictionaryBatiment.Add (KEY_IMAGE, ConstanteInGame.materialConstructionImage);
		dictionaryBatiment.Add (KEY_METAL, ConstanteInGame.materialBatimentCardreMetal);
		dictionaryBatiment.Add (KEY_NIVEAU, ConstanteInGame.materialBatimentCardreNiveau);
		dictionaryBatiment.Add (KEY_CARBURANT, ConstanteInGame.materialBatimentCardreCarbrant);
		dictionaryBatiment.Add (KEY_DESCRIPTION, ConstanteInGame.materialBatimentCardreDescription);
		dictionaryBatiment.Add (KEY_POINT_DEF_ATT, ConstanteInGame.materialBatimentPointAttDef);
		resultDictionary.Add (ConstanteInGame.strBatiment, dictionaryBatiment);

		Dictionary<string,Material> dictionaryDefense = new Dictionary<string, Material> ();
		dictionaryDefense.Add (KEY_FOND, ConstanteInGame.materialDefenseFond);
		dictionaryDefense.Add (KEY_TITRE, ConstanteInGame.materialDefenseCardreTitre);
		dictionaryDefense.Add (KEY_IMAGE, ConstanteInGame.materialConstructionImage);
		dictionaryDefense.Add (KEY_METAL, ConstanteInGame.materialDefenseCardreMetal);
		dictionaryDefense.Add (KEY_NIVEAU, ConstanteInGame.materialDefenseCardreNiveau);
		dictionaryDefense.Add (KEY_CARBURANT, ConstanteInGame.materialDefenseCardreCarbrant);
		dictionaryDefense.Add (KEY_DESCRIPTION, ConstanteInGame.materialDefenseCardreDescription);
		dictionaryDefense.Add (KEY_POINT_DEF_ATT, ConstanteInGame.materialDefensePointAttDef);
		resultDictionary.Add (ConstanteInGame.strDefense, dictionaryDefense);

		Dictionary<string,Material> dictionaryVaisseau = new Dictionary<string, Material> ();
		dictionaryVaisseau.Add (KEY_FOND, ConstanteInGame.materialVaisseauFond);
		dictionaryVaisseau.Add (KEY_TITRE, ConstanteInGame.materialVaisseauCardreTitre);
		dictionaryVaisseau.Add (KEY_IMAGE, ConstanteInGame.materialConstructionImage);
		dictionaryVaisseau.Add (KEY_METAL, ConstanteInGame.materialVaisseauCardreMetal);
		dictionaryVaisseau.Add (KEY_NIVEAU, ConstanteInGame.materialVaisseauCardreNiveau);
		dictionaryVaisseau.Add (KEY_CARBURANT, ConstanteInGame.materialVaisseauCardreCarbrant);
		dictionaryVaisseau.Add (KEY_DESCRIPTION, ConstanteInGame.materialVaisseauCardreDescription);
		dictionaryVaisseau.Add (KEY_POINT_DEF_ATT, ConstanteInGame.materialVaisseauPointAttDef);
		resultDictionary.Add (ConstanteInGame.strVaisseau, dictionaryVaisseau);

		return resultDictionary;
	}
}
