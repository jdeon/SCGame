using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConditionCarteUtils {

	public static List<int> listIdCondtionCibleCarte = getListIdCondtionCibleCarte();

	public static List<int> listIdCapacitePourCarte = getListOfCardCapacity();

	/**
	 * Retourne la liste des cartes cible si la cible est sensé être une carte
	 * */
	public static List<CarteMetierAbstract> getMethodeCarteCible (int conditionCible, string conditionAllierEnnemie, List<IConteneurCarte> listEmplacementCible, CarteMetierAbstract carteProvenance,NetworkInstanceId netIdJoueur){
		List<CarteMetierAbstract> listCarteCible = new List<CarteMetierAbstract> ();

		if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_CARTE_AMELIORATION) {
			//TODO listCarteCible = getListCartOfType<CarteAmliorationMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur);
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_PLANETE) {
			listCarteCible.AddRange(getListCartOfType<CartePlaneteMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur));
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT) {
			listCarteCible.AddRange(getListCartOfType<CarteBatimentMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur));
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE) {
			listCarteCible.AddRange(getListCartOfType<CarteDefenseMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur));
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU) {
			listCarteCible.AddRange(getListCartOfType<CarteVaisseauMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur));
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_MODULE) {
			//TODO listCarteCible = getListCartOfType<CarteModuleMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur);
			listCarteCible = new List<CarteMetierAbstract> ();
		} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_DETERIORATION) {
			//TODO listCarteCible = getListCartOfType<CarteDeteriorationMetier>(listEmplacementCible,carteProvenance,conditionAllierEnnemie,netIdJoueur);
			listCarteCible = new List<CarteMetierAbstract> ();
		}

		return listCarteCible;
	}

	private static List<CarteMetierAbstract> getListCartOfType<T>(List<IConteneurCarte> listConteneur, CarteMetierAbstract carteProvenance, string conditionAlegence, NetworkInstanceId idJoueur){
		List<CarteMetierAbstract> listCarteCible = new List<CarteMetierAbstract> ();

		//TODO rajouter condition déjà affecter par la capacite si unique
		if (null != listConteneur && listConteneur.Count > 0) {
			foreach (IConteneurCarte conteneur in listConteneur) {
				if (null != conteneur.getCartesContenu()) {
					foreach (CarteMetierAbstract carteContenu in conteneur.getCartesContenu()) {
						if (carteContenu is T && carteContenu && (
							(conditionAlegence.Contains(ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && carteContenu.JoueurProprietaire.netId == idJoueur)
							|| (conditionAlegence.Contains(ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && carteContenu.JoueurProprietaire.netId != idJoueur)
								|| (conditionAlegence.Contains(ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE) && carteContenu == carteProvenance))) {

							listCarteCible.Add(carteContenu);
						}
					}
				}
			}
		}
		return listCarteCible;
	}

	private static List<int> getListIdCondtionCibleCarte(){
		List<int> listIdCibleResult = new List<int>();
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_PLANETE);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_MODULE);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_PLATEAU);
		listIdCibleResult.Add (ConstanteIdObjet.ID_CONDITION_CIBLE_DETERIORATION);
		return listIdCibleResult;
	}

	private static List<int> getListOfCardCapacity(){
		List<int> listIdCapacityForCard = new List<int> ();
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_LVL);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_XP); //TODO a verifier
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_PV_MAX);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_PV);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_PUISSANCE_AMELIORATION);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_FURTIF);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_ATTAQUE_RETOUR);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_NB_ATTAQUE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_IMMOBILE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_DESARME);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_HORS_DE_CONTROLE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_REVELE_CARTE); //TODO verif utilité
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_VOLE_CARTE); //TODO verif utilité
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_EMPLACEMENT_CARTE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_INVOQUE_CARTE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_DESTRUCTION_CARTE);

		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_INVULNERABLE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ATTAQUE_OPPORTUNITE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_XP);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ETAT_INVISIBLE);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ACTION_HASARD);
		listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_CONDITION);

		return listIdCapacityForCard;
	}


	/*
	if (conditionCible == (ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_CONSTRUCTION)) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_AMELIORATION) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_MAIN) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_CARTE_AMELIORATION) {
		//TODO listCarteCible = getListCartOfType<CarteAmliorationMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_PLANETE) {
		listCarteCible = getListCartOfType<CartePlaneteMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT) {
		listCarteCible = getListCartOfType<CarteBatimentMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE) {
		listCarteCible = getListCartOfType<CarteDefenseMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU) {
		listCarteCible = getListCartOfType<CarteVaisseauMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_MODULE) {
		//TODO listCarteCible = getListCartOfType<CarteModuleMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_SYSTEME) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_PLATEAU) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_DETERIORATION) {
		//TODO listCarteCible = getListCartOfType<CarteDeteriorationMetier>(listEmplacementCible);
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_METAL) {
		//TODO 
	} else if (conditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_CARBURANT) {
		//TODO 
	} else {
		listCarteCible = new List<CarteMetierAbstract> ();
	}*/
}
