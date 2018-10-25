using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract {

	protected override void initId(){
		if (null == id || id == "") {
			id = "VAIS_" + sequenceId;
			sequenceId++;
		}
	}
		
	//Affiche la carte si clique dessus
	public virtual void OnMouseDown()
	{
		generateVisualCard ();
	}

	public override void generateVisualCard()
	{
		if (!joueurProprietaire.carteEnVisuel) {
			base.generateVisualCard ();
			joueurProprietaire.carteEnVisuel = true;
			designCarte.setCarburant (carteRef.ConsommationCarburant);
			designCarte.setPA (carteRef.PointAttaque);
		}
	}


	public override void generateGOCard(){
		base.generateGOCard ();
		GameObject faceCarteGO = transform.Find("faceCarte_" + id).gameObject;
		GameObject ressource = faceCarteGO.transform.Find("Ressource_" + id).gameObject;

		GameObject carburant = new GameObject("Carburant_" + id);
		//GameObject carburant = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		carburant.transform.SetParent (ressource.transform);
		carburant.transform.localPosition = new Vector3 (4, 0.01f, 0);
		carburant.transform.localRotation = Quaternion.identity;
		carburant.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		carburant.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtCarburant = carburant.AddComponent<TextMesh> ();
		txtCarburant.text = "C-" + carteRef.ConsommationCarburant;
		txtCarburant.color = Color.black;
		txtCarburant.fontSize = 20;
		txtCarburant.font = ConstanteInGame.fontChintzy;
		txtCarburant.anchor = TextAnchor.MiddleRight;
		carburant.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;


		GameObject cadrePA = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject cadrePA = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		cadrePA.name = "CadrePA_" + id;
		cadrePA.transform.SetParent (faceCarteGO.transform);
		cadrePA.transform.localPosition = new Vector3 (-2.75f, 0.01f,-4.5f);
		cadrePA.transform.localRotation = Quaternion.identity;
		cadrePA.transform.localScale = new Vector3(.25f,1,.05f);
		cadrePA.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject pointAttaque = new GameObject("TxtPA_" +id);
		//GameObject pointAttaque = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		//pointAttaque.name = "TxtPA_" + id;
		pointAttaque.transform.SetParent (cadrePA.transform);
		pointAttaque.transform.localPosition = new Vector3(0,.01f,0);
		pointAttaque.transform.localRotation = Quaternion.identity;
		pointAttaque.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		pointAttaque.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtPA = pointAttaque.AddComponent<TextMesh> ();
		txtPA.text = "Def-" + carteRef.PointAttaque;	//TODO modif pour getAttaque
		txtPA.color = Color.black;
		txtPA.fontSize = 60;
		txtPA.font = ConstanteInGame.fontChintzy;
		txtPA.anchor = TextAnchor.MiddleCenter;
		pointAttaque.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorVaisseau;
	}

}
