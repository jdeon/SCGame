using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract, IAttaquer, IDefendre {

	[SyncVar]
	private int nbAttaqueCeTours;

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
		
	public void attaqueCarte (CarteConstructionMetierAbstract cible, bool dejaReoriente){
		//EVENT ATTAQUE

		bool attaqueReoriente = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
		bool attaqueEvite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);

		if (!attaqueEvite) {
			if (attaqueReoriente && !dejaReoriente) {
				List<CarteMetierAbstract> listCartes = CarteUtils.getListCarteCibleReorientation (this, cible);
				CarteMetierAbstract cibleReoriente = listCartes [Random.Range (0, listCartes.Count)];

				if (cibleReoriente is CartePlaneteMetier) {
					attaquePlanete ((CartePlaneteMetier)cibleReoriente);
				} else if (cibleReoriente is CarteConstructionMetierAbstract) {
					cible = (CarteConstructionMetierAbstract)cibleReoriente;
				}
			}

			if (cible is IDefendre) {
				((IDefendre)cible).defenseSimultanee (this);
				AttaqueCeTour = true;
			} else if (cible is IVulnerable) {
				((IVulnerable)cible).recevoirDegat (getPointDegat ());
				AttaqueCeTour = true;
			}
		}
	}

	public void sacrificeCarte (){
		CartePlaneteMetier cartePlanete = CartePlaneteMetier.getPlaneteEnnemie (getJoueurProprietaire ().netId);

		//TODO point de degat ou cout carte?
		cartePlanete.recevoirDegat(getPointDegat ());
		destruction ();
	}

	public IEnumerator attaquePlanete (CartePlaneteMetier cible){
		//TODO
		bool modeFurtif = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_FURTIF); 

		if (!modeFurtif) {
			choixDefensePlanete (cible.getJoueurProprietaire ().netId);
		} 

		cible.recevoirDegat (getPointDegat ());

		return null;
	}

	public bool isCapableAttaquer (){
		bool capableDAttaquer = false;

		if(!AttaqueCeTour){
			//TODO de verification de la position
			capableDAttaquer = 0 >= CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_DESARME);

		}

		return capableDAttaquer;
	}
		
	public bool AttaqueCeTour {
		get { 
			int nbAttaqueAutorise = CapaciteUtils.valeurAvecCapacite (1, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_NB_ATTAQUE);

			return nbAttaqueCeTours >= nbAttaqueAutorise;
		} 
		set {
			if (value) {
				//une attaque effectuer
				nbAttaqueCeTours++;
			} else {
				//ReinitAttaque
				nbAttaqueCeTours = 0;
			}
		}
	}


	private IEnumerator choixDefensePlanete(NetworkInstanceId idJoueurAttaque){
		List<IDefendre> listDefenseurPlanete = new List<IDefendre> ();

		List<EmplacementSolMetier> listEmplacementDefenseEnnemie = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementSolMetier> (idJoueurAttaque);
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
		bool attaqueEvite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);

		if (!attaqueEvite) {

			bool attaqueReoriente = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
			if (attaqueReoriente) {
				List<CarteMetierAbstract> listCartes = CarteUtils.getListCarteCibleReorientation (vaisseauAttaquant, this);
				CarteMetierAbstract cible = listCartes [Random.Range (0, listCartes.Count)];

				if (cible is CarteConstructionMetierAbstract) {
					vaisseauAttaquant.attaqueCarte ((CarteConstructionMetierAbstract)cible, true);
				} else if (cible is CartePlaneteMetier) {
					vaisseauAttaquant.attaquePlanete ((CartePlaneteMetier)cible);
				}

			} else {
				bool attaquePriorite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ATTAQUE_OPPORTUNITE);
				int degatInfliger = getPointDegat ();
				int degatRecu = vaisseauAttaquant.getPointDegat ();

				if (attaquePriorite) {
					int pvDefenseRestante = this.recevoirDegat (degatRecu);

					if (pvDefenseRestante > 0) {
						vaisseauAttaquant.recevoirDegat (degatInfliger);
					}
				} else {
					vaisseauAttaquant.recevoirDegat (degatInfliger);
					this.recevoirDegat (degatRecu);
				}
			}
		}
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


	public int getConsomationCarburant(){
		return CapaciteUtils.valeurAvecCapacite (carteRef.ConsommationCarburant, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT);
	}

	public int getPointAttaque(){
		return CapaciteUtils.valeurAvecCapacite (carteRef.PointAttaque, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE);
	}

	public int getPointDegat(){
		return CapaciteUtils.valeurAvecCapacite (getPointAttaque(), listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE);
	}

	public override void generateVisualCard()
	{
		if (!joueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();
			joueurProprietaire.CarteEnVisuel = true;
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
