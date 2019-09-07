using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CapaciteManuelleUtils {

	private delegate void executeTest();

	private static event executeTest executeCapaciteTest;

	public static void useCapacite(CarteConstructionMetierAbstract carteSource, int numLvl, int indexCapaciteAppelee){
		CapaciteMannuelleDTO capaciteAppelee;

		if (carteSource.getCarteRef ().ListNiveau.Count > numLvl
			&& carteSource.getCarteRef ().ListNiveau [numLvl-1].CapaciteManuelle.Count > indexCapaciteAppelee) {
			capaciteAppelee = carteSource.getCarteRef ().ListNiveau [numLvl-1].CapaciteManuelle [indexCapaciteAppelee];
		} else {
			capaciteAppelee = null;
		}

		if (null != capaciteAppelee) {
			bool utilisable = true;
			executeCapaciteTest = null;

			foreach (CapaciteDTO conditionUtilisation in capaciteAppelee.CapaciteCondition) {
				CapaciteDTO capaciteUtilisable = cloneCapacityWhithoutDuRandom (conditionUtilisation);
				if (!testCondition (carteSource, capaciteUtilisable)) {
					utilisable = false;
					break;
				} 
			}
	
			if (utilisable) {
				if (null != executeCapaciteTest) {
					executeCapaciteTest ();
				}

				carteSource.CmdUseCapacityManuelle (numLvl, indexCapaciteAppelee);
			}
		} else {
			Debug.Log ("Capacité manuelle pas retrouvé avec index");
		}
	}

	public static bool testCondition(CarteConstructionMetierAbstract carteSource, CapaciteDTO testCapa){
		bool result;
		if (ConstanteIdObjet.ID_CAPACITE_CONDITION == testCapa.Capacite) {
			result = testPlacementCarte (carteSource, testCapa);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE == testCapa.Capacite) {
			result = testRessourceJoueur (carteSource.JoueurProprietaire, testCapa);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_PV == testCapa.Capacite) {
			result = testValeurSuperieur(carteSource.PV, testCapa.Quantite, testCapa.ModeCalcul);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_POINT_ATTAQUE == testCapa.Capacite) {
			if (carteSource is CarteDefenseMetier) {
				result = testValeurSuperieur(((CarteDefenseMetier)carteSource).getPointAttaque(), testCapa.Quantite, testCapa.ModeCalcul);
			} else if (carteSource is CarteVaisseauMetier) {
				result = testValeurSuperieur(((CarteVaisseauMetier)carteSource).getPointAttaque(), testCapa.Quantite, testCapa.ModeCalcul);
			} else {
				result = false;
			}

		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_LVL == testCapa.Capacite) {
			result = testValeurSuperieur(carteSource.NiveauActuel, testCapa.Quantite, testCapa.ModeCalcul);
		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_CONSOMATION_CARBURANT == testCapa.Capacite) {
			if (carteSource is CarteVaisseauMetier) {
				result = testValeurSuperieur(((CarteVaisseauMetier)carteSource).getConsomationCarburant(), testCapa.Quantite, testCapa.ModeCalcul);
			} else {
				result = false;
			}

		} else if (ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION == testCapa.Capacite) {
			result = testValeurSuperieur(carteSource.getCoutMetalReelCarte(), testCapa.Quantite, testCapa.ModeCalcul);
		} else {
			result = true;
			Debug.Log ("Condition : " + testCapa.Capacite + " non définie pour les capacités manuelle");
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
			if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_METAL.ToString ())) {
				int oldValue = joueurCarteSource.RessourceMetal.Stock;
				if (testValeurSuperieur (oldValue, testCapa.Quantite, testCapa.ModeCalcul)) {
					int newValue = CapaciteUtils.getNewValue (oldValue, testCapa.Quantite, testCapa.ModeCalcul);
					addListnerToDelegate (joueurCarteSource.RessourceMetal, oldValue - newValue);
				} else {
					result = false;
					break;
				}
			} else if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_CARBURANT.ToString ())) {
				int oldValue = joueurCarteSource.RessourceCarburant.Stock;
				if (testValeurSuperieur (oldValue, testCapa.Quantite, testCapa.ModeCalcul)) {
					int newValue = CapaciteUtils.getNewValue (oldValue, testCapa.Quantite, testCapa.ModeCalcul);
					addListnerToDelegate (joueurCarteSource.RessourceCarburant, oldValue - newValue);
				} else {
					result = false;
					break;
				}
			}
		}

		return result;
	}

	private static void addListnerToDelegate(RessourceMetier ressource, int payAmount){
		executeCapaciteTest += delegate {
			payRessource (ressource, payAmount);
		};
	}

	private static void payRessource(RessourceMetier ressource, int payAmount){
		ressource.Stock -= payAmount;
		ressource.updateVisual ();
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
		case ConstanteEnum.TypeCalcul.Chance: 
			resultTest = Random.Range (0, valeurCible + 1) >= valeurOrigine;
			break;
		default :
			resultTest = true;
			Debug.Log ("Type de calcul non définie pour les capacités manuelle");
			break;
		}

		return resultTest;
	}

	private static CapaciteDTO cloneCapacityWhithoutDuRandom(CapaciteDTO capacityBase){
		CapaciteDTO capacityResult = capacityBase.Clone ();

		if (capacityResult.ModeCalcul == ConstanteEnum.TypeCalcul.Des) {
			int newValue = Random.Range (0, capacityResult.Quantite + 1);
			capacityResult.Quantite = newValue;
			capacityResult.ModeCalcul = ConstanteEnum.TypeCalcul.Ajout;
		}

		return capacityResult;
	}

}
