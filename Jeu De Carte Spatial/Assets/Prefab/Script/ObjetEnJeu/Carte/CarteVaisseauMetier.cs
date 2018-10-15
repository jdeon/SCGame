using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract {

	public CarteVaisseauDTO carteRef;

	public override CarteAbstractDTO getCarteRef ()
	{
		return carteRef;
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorVaisseau;
	}

	protected override void initId(){
		if (null == id || id == "") {
			id = "VAIS_" + sequenceId;
			sequenceId++;
		}
	}

	public string initCarte (CarteVaisseauDTO carteConstructionDTO){
		carteRef = carteConstructionDTO;

		return base.initCarte();
	}

	public void OnMouseDown()
	{
		base.OnMouseDown ();
		designCarte.setCarburant (carteRef.consommationCarburant);
		designCarte.setPA (carteRef.pointAttaque);
	}

	public override void generateGOCard(){
		base.generateGOCard ();
		GameObject faceCarteGO = transform.Find("faceCarte_" + getCarteRef ().idCarte).gameObject;
		GameObject ressource = faceCarteGO.transform.Find("Ressource_" + getCarteRef ().idCarte).gameObject;

		GameObject carburant = new GameObject("Carburant_" + getCarteRef().idCarte);
		carburant.transform.SetParent (ressource.transform);
		carburant.transform.localPosition = new Vector3 (4, 0, 0);
		carburant.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		carburant.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtCarburant = carburant.AddComponent<TextMesh> ();
		txtCarburant.text = "C-" + carteRef.consommationCarburant;
		txtCarburant.color = Color.black;
		txtCarburant.fontSize = 20;
		txtCarburant.font = ConstanteInGame.fontArial;
		txtCarburant.anchor = TextAnchor.MiddleRight;


		GameObject cadrePA = GameObject.CreatePrimitive (PrimitiveType.Plane);
		cadrePA.name = "CadrePA_" + getCarteRef().idCarte;
		cadrePA.transform.SetParent (faceCarteGO.transform);
		cadrePA.transform.localPosition = new Vector3 (-2.75f, 0.01f,-4.5f);
		cadrePA.transform.localScale = new Vector3(.25f,1,.05f);
		cadrePA.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject pointAttaque = new GameObject("TxtPA_" + getCarteRef().idCarte);
		pointAttaque.transform.SetParent (cadrePA.transform);
		pointAttaque.transform.localPosition = Vector3.zero;
		pointAttaque.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		pointAttaque.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtPA = pointAttaque.AddComponent<TextMesh> ();
		txtPA.text = "Def-" + carteRef.pointAttaque;	//TODO modif pour getAttaque
		txtPA.color = Color.black;
		txtPA.fontSize = 60;
		txtPA.font = ConstanteInGame.fontArial;
		txtPA.anchor = TextAnchor.MiddleCenter;
	}

}
