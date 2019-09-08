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

	public override void reinitDebutTour(){
		base.reinitDebutTour ();
		reinitDefenseSelectTour ();
	}

	public override void generateVisualCard()
	{
		if (!JoueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();
			designCarte.setPA (carteRef.PointAttaque);
		}
	}

	public override void onClick(){
		Joueur joueurLocal = JoueurUtils.getJoueurLocal ();

		//Selection de la defense lors de la phase attaque adverse
		if (SelectionnableDefense && null != joueurLocal) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			if (systemTour.getPhase () == TourJeuSystem.PHASE_ATTAQUE
				&& systemTour.getPhase (joueurLocal.netId) == TourJeuSystem.EN_ATTENTE) {
			defenseSelectionne = true;
			defenduCeTour = true;
			} else {
				base.onClick ();
			}
		} else {
			base.onClick ();
		}
	}

	public override void generateGOCard(){
		base.generateGOCard ();
		GenerateCardUtils.generateAttaquePartCard (this, id, beanTextCarte);
	}

	protected override void updateVisuals (){
		base.updateVisuals ();
		beanTextCarte.txtPointAttaque.text = "Att - " + getPointAttaque();

		designCarte.setPA (getPointAttaque());
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorDefense;
	}

	public void preDefense (CarteVaisseauMetier vaisseauAttaquant, NetworkInstanceId netIdTaskEvent){
		vaisseauAttaquant.recevoirAttaque (this, netIdTaskEvent, false);

		defenduCeTour = true;

		if (! isCapableDefendre ()) {
			this.JoueurProprietaire.CarteSelectionne = null;
			this.EtatSelectionnable = SelectionnableUtils.ETAT_NON_SELECTION;
		}
	}

	public void defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant, NetworkInstanceId netIdTaskEvent){
		bool attaqueEvite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);

		if (!attaqueEvite) {
			
			bool attaqueReoriente = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
			if (attaqueReoriente) {
				List<CarteMetierAbstract> listCartes = CarteUtils.getListCarteCibleReorientation (vaisseauAttaquant, this);
				CarteMetierAbstract cible = listCartes [Random.Range (0, listCartes.Count)];

				if (cible is CarteConstructionMetierAbstract) {
					vaisseauAttaquant.attaqueCarte ((CarteConstructionMetierAbstract)cible, netIdTaskEvent);
				} else if (cible is CartePlaneteMetier) {
					vaisseauAttaquant.attaquePlanete ((CartePlaneteMetier)cible, netIdTaskEvent);
				}
			} else {
				bool attaquePriorite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ATTAQUE_OPPORTUNITE);
				int degatInfliger = getPointDegat ();
				int degatRecu = vaisseauAttaquant.getPointDegat ();

				if (attaquePriorite) {
					this.recevoirAttaque (vaisseauAttaquant, netIdTaskEvent,false);

					if (this.PV > 0) {
						vaisseauAttaquant.recevoirAttaque (this, netIdTaskEvent, false);
					}
				} else {
					vaisseauAttaquant.recevoirAttaque (this, netIdTaskEvent, true);
					this.recevoirAttaque (vaisseauAttaquant, netIdTaskEvent, true);
				}
			}
		}
	}

	public bool isCapableDefendre (){
		IConteneurCarte conteneur = getConteneur ();
		bool result = conteneur is EmplacementSolMetier && 0 <= CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_DESARME);

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