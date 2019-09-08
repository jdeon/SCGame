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

	public override void reinitDebutTour(){
		base.reinitDebutTour ();
		reinitDefenseSelectTour ();
		AttaqueCeTour = false;
	}

	/***************************Methode IAttaquer*************************/

	public void attaqueCarte (CarteConstructionMetierAbstract cible, NetworkInstanceId netIdEventTask){

		bool attaqueReoriente = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
		bool attaqueEvite = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);

		if (!attaqueEvite) {
			if (attaqueReoriente && netIdEventTask == NetworkInstanceId.Invalid) { //Si netIdEventTask est invalid alors ce n'est pas une action appeler diretement mais une action rappelé par
				List<CarteMetierAbstract> listCartes = CarteUtils.getListCarteCibleReorientation (this, cible);
				CarteMetierAbstract cibleReoriente = listCartes [Random.Range (0, listCartes.Count)];

				if (cibleReoriente is CartePlaneteMetier) {
					attaquePlanete ((CartePlaneteMetier)cibleReoriente, netIdEventTask);
				} else if (cibleReoriente is CarteConstructionMetierAbstract) {
					cible = (CarteConstructionMetierAbstract)cibleReoriente;
				}
			}

			if (cible is IDefendre) {
				JoueurUtils.getJoueurLocal ().CmdCreateTask (cible.netId, JoueurProprietaire.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND, netIdEventTask, false);
				AttaqueCeTour = true;
			} else if (cible is IVulnerable) {
				((IVulnerable)cible).recevoirAttaque (this, netIdEventTask, false);

				AttaqueCeTour = true;
			}
		} else {
			//TODO anim evite
		}

		if (! isCapableAttaquer ()) {
			this.JoueurProprietaire.CarteSelectionne = null;
		}
	}

	public void sacrificeCarte (){
		CartePlaneteMetier cartePlanete = CartePlaneteMetier.getPlaneteEnnemie (idJoueurProprietaire);

		//TODO point de degat ou cout carte?
		cartePlanete.recevoirDegat(getPointDegat (),this, NetworkInstanceId.Invalid);
		JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, cartePlanete.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE, NetworkInstanceId.Invalid, false); //TODO idEventTask provenance?

	}

	public void attaquePlanete (CartePlaneteMetier cible, NetworkInstanceId netIdTaskEvent){
		//TODO
		bool modeFurtif = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_FURTIF); 

		//Mode furtif permet d'attaquer sans déclancher de réponse
		if (modeFurtif) {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (cible.netId, JoueurProprietaire.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netIdTaskEvent, false);
		} else {
			JoueurUtils.getJoueurLocal ().CmdCreateTask (cible.netId, JoueurProprietaire.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND, netIdTaskEvent, false);
		} 
		AttaqueCeTour = true;

		if (! isCapableAttaquer ()) {
			this.JoueurProprietaire.CarteSelectionne = null;
		}
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



	/*************************Methode IDefendre********************/

	public void preDefense (CarteVaisseauMetier vaisseauAttaquant, NetworkInstanceId netIdTaskEvent){
		JoueurUtils.getJoueurLocal ().CmdCreateTask (vaisseauAttaquant.netId, vaisseauAttaquant.idJoueurProprietaire, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netIdTaskEvent, false);

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

				if (attaquePriorite) {
					this.recevoirAttaque(vaisseauAttaquant, netIdTaskEvent, false);

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


	public int getConsomationCarburant(){
		return CapaciteUtils.valeurAvecCapacite (carteRef.ConsommationCarburant, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT);
	}

	public int getPointAttaque(){
		return CapaciteUtils.valeurAvecCapacite (carteRef.PointAttaque, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE);
	}

	public int getPointDegat(){
		return CapaciteUtils.valeurAvecCapacite (getPointAttaque(), listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE);
	}

	public override void generateVisualCard(){
		if (!JoueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();

			designCarte.setCarburant (carteRef.ConsommationCarburant);
			designCarte.setPA (carteRef.PointAttaque);
		}
	}


	public override void generateGOCard(){
		base.generateGOCard ();
		GenerateCardUtils.generateCarburantPartCard (this, id, beanTextCarte);
		GenerateCardUtils.generateAttaquePartCard (this, id, beanTextCarte);
	}

	protected override void updateVisuals (){
		base.updateVisuals ();
		beanTextCarte.txtPointAttaque.text = "Att - " + getPointAttaque();
		beanTextCarte.txtCarburant.text = "C - " + getConsomationCarburant() ;

		designCarte.setPA (getPointAttaque());
		designCarte.setCarburant (getConsomationCarburant());
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorVaisseau;
	}

}
