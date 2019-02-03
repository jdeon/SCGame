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
		if (!joueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();
			joueurProprietaire.CarteEnVisuel = true;
			designCarte.setPA (carteRef.PointAttaque);
		}
	}

	public override void OnMouseDown(){
		Joueur joueurLocal = JoueurUtils.getJoueurLocal ();

		//Selection de la defense lors de la phase attaque adverse
		if (SelectionnableDefense && null != joueurLocal) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			if (systemTour.getPhase () == TourJeuSystem.PHASE_ATTAQUE
				&& systemTour.getPhase (joueurLocal.netId) == TourJeuSystem.EN_ATTENTE) {
			defenseSelectionne = true;
			defenduCeTour = true;
			} else {
			base.OnMouseDown ();
			}
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

	public IEnumerator preDefense (CarteVaisseauMetier vaisseauAttaquant){
		vaisseauAttaquant.recevoirDegat (getPointDegat (), this);

		if (vaisseauAttaquant.OnBoard) {
			this.recevoirDegat (vaisseauAttaquant.getPointDegat (), vaisseauAttaquant);
		}

		yield return null;
	}

	public IEnumerator defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant){
		ActionEventManager.EventActionManager.CmdDefense (joueurProprietaire.netId, this.netId, vaisseauAttaquant.IdISelectionnable);

		bool attaqueEvite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);

		if (!attaqueEvite) {
			
			bool attaqueReoriente = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
			if (attaqueReoriente) {
				List<CarteMetierAbstract> listCartes = CarteUtils.getListCarteCibleReorientation (vaisseauAttaquant, this);
				CarteMetierAbstract cible = listCartes [Random.Range (0, listCartes.Count)];

				if (cible is CarteConstructionMetierAbstract) {
					vaisseauAttaquant.attaqueCarte ((CarteConstructionMetierAbstract)cible, -1);
				} else if (cible is CartePlaneteMetier) {
					vaisseauAttaquant.attaquePlanete ((CartePlaneteMetier)cible, -1);
				}
			} else {
				bool attaquePriorite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ATTAQUE_OPPORTUNITE);
				int degatInfliger = getPointDegat ();
				int degatRecu = vaisseauAttaquant.getPointDegat ();

				if (attaquePriorite) {
					this.recevoirDegat (degatRecu,vaisseauAttaquant);

					if (this.PV > 0) {
						vaisseauAttaquant.recevoirDegat (degatInfliger,this);
					}
				} else {
					vaisseauAttaquant.recevoirDegat (degatInfliger, this);
					this.recevoirDegat (degatRecu, vaisseauAttaquant);
				}
			}
		}

		yield return null;
	}

	public bool isCapableDefendre (){
		IConteneurCarte conteneur = getConteneur ();
		bool result = conteneur is EmplacementSolMetier && 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_DESARME);

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
		return CapaciteUtils.valeurAvecCapacite (carteRef.PointAttaque, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE);   
	}

	public int getPointDegat(){
		return CapaciteUtils.valeurAvecCapacite (getPointAttaque(), listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE);   ;
	}
}