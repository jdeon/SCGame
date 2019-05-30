using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarteUtils {

	public static GameObject convertCarteDTOToGameobject(CarteDTO carteDTO, bool isServer){
		GameObject carteGO;
		string idCarte = "";

		if (carteDTO.TypeOfCarte == ConstanteInGame.strBatiment) {
			carteGO = GameObject.Instantiate(ConstanteInGame.carteBatimentPrefab);
			idCarte = carteGO.GetComponent<CarteBatimentMetier> ().initCarte ((CarteConstructionDTO) carteDTO, isServer);
		} else if (carteDTO.TypeOfCarte == ConstanteInGame.strDefense) {
			carteGO = GameObject.Instantiate(ConstanteInGame.carteDefensePrefab);
			idCarte = carteGO.GetComponent<CarteDefenseMetier> ().initCarte ((CarteConstructionDTO) carteDTO, isServer);
		} else if (carteDTO.TypeOfCarte == ConstanteInGame.strVaisseau) {
			carteGO = GameObject.Instantiate (ConstanteInGame.carteVaisseauPrefab);
			idCarte = carteGO.GetComponent<CarteVaisseauMetier> ().initCarte ((CarteConstructionDTO) carteDTO, isServer);
		} else {
			carteGO = GameObject.Instantiate (ConstanteInGame.emptyPrefab);
		}
		//TODO rajout amelioration et deterioration

		carteGO.name = "Carte_" + idCarte;

		return carteGO;
	}

	public static List<CarteMetierAbstract> getListCarteJoueur(NetworkInstanceId idJoueur){
		List<CarteMetierAbstract> listCarteJoueur = new List<CarteMetierAbstract>();

		CarteMetierAbstract[] listCarte = GameObject.FindObjectsOfType<CarteMetierAbstract> ();

		if (null != listCarte && listCarte.Length > 0) {
			foreach (CarteMetierAbstract carte in listCarte) {
				if (null != carte.getJoueurProprietaire() && carte.getJoueurProprietaire().netId == idJoueur) {
					listCarteJoueur.Add (carte);
				}
			}
		}

		return listCarteJoueur;
	}

	public static List<CarteMetierAbstract> getListCarteCibleReorientation (CarteMetierAbstract carteSource, CarteMetierAbstract carteCible){
		List<CarteMetierAbstract> listCarteCibleReorientation = new List<CarteMetierAbstract> ();

		List<EmplacementAttaque> listEmplacementAttaqueAllier = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementAttaque>(carteSource.getJoueurProprietaire().netId);
		foreach (EmplacementMetierAbstract emplacement in listEmplacementAttaqueAllier) {
			foreach (CarteMetierAbstract carteContenu in emplacement.getCartesContenu()) {
				if (carteContenu is CarteConstructionMetierAbstract && carteContenu != carteSource) {
					listCarteCibleReorientation.Add (carteContenu);
				}
			}
		}

		List<EmplacementSolMetier> listEmplacementSolEnnemie = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementSolMetier>(carteCible.getJoueurProprietaire().netId);
		foreach (EmplacementMetierAbstract emplacement in listEmplacementSolEnnemie) {
			foreach (CarteMetierAbstract carteContenu in emplacement.getCartesContenu()) {
				if (carteContenu is CarteConstructionMetierAbstract && carteContenu != carteSource) {
					listCarteCibleReorientation.Add (carteContenu);
				}
			}
		}


		listCarteCibleReorientation.Add (carteCible.getJoueurProprietaire ().CartePlaneteJoueur);

		if (listCarteCibleReorientation.Contains (carteSource)) {
			listCarteCibleReorientation.Remove (carteSource);
		}

		return listCarteCibleReorientation;
	}

	public static Vector3 getParentScale(Transform tfmChild){
		Vector3 vectorParent = Vector3.one;
		Transform tfmParent = tfmChild.parent;

		while (null != tfmParent) {
			vectorParent.x *= tfmParent.localScale.x;
			vectorParent.y *= tfmParent.localScale.y;
			vectorParent.z *= tfmParent.localScale.z;

			tfmParent = tfmParent.parent;
		}

		return vectorParent;
	}

	public static Vector3 inverseVector (Vector3 vectorParam){
		Vector3 vectorResult = Vector3.zero;

		for (int i = 0; i < 3; i++) {
			if (vectorParam [i] != 0) {
				vectorResult [i] = 1 / vectorParam [i];
			}
		}

		return vectorResult;
	}

	public static List<CarteConstructionMetierAbstract> getListCarteCapableDefendrePlanete(Joueur joueurPlanete){
		List<CarteConstructionMetierAbstract> listeDefenseur = new List<CarteConstructionMetierAbstract> ();

		List<EmplacementSolMetier> listEmplacementSolJoueur = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementSolMetier> (joueurPlanete.netId);

		foreach(EmplacementSolMetier emplacementSolJoueur in listEmplacementSolJoueur){
			List<CarteMetierAbstract> listCarteContenu = emplacementSolJoueur.getCartesContenu ();
			if (null != listCarteContenu && listCarteContenu.Count > 0) {
				CarteMetierAbstract carteContenue = listCarteContenu [0];
				if (null != carteContenue && carteContenue is IDefendre && ((IDefendre)carteContenue).isCapableDefendre () && carteContenue is CarteConstructionMetierAbstract) {
					listeDefenseur.Add ((CarteConstructionMetierAbstract)carteContenue);
				}
			}
		}

		return listeDefenseur;
	}
}
