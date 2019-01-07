using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementUtils {

	/**
	 * Si idJoueur égal idInvalid alors on regarde chez tous les joueur
	 * */
	public static List<T> getListEmplacementJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementJoueur = new List<T> ();

		T[] listEmplacement = GameObject.FindObjectsOfType<T> ();

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if (idJoueur == NetworkInstanceId.Invalid ||  emplacement.IdJoueurPossesseur == idJoueur) {
					listEmplacementJoueur.Add (emplacement);
				}
			}
		}
		return listEmplacementJoueur;
	}

	/**
	 * Si idJoueur égal idInvalid alors on regarde chez tous les joueur
	 * */
	public static List<T> getListEmplacementLibreJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementLibre = new List<T>();

		T[] listEmplacement = GameObject.FindObjectsOfType<T> ();	

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if ((idJoueur == NetworkInstanceId.Invalid ||  emplacement.IdJoueurPossesseur == idJoueur) && ((EmplacementMetierAbstract)emplacement).transform.childCount == 0) {
					listEmplacementLibre.Add (emplacement);
				}
			}
		}
		return listEmplacementLibre;
	}

	/**
	 * Si idJoueur égal idInvalid alors on regarde chez tous les joueur
	 * */
	public static List<T> getListEmplacementOccuperJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementLibre = new List<T> ();

		T[] listEmplacement = GameObject.FindObjectsOfType<T> ();	

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if ((idJoueur == NetworkInstanceId.Invalid ||  emplacement.IdJoueurPossesseur == idJoueur) && emplacement.transform.childCount > 0
					&& emplacement.gameObject.GetComponentInChildren<CarteMetierAbstract> ()) {
					listEmplacementLibre.Add (emplacement);
				}
			}
		}

		return listEmplacementLibre;
	}

	public static List<T> getListEmplacementLibre <T> (List<EmplacementMetierAbstract> listSource) where T : EmplacementMetierAbstract{
		List<T> listEmplacementLibre = new List<T> ();

		if (null != listSource && listSource.Count > 0) {
			foreach (EmplacementMetierAbstract emplacement in listSource) {
				if (emplacement is T && emplacement.transform.childCount == 0) {
					listEmplacementLibre.Add ((T)emplacement);
				}
			}
		}

		return listEmplacementLibre;
	}

	public static List<T> getListEmplacementOccuper <T> (List<EmplacementMetierAbstract> listSource) where T : EmplacementMetierAbstract{
		List<T> listEmplacementOccuper = new List<T>();

		if (null != listSource && listSource.Count > 0) {
			foreach (EmplacementMetierAbstract emplacement in listSource) {
				if (emplacement is T && emplacement.transform.childCount > 0
					&& emplacement.gameObject.GetComponentInChildren<CarteMetierAbstract> ()) {
					listEmplacementOccuper.Add ((T)emplacement);
				}
			}
		}

		return listEmplacementOccuper;
	}

	public static List<EmplacementMetierAbstract> getRangerSuperieur(EmplacementMetierAbstract emplacementReference,NetworkInstanceId netIdJoueurPlateau, NetworkInstanceId netIdPlateauOppose){
		List<EmplacementMetierAbstract> listEmplacementRetour = new List<EmplacementMetierAbstract> ();

		if (emplacementReference is EmplacementSolMetier && emplacementReference.IdJoueurPossesseur == netIdJoueurPlateau) {
			listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAtomsphereMetier>(netIdJoueurPlateau).ConvertAll(x => (EmplacementMetierAbstract)x));
		} else if (emplacementReference is EmplacementAtomsphereMetier) {
			if (emplacementReference.IdJoueurPossesseur == netIdJoueurPlateau) {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAttaque>(netIdJoueurPlateau).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementSolMetier>(netIdPlateauOppose).ConvertAll(x => (EmplacementMetierAbstract)x));
			}
		} else if (emplacementReference is EmplacementAttaque) {
			if (emplacementReference.IdJoueurPossesseur == netIdJoueurPlateau) {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAttaque>(netIdPlateauOppose).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAtomsphereMetier>(netIdPlateauOppose).ConvertAll(x => (EmplacementMetierAbstract)x));
			}
		}

		return listEmplacementRetour;
	}

	public static List<EmplacementMetierAbstract> getRanger(EmplacementMetierAbstract emplacementReference){
		List<EmplacementMetierAbstract> listEmplacementRetour = new List<EmplacementMetierAbstract> ();

		if (null != emplacementReference) {
			if (emplacementReference is EmplacementSolMetier) {
				listEmplacementRetour.AddRange (getListEmplacementJoueur<EmplacementSolMetier> (emplacementReference.IdJoueurPossesseur).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else if (emplacementReference is EmplacementAtomsphereMetier) {
				listEmplacementRetour.AddRange (getListEmplacementJoueur<EmplacementAtomsphereMetier> (emplacementReference.IdJoueurPossesseur).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else if (emplacementReference is EmplacementAttaque) {
				listEmplacementRetour.AddRange (getListEmplacementJoueur<EmplacementAttaque> (emplacementReference.IdJoueurPossesseur).ConvertAll(x => (EmplacementMetierAbstract)x));
			}
		}
		return listEmplacementRetour;
	}

	public static List<EmplacementMetierAbstract> getRangerInferieur(EmplacementMetierAbstract emplacementReference,NetworkInstanceId netIdJoueurPlateau, NetworkInstanceId netIdPlateauOppose){
		List<EmplacementMetierAbstract> listEmplacementRetour = new List<EmplacementMetierAbstract> ();

		if (emplacementReference is EmplacementSolMetier && emplacementReference.IdJoueurPossesseur != netIdJoueurPlateau) {
			listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAtomsphereMetier>(netIdPlateauOppose).ConvertAll(x => (EmplacementMetierAbstract)x));
		} else if (emplacementReference is EmplacementAtomsphereMetier) {
			if (emplacementReference.IdJoueurPossesseur == netIdJoueurPlateau) {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementSolMetier>(netIdJoueurPlateau).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAttaque>(netIdPlateauOppose).ConvertAll(x => (EmplacementMetierAbstract)x));
			}
		} else if (emplacementReference is EmplacementAttaque) {
			if (emplacementReference.IdJoueurPossesseur == netIdJoueurPlateau) {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAtomsphereMetier>(netIdJoueurPlateau).ConvertAll(x => (EmplacementMetierAbstract)x));
			} else {
				listEmplacementRetour.AddRange(getListEmplacementJoueur<EmplacementAttaque>(netIdJoueurPlateau).ConvertAll(x => (EmplacementMetierAbstract)x));
			}
		}

		return listEmplacementRetour;
	}

	//TODO modifier si plus de 2 joueur
	public static NetworkInstanceId netIdJoueurEnFaceEmplacement (int numColonneEmplacement, NetworkInstanceId netIdJoueur){
		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();
		NetworkInstanceId netIdAdverse = NetworkInstanceId.Invalid;

		foreach (Joueur joueur in listJoueur) {
			if (joueur.netId != netIdJoueur) {
				netIdAdverse = joueur.netId;
				break;
			}
		}

		return netIdAdverse;
	}

	public static List<EmplacementMetierAbstract> getEmplacementByNumColonne(List<EmplacementMetierAbstract> listEmplacement, int numColone){
		List<EmplacementMetierAbstract> listEmplacementRetour = new List<EmplacementMetierAbstract> ();

		foreach (EmplacementMetierAbstract emplacement in listEmplacement) {
			if (emplacement.NumColonne == numColone) {
				listEmplacementRetour.Add (emplacement);
			}
		}

		return listEmplacementRetour;
	}
}
