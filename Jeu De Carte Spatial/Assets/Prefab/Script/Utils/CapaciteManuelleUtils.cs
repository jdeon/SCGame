using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapaciteManuelleUtils {

	public static void useCapacite(CarteConstructionMetierAbstract carteSource, CapaciteMannuelleDTO capaciteAppelee){
		bool utilisable = true;

		foreach (CapaciteDTO conditionUtilisation in capaciteAppelee.CapaciteCondition) {
			if (!testCondition (carteSource, conditionUtilisation)) {
				utilisable = false;
				break;
			}
		}
	
		if (utilisable) {

		}



	}

	public static bool testCondition(CarteConstructionMetierAbstract carteSource, CapaciteDTO testCapa){
		bool result;
		if (ConstanteIdObjet.ID_CAPACITE_CONDITION == testCapa.idCapacite) {
			result = testPlacementCarte (carteSource, testCapa);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE == testCapa.idCapacite) {
			result = testRessourceJoueur (carteSource.JoueurProprietaire, testCapa);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_PV == testCapa.idCapacite) {
			result = testValeurSuperieur(carteSource.PV, testCapa.Quantite, testCapa.ModeCalcul);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE == testCapa.idCapacite) {
			if (carteSource is CarteDefenseMetier) {
				result = testValeurSuperieur(((CarteDefenseMetier)carteSource).getPointAttaque(), testCapa.Quantite, testCapa.ModeCalcul);
			} else if (carteSource is CarteVaisseauMetier) {
				result = testValeurSuperieur(((CarteVaisseauMetier)carteSource).getPointAttaque(), testCapa.Quantite, testCapa.ModeCalcul);
			} else {
				result = false;
			}

		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_LVL == testCapa.idCapacite) {
			result = testValeurSuperieur(carteSource.NiveauActuel, testCapa.Quantite, testCapa.ModeCalcul);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT == testCapa.idCapacite) {
			if (carteSource is CarteVaisseauMetier) {
				result = testValeurSuperieur(((CarteVaisseauMetier)carteSource).getConsomationCarburant(), testCapa.Quantite, testCapa.ModeCalcul);
			} else {
				result = false;
			}

		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION == testCapa.idCapacite) {
			result = testValeurSuperieur(carteSource.getCoutMetalReelCarte(), testCapa.Quantite, testCapa.ModeCalcul);
		} else {
			result = true;
			Debug.Log ("Condition : " + testCapa.idCapacite + " non définie pour les capacités manuelle");
		}

		return result;
	}

	private static bool testPlacementCarte(CarteConstructionMetierAbstract carteSource, CapaciteDTO testCapa){
		List<CarteMetierAbstract> carteCiblesPossible = CapaciteUtils.getCartesCible (carteSource, carteSource, testCapa, carteSource.NetIdJoueurPossesseur);

		return carteCiblesPossible.Contains (carteSource);
	}

	private static bool testRessourceJoueur(Joueur joueurCarteSource, CapaciteDTO testCapa){
		bool result = true;

		foreach (string conditionEmplacement in testCapa.ConditionsEmplacement) {
				if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_METAL.ToString ())
				&& ! testValeurSuperieur(joueurCarteSource.RessourceMetal.Stock, testCapa.Quantite, testCapa.ModeCalcul)) {
					result = false;
					break;
			} else if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_CARBURANT.ToString ())
				&& ! testValeurSuperieur(joueurCarteSource.RessourceCarburant.Stock, testCapa.Quantite, testCapa.ModeCalcul)) {
				result = false;
				break;
				}
			}

		return result;
	}

private static bool testValeurSuperieur(int valeurOrigine, int valeurCible, ConstanteEnum.TypeCalcul typeCalcul){
		bool resultTest;

		switch (typeCalcul) {
		case ConstanteEnum.TypeCalcul.RemiseA:
			resultTest = valeurCible >= valeurOrigine;
			break;
		case ConstanteEnum.TypeCalcul.Ajout: 
			resultTest = (valeurOrigine + valeurCible >= 0);
			break;
		case ConstanteEnum.TypeCalcul.Multiplication:
			resultTest = valeurOrigine * valeurCible >= 1;
			break;
		case ConstanteEnum.TypeCalcul.Division:	
			resultTest = valeurOrigine / valeurCible >= 1;
			break;
		case ConstanteEnum.TypeCalcul.Des:
			resultTest = (int)Random.Range (0, valeurCible + 1) >= valeurOrigine;
			break;
		case ConstanteEnum.TypeCalcul.Chance: 
		//TODO
			resultTest = Random.Range (0, valeurCible + 1) >= valeurOrigine;
			break;
		default :
			resultTest = true;
			Debug.Log ("Type de calcul non définie pour les capacités manuelle");
			break;
		}

		return resultTest;
	}

}
