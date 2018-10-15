using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CarteConstructionMetierAbstract : CarteMetierAbstract {

	//TODO se servir de la valeur pour construire l'id ("CONSTR" + sequenceId++)
	protected static int sequenceId;

	protected int niveauActuel;

	protected int PV;

	/*protected GameObject paternCarteConstruction;

	protected GameObject paternRessourceCarbu;*/

	protected DesignCarteConstructionV2 designCarte;

	protected List<CapaciteMetier> listEffetCapacite;

	protected string initCarte (){
		initId ();
		niveauActuel = 1;
		PV = ((CarteConstructionAbstractDTO) getCarteRef()).pointVieMax;

		return id;
	}

	//Affiche la carte si clique dessus
	public void OnMouseDown()
	{
		base.OnMouseDown ();
		float height = panelGO.GetComponent<RectTransform>().rect.height;
		float width = panelGO.GetComponent<RectTransform>().rect.width;

		CarteConstructionAbstractDTO carteSource = (CarteConstructionAbstractDTO)getCarteRef ();
		int nbNiveau = carteSource.listNiveau.Count;

		//On supprime le premier niveau s'il est vide
		if (nbNiveau > 1 && carteSource.listNiveau [0].titreNiveau == "") {
			nbNiveau--;
		}


		designCarte = new DesignCarteConstructionV2 (panelGO, height, width,nbNiveau);

		designCarte.setTitre (carteSource.titreCarte);
		designCarte.setImage (carteSource.image);
		designCarte.setMetal (carteSource.listNiveau[0].cout);//TODO passer par getCout(qui vérifie s'il y a des capacité malus au bonus vert ou roge)
		designCarte.setNiveauActuel (niveauActuel);
		designCarte.setCarburant (0);
		//designCarte.setDescription ("Ceci est une description de la carte");
		//designCarte.setCitation ("Il était une fois une carte");

		bool premierNivCache = false;
		for( int index = 0 ; index <  carteSource.listNiveau.Count; index++){
			NiveauDTO niveau = carteSource.listNiveau [index];

			//ne rempie pas le premier titre s'il est vide
			if (index == 0 && niveau.titreNiveau == "") {
				premierNivCache = true;
				continue;
			}

			//On affiche le cout uniquement
			int cout = niveauActuel < index +1?  niveau.cout : 0;

			designCarte.setNiveau (premierNivCache? index : index + 1, niveau.titreNiveau, niveau.descriptionNiveau, niveau.cout);
		}

		//TODO calcul PA, PD, ...
		designCarte.setPA (0);
		designCarte.setPD (carteSource.pointVieMax);

		/*paternCarteConstruction = (GameObject) Instantiate(Resources.Load("Graphique/CarteConstructionPatern"));
		paternCarteConstruction.transform.SetParent (panelGO.transform);
		paternCarteConstruction.transform.localPosition = Vector3.zero;
		paternCarteConstruction.GetComponent<RectTransform>().ForceUpdateRectTransforms();

		float height = panelGO.GetComponent<RectTransform>().rect.height;
		float width = panelGO.GetComponent<RectTransform>().rect.width;
		paternCarteConstruction.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		GameObject paternTitre = paternCarteConstruction.transform.Find ("Titre").gameObject;
		GameObject paternImage = paternCarteConstruction.transform.Find ("Image").gameObject;
		GameObject paternRessource = paternCarteConstruction.transform.Find ("Ressource").gameObject;
		GameObject paternRessourceMetal = paternRessource.transform.Find ("Metal").gameObject;
		GameObject paternRessourceNiveau = paternRessource.transform.Find ("NiveauActuel").gameObject;
		paternRessourceCarbu = paternRessource.transform.Find ("Carburant").gameObject;
		GameObject paternDescription = paternCarteConstruction.transform.Find ("Description").gameObject;
		GameObject paternCitation = paternCarteConstruction.transform.Find ("Citation").gameObject;
		GameObject paternNiveaux = paternCarteConstruction.transform.Find ("Niveaux").gameObject;
		GameObject paternUtilise = paternCarteConstruction.transform.Find ("Utilise").gameObject;

		CarteConstructionAbstractDTO carteRef = (CarteConstructionAbstractDTO) getCarteRef ();

		paternTitre.GetComponent<Text>().text = carteRef.titreCarte;
		//paternImage.GetComponent<Image> ().sprite = carteRef.image; //TODO carte Ref doit être un sprite
		paternRessourceMetal.GetComponentInChildren<Text>().text = "" + carteRef.listNiveau[0].cout; //TODO passer par getCout(qui vérifie s'il y a des capacité malus au bonus vert ou roge)
		paternRessourceNiveau.GetComponentInChildren<Text> ().text = "" + this.niveauActuel; //TODO rajouter niveau actuelle dans la carte

		paternRessourceCarbu.SetActive (false);

		paternDescription.GetComponent<Text> ().text = carteRef.libelleCarte;
		paternCitation.GetComponent<Text> ().text = "\"" + carteRef.citationCarte+ "\"";*/
	}

	public  virtual void generateGOCard(){
		base.generateGOCard ();
		CarteConstructionAbstractDTO carteSource = (CarteConstructionAbstractDTO)getCarteRef ();
		GameObject faceCarteGO = transform.Find("faceCarte_" + getCarteRef ().idCarte).gameObject;

		GameObject ressource = GameObject.CreatePrimitive (PrimitiveType.Plane);
		ressource.name = "Ressource_" + getCarteRef().idCarte;
		ressource.transform.SetParent (faceCarteGO.transform);
		ressource.transform.localPosition = new Vector3 (0, 0.01f,-0.5f);
		ressource.transform.localScale = new Vector3(.9f,1,.15f);
		ressource.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject metal = new GameObject("Metal_" + getCarteRef().idCarte);
		metal.transform.SetParent (ressource.transform);
		metal.transform.localPosition = new Vector3 (-4, 0, 0);
		metal.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		metal.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtmetal = metal.AddComponent<TextMesh> ();
		txtmetal.text = "M-" + carteSource.listNiveau[0].cout;
		txtmetal.color = Color.black;
		txtmetal.fontSize = 20;
		txtmetal.font = ConstanteInGame.fontArial;
		txtmetal.anchor = TextAnchor.MiddleLeft;

		GameObject niveau = GameObject.CreatePrimitive (PrimitiveType.Plane);
		niveau.name = "Niveau_" + getCarteRef().idCarte;
		niveau.transform.SetParent (ressource.transform);
		niveau.transform.localPosition = new Vector3 (0, 0.01f, 0);
		niveau.transform.localScale = new Vector3 (.25f, .75f, 1);

		Material matNiveau = new Material(ConstanteInGame.shaderStandart);
		matNiveau.SetTexture ("_MainTex", getSpriteNiveau(niveauActuel).texture);
		niveau.GetComponent<Renderer> ().material = matNiveau;


		GameObject cadreListNiveaux = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadreListNiveaux.name = "CadreListNiv_" + getCarteRef().idCarte;
		cadreListNiveaux.transform.SetParent (faceCarteGO.transform);
		cadreListNiveaux.transform.localPosition = new Vector3 (0, 0.01f, -2.78f);
		cadreListNiveaux.transform.localScale = new Vector3 (0.75f, 1f, 0.25f);
		cadreListNiveaux.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;


		GameObject listNiveaux = new GameObject ("TxtListNiv_" + getCarteRef().idCarte);
		listNiveaux.transform.SetParent (cadreListNiveaux.transform);
		listNiveaux.transform.localPosition = new Vector3 (-4.5f,0, 0);
		listNiveaux.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		listNiveaux.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtNiveaux = listNiveaux.AddComponent<TextMesh> ();
		string textNiv = "";
		for(int index = 0 ; index < carteSource.listNiveau.Count; index++){
				NiveauDTO niveauDTO = carteSource.listNiveau[index];
			if (textNiv != "") {
				textNiv += "\n";
			}

			if (niveauDTO.titreNiveau != "") {
				textNiv += niveauDTO.titreNiveau;
			}
		}

		txtNiveaux.text = textNiv;
		txtNiveaux.anchor = TextAnchor.MiddleLeft;
		txtNiveaux.color = Color.black;
		txtNiveaux.fontSize = 15;
		txtNiveaux.font = ConstanteInGame.fontArial;

		GameObject cadrePD = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadrePD.name = "CadrePD_" + getCarteRef().idCarte;
		cadrePD.transform.SetParent (faceCarteGO.transform);
		cadrePD.transform.localPosition = new Vector3 (2.75f, 0.01f,-4.5f);
		cadrePD.transform.localScale = new Vector3(.25f,1,.05f);
		cadrePD.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject pointDefence = new GameObject("TxtPD_" + getCarteRef().idCarte);
		pointDefence.transform.SetParent (cadrePD.transform);
		pointDefence.transform.localPosition = Vector3.zero;
		pointDefence.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		pointDefence.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtPD = pointDefence.AddComponent<TextMesh> ();
		txtPD.text = "Def-" + carteSource.pointVieMax;	//TODO modif pour PV reelle
		txtPD.color = Color.black;
		txtPD.fontSize = 60;
		txtPD.font = ConstanteInGame.fontArial;
		txtPD.anchor = TextAnchor.MiddleCenter;
	}

	private Sprite getSpriteNiveau(int niveauActuel){
		Sprite result = null;

		switch (niveauActuel) {	
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
}
