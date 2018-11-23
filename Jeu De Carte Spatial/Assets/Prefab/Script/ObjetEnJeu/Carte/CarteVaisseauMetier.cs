using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract, IAttaquer, IDefendre {

	[SyncVar]
	private bool attaqueCeTours;

	[SyncVar]
	private bool selectionnableDefense;

	[SyncVar]
	private bool defenseSelectionne;

	[SyncVar]
	private bool defenduCeTour;

	protected override void initId(){
		if (null == id || id == "") {
			id = "VAIS_" + sequenceId;
			sequenceId++;
		}
	}

	/***************************Methode IAttaquer*************************/
		
	public void attaqueCarte (CarteConstructionMetierAbstract cible){
		if (cible is IDefendre) {
			((IDefendre)cible).defenseSimultanee (this);
			attaqueCeTours = true;
		} else if (cible is IVulnerable){
			((IVulnerable)cible).recevoirDegat (getPointDegat());
			attaqueCeTours = true;
		}
	}

	public void attaquePlanete (CartePlaneteMetier cible){
		//TODO
		StartCoroutine(choixDefensePlanete(cible.getJoueurProprietaire().netId));
	}

	public bool isCapableAttaquer (){
		bool capableDAttaquer = false;

		if(!attaqueCeTours){
			capableDAttaquer = true;
		//TODO recherche dans capacité
		}

		return capableDAttaquer;
	}
		
	public bool AttaqueCeTour { get { return attaqueCeTours; } }

	private IEnumerator choixDefensePlanete(NetworkInstanceId idJoueurAttaque){
		List<IDefendre> listDefenseurPlanete = new List<IDefendre> ();

		List<EmplacementSolMetier> listEmplacementDefenseEnnemie = EmplacementSolMetier.getEmplacementSolJoueur (idJoueurAttaque);
		foreach(EmplacementSolMetier emplacementDefenseEnnemie in listEmplacementDefenseEnnemie){
			if (null != emplacementDefenseEnnemie && null != emplacementDefenseEnnemie.NetIdCartePosee && NetworkInstanceId.Invalid != emplacementDefenseEnnemie.NetIdCartePosee) {
				GameObject goCarte = NetworkServer.FindLocalObject (emplacementDefenseEnnemie.NetIdCartePosee);

				if (null != goCarte && null != goCarte.GetComponent<IDefendre> ()) {
					IDefendre defenseur = goCarte.GetComponent<IDefendre> ();
					defenseur.SelectionnableDefense = true;
					listDefenseurPlanete.Add (defenseur);
				}
			}
		}

		if (listDefenseurPlanete.Count > 0) {
			float tempsRetant = ConstanteInGame.tempChoixDefense;
			IDefendre defenseurChoisi = null;

			while (tempsRetant > 0 && null != defenseurChoisi) {
				foreach(IDefendre defenseurPlanete in listDefenseurPlanete){
					if (defenseurPlanete.DefenseSelectionne) {
						defenseurChoisi = defenseurPlanete;
					}
				}

				tempsRetant -= .5f;
				yield return new WaitForSeconds (.5f);
			}

			if (null != defenseurChoisi) {
				defenseurChoisi.preDefense (this);
			}

			foreach(IDefendre defenseurPlanete in listDefenseurPlanete){
				defenseurPlanete.reinitDefenseSelect();
			}
		}

		yield return null;
	}



	/*************************Methode IDefendre********************/

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


	public int getConsomationCarburant(){
		int consomationCarburant = carteRef.ConsommationCarburant;

		if( null != listEffetCapacite){
			foreach(CapaciteMetier capaciteCourante in listEffetCapacite){
				if (capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT)) {
					consomationCarburant = capaciteCourante.getNewValue (consomationCarburant);
				}
			}
		}

		return consomationCarburant;
	}

	public int getPointAttaque(){
		int pointAttaqueBase = carteRef.PointAttaque;

		if( null != listEffetCapacite){
			foreach(CapaciteMetier capaciteCourante in listEffetCapacite){
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
