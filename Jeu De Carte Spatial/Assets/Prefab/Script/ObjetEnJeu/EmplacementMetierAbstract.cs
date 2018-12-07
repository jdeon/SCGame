using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementMetierAbstract : NetworkBehaviour {

	protected int numColone;

	[SyncVar]
	protected NetworkInstanceId idJoueurPossesseur;

	[SyncVar]
	protected NetworkInstanceId idCarte;

	public void Start(){
		numColone = transform.GetSiblingIndex () + 1;
	}

	public static List<T> getListEmplacementJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementJoueur = new List<T> ();

		T[] listEmplacement = GameObject.FindObjectsOfType<T> ();

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if (emplacement.idJoueurPossesseur == idJoueur) {
					listEmplacementJoueur.Add (emplacement);
				}
			}
		}
		return listEmplacementJoueur;
	}

	public static List<T> getListEmplacementLibreJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementLibre = new List<T>();

			T[] listEmplacement = GameObject.FindObjectsOfType<T> ();	

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if (emplacement.idJoueurPossesseur == idJoueur && ((EmplacementMetierAbstract)emplacement).transform.childCount == 0) {
					listEmplacementLibre.Add (emplacement);
				}
			}
		}
		return listEmplacementLibre;
	}

	public static List<T> getListEmplacementOccuperJoueur <T> (NetworkInstanceId idJoueur) where T : EmplacementMetierAbstract{
		List<T> listEmplacementLibre = new List<T> ();

		T[] listEmplacement = GameObject.FindObjectsOfType<T> ();	

		if (null != listEmplacement && listEmplacement.Length > 0) {
			foreach (T emplacement in listEmplacement) {
				if (emplacement.idJoueurPossesseur == idJoueur && emplacement.transform.childCount > 0
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

	public static List<T> getListEmplacementOccuperJoueur <T> (List<EmplacementMetierAbstract> listSource) where T : EmplacementMetierAbstract{
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


	public void putCard(CarteConstructionMetierAbstract cartePoser){
		Transform trfmCard = cartePoser.transform;

		cartePoser.OnBoard = true;

		trfmCard.SetParent (transform);
		trfmCard.localPosition = new Vector3 (0, .01f, 0);
		trfmCard.localRotation = Quaternion.identity;
		trfmCard.localScale = Vector3.one;

		cartePoser.getJoueurProprietaire ().carteSelectionne = null;
	}

	public bool isCardCostPayable(CartePlaneteMetier cartePlanetJoueur, CarteMetierAbstract carteSelectionne){
		bool costPayable = false;

		if (null != cartePlanetJoueur && carteSelectionne is CarteConstructionMetierAbstract && cartePlanetJoueur.isMetalSuffisant (((CarteConstructionMetierAbstract)carteSelectionne).getCoutMetal ())) {
			costPayable = true;
		}

		return costPayable;
	}

	/**
	 * Retourne si la carte est déplaçable par le joueur
	 * */
	protected bool isMovableByPlayer(Joueur joueur){
		bool movable = false;

		if (null != joueur && joueur.isLocalPlayer) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			if (systemTour.getPhase (joueur.netId) == TourJeuSystem.PHASE_DEPLACEMENT
			   && null != joueur.carteSelectionne && joueur.netId == joueur.carteSelectionne.getJoueurProprietaire ().netId) {
				movable = true;
			}
		}
			
		return movable;
	}


	public void setIdJoueurPossesseur(NetworkInstanceId idJoueurPossesseur){
		this.idJoueurPossesseur = idJoueurPossesseur;
	}

	public NetworkInstanceId NetIdCartePosee {
		get{return idCarte;}
	}

	public int NumColonne {
		get{ return numColone; }
	}
}
