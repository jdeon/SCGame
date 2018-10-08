using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CarteConstructionMetierAbstract : CarteMetierAbstract {

	//TODO se servir de la valeur pour construire l'id ("CONSTR" + sequenceId++)
	private static int sequenceId;

	public int niveauActuel = 1;

	protected GameObject paternCarteConstruction;

	protected GameObject paternRessourceCarbu;

	protected List<CapaciteMetier> listEffetCapacite;

	//Affiche la carte si clique dessus
	public void OnMouseDown()
	{
		base.OnMouseDown ();
		float height = panelGO.GetComponent<RectTransform>().rect.height;
		float width = panelGO.GetComponent<RectTransform>().rect.width;
		DesignCarteConstructionV2 designCarte = new DesignCarteConstructionV2 (panelGO, height, width);
		CarteConstructionAbstractDTO carteSource = (CarteConstructionAbstractDTO)getCarteRef ();

		designCarte.setTitre (carteSource.titreCarte);
		designCarte.setImage (carteSource.image);
		designCarte.setMetal (carteSource.listNiveau[0].cout);//TODO passer par getCout(qui vérifie s'il y a des capacité malus au bonus vert ou roge)
		designCarte.setNiveauActuel (niveauActuel);
		designCarte.setCarburant (5);
		//designCarte.setDescription ("Ceci est une description de la carte");
		//designCarte.setCitation ("Il était une fois une carte");
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
}
