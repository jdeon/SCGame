using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract, IAttaquer, IDefendre {

	[SyncVar]
	private bool attaqueEnCours;

	protected override void initId(){
		if (null == id || id == "") {
			id = "VAIS_" + sequenceId;
			sequenceId++;
		}
	}

	public override void OnMouseDown(){

		//Si un joueur clique sur une carte capable d'attaquer puis sur une carte ennemie cela lance une attaque
		if (null != joueurProprietaire.carteSelectionne && joueurProprietaire.carteSelectionne.getJoueurProprietaire () != joueurProprietaire && ! joueurProprietaire.isLocalPlayer 
			&&  joueurProprietaire.carteSelectionne is IAttaquer && !((IAttaquer) joueurProprietaire.carteSelectionne).isCapableAttaquer()) {
			//TODO vérifier aussi l'état cable d'attaquer (capacute en cours, déjà sur une autre attaque)
			((IAttaquer) joueurProprietaire.carteSelectionne).attaque (this);
		} else {
			base.OnMouseDown ();
		}
	}

	public void attaque (CarteConstructionMetierAbstract cible){
		//TODO 
	}

	public bool isCapableAttaquer (){
		bool capableDAttaquer = false;

		if(!isAttaqueEnCours()){
			capableDAttaquer = true;
		//TODO recherche dans capacité
		}

		return capableDAttaquer;
	}
		
	public bool isAttaqueEnCours (){
		return attaqueEnCours;
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
