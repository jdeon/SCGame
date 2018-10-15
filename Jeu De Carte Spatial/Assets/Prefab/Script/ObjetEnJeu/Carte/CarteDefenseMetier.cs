using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarteDefenseMetier : CarteConstructionMetierAbstract {

	public CarteDefenseDTO carteRef;

	public override CarteAbstractDTO getCarteRef ()
	{
		return carteRef;
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorDefense;
	}

	protected override void initId(){
		if (null == id || id == "") {
			id = "DEF_" + sequenceId;
			sequenceId++;
		}
	}

	public string initCarte (CarteDefenseDTO carteConstructionDTO){
		carteRef = carteConstructionDTO;
		return base.initCarte();
	}

	public void OnMouseDown()
	{
		base.OnMouseDown ();
		designCarte.setPA (carteRef.pointAttaque);
	}

	public override void generateGOCard(){
		base.generateGOCard ();
		GameObject faceCarteGO = transform.Find("faceCarte_" + getCarteRef ().idCarte).gameObject;

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