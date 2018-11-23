using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarteDefenseMetier : CarteConstructionMetierAbstract, IDefendre {

	[SyncVar]
	private bool selectionnableDefense;

	[SyncVar]
	private bool defenseSelectionne;

	[SyncVar]
	private bool defenduCeTour;

	protected override void initId(){
		if (null == id || id == "") {
			id = "DEF_" + sequenceId;
			sequenceId++;
		}
	}

	public override void generateVisualCard()
	{
		if (!joueurProprietaire.carteEnVisuel) {
			base.generateVisualCard ();
			joueurProprietaire.carteEnVisuel = true;
			designCarte.setPA (carteRef.PointAttaque);
		}
	}

	public override void OnMouseDown(){
		Joueur joueurLocal = Joueur.getJoueurLocal ();

		//Selection de la defense lors de la phase attaque adverse
		if (SelectionnableDefense && TourJeuSystem.getPhase() == TourJeuSystem.PHASE_ATTAQUE
			&& null != joueurLocal && TourJeuSystem.getPhase(joueurLocal.netId) == TourJeuSystem.EN_ATTENTE) {
			defenseSelectionne = true;
			defenduCeTour = true;
		} else {
			base.OnMouseDown ();
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

	public void preDefense (CarteVaisseauMetier vaisseauAttaquant){
		vaisseauAttaquant.recevoirDegat (getPointDegat ());

		if (vaisseauAttaquant.OnBoard) {
			this.recevoirDegat (vaisseauAttaquant.getPointDegat ());
		}
	}

	public void defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant){
		int degatInfliger = getPointDegat ();
		int degatRecu = vaisseauAttaquant.getPointDegat ();

		vaisseauAttaquant.recevoirDegat (degatInfliger);
		this.recevoirDegat (degatRecu);
	}

	public bool isCapableDefendre (){
		bool result = true;
		//TODO check capacite
		if (defenduCeTour) {
			result = false;
		}
		return result;
	}

	public bool SelectionnableDefense { 
		get{ return selectionnableDefense; }
		set {
			if (value && isCapableDefendre ()) {
				selectionnableDefense = true;
			} else {
				selectionnableDefense = false;
			}
		}
	}

	public bool DefenseSelectionne{
		get{return defenseSelectionne;}
	}

	public void reinitDefenseSelect (){
		selectionnableDefense = false;
		defenseSelectionne = false;
	}

	public void reinitDefenseSelectTour (){
		defenduCeTour = false;
	}

	public int getPointAttaque(){
		int pointAttaqueBase = carteRef.PointAttaque;

		if (null != listEffetCapacite) {
			foreach (CapaciteMetier capaciteCourante in listEffetCapacite) {
				if (capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE)) {
					pointAttaqueBase = capaciteCourante.getNewValue (pointAttaqueBase);
				}
			}
		}

		return pointAttaqueBase;
	}

	public int getPointDegat(){
		int pointDegatBase = getPointAttaque();

		if( null != listEffetCapacite){
			foreach(CapaciteMetier capaciteCourante in listEffetCapacite){
				if (capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE)) {
					pointDegatBase = capaciteCourante.getNewValue (pointDegatBase);
				}
			}
		}

		return pointDegatBase;
	}
}