using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarteDefenseMetier : CarteConstructionMetierAbstract {


	protected override void initId(){
		if (null == id || id == "") {
			id = "DEF_" + sequenceId;
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
			designCarte.setPA (carteRef.PointAttaque);
		}
	}


	public override void generateGOCard(){
		base.generateGOCard ();
		GameObject faceCarteGO = transform.Find("faceCarte_" + id).gameObject;

		GameObject cadrePA = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject cadrePA = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		cadrePA.name = "CadrePA_" + id;
		cadrePA.transform.SetParent (faceCarteGO.transform);
		cadrePA.transform.localPosition = new Vector3 (-2.75f, 0.01f,-4.5f);
		cadrePA.transform.localRotation = Quaternion.identity;
		cadrePA.transform.localScale = new Vector3(.25f,1,.05f);
		cadrePA.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject pointAttaque = new GameObject("TxtPA_" + id);
		//GameObject pointAttaque = Instantiate<GameObject>(ConstanteInGame.textPrefab);
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
		return ConstanteInGame.colorDefense;
	}
}