using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ConditionEmplacementUtils {

	public static List<IConteneurCarte> getMethodeEmplacement (int conditionEmplacement, string conditionAllierEnnemie, IConteneurCarte emplacementOrigin, CarteMetierAbstract carteCible, NetworkInstanceId netIdJoueur){
		List<IConteneurCarte> listEmplacementsCible;

		if (conditionEmplacement == (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_CIBLE)) {
			listEmplacementsCible = getEmplacementsCible (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_ADJACENT_VERTICAL) {
			listEmplacementsCible = getEmplacementsAdjaccentVertical (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_ADJACENT_HORIZONTAL) {
			listEmplacementsCible = getEmplacementsAdjaccentHorizontal (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_ADJACENT_DIAGONAL_HAUT_GAUCHE) {
			listEmplacementsCible = getEmplacementsDiagonalDescendante (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_ADJACENT_DIAGONAL_BAS_GAUCHE) {
			listEmplacementsCible = getEmplacementsDiagonalMontante (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_LIGNE_HORIZONTAL) {
			listEmplacementsCible = getEmplacementsLigneHorizontal (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_LIGNE_VERTICAL) {
			listEmplacementsCible = getEmplacementsLigneVertical (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_LIGNE_ATTAQUANT) {
			listEmplacementsCible = getEmplacementsLigneAttaquant (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_LIGNE_ATMOSPHERE) {
			listEmplacementsCible = getEmplacementsLigneAtmosphere (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_LIGNE_SOL) {
			listEmplacementsCible = getEmplacementsLigneSol (netIdJoueur, conditionAllierEnnemie, emplacementOrigin, carteCible);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_MAIN) {
			listEmplacementsCible = getEmplacementsMain (netIdJoueur, conditionAllierEnnemie, emplacementOrigin);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDTION_EMPLACEMENT_DECK) {
			listEmplacementsCible = getEmplacementsDeck (netIdJoueur, conditionAllierEnnemie, emplacementOrigin);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDTION_EMPLACEMENT_HASARD) {
			listEmplacementsCible = getEmplacementsHasard (netIdJoueur, conditionAllierEnnemie, emplacementOrigin);
		} else if (conditionEmplacement == ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_CARTE_PLANETE) {
			listEmplacementsCible = getCarteEmplacementPlanete (netIdJoueur, conditionAllierEnnemie, emplacementOrigin);
		} else {
			listEmplacementsCible = new List<IConteneurCarte> ();
		}

		return listEmplacementsCible;
	}

	public static List<IConteneurCarte>	getEmplacementsCible (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementCible = new List<IConteneurCarte> ();

		if(conditionAllierEnnemie.Contains(ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE) && null != emplacementOrigin.getCartesContenu()){
			listEmplacementCible.Add (emplacementOrigin);
		}

		IConteneurCarte conteneurCible = carteCible.getConteneur ();
		if(conditionAllierEnnemie.Contains(ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && null != conteneurCible && conteneurCible.isConteneurAllier(netidJoueur)){
			listEmplacementCible.Add (conteneurCible);
		}

		if(conditionAllierEnnemie.Contains(ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && null != conteneurCible && !conteneurCible.isConteneurAllier(netidJoueur)){
			listEmplacementCible.Add (conteneurCible);
		}

		return listEmplacementCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsAdjaccentVertical (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);
			
		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacement = (EmplacementMetierAbstract) conteneurOrigin;
			List<EmplacementMetierAbstract> listEmplacement = new List<EmplacementMetierAbstract> ();

			List<EmplacementMetierAbstract> listEmplacementRangerAdjacente = EmplacementUtils.getRangerSuperieur(emplacement,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacement.NumColonne,netidJoueur));
			listEmplacementRangerAdjacente.AddRange(EmplacementUtils.getRangerInferieur(emplacement,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacement.NumColonne,netidJoueur)));

			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRangerAdjacente, emplacement.NumColonne));

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				if (isProvenance
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && emplacementCible.isConteneurAllier (netidJoueur))
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && !emplacementCible.isConteneurAllier (netidJoueur)))
				{
					listEmplacementCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsAdjaccentHorizontal (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigine = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementMetierAbstract> listEmplacement = new List<EmplacementMetierAbstract> ();

			List<EmplacementMetierAbstract> listEmplacementRanger = EmplacementUtils.getRanger(emplacementOrigine);

			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRanger, emplacementOrigine.NumColonne -1));
			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRanger, emplacementOrigine.NumColonne +1));

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				if (isProvenance
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && emplacementCible.isConteneurAllier (netidJoueur))
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && !emplacementCible.isConteneurAllier (netidJoueur)))
				{
					listEmplacementsCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsDiagonalDescendante (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);
			
		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigin = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementMetierAbstract> listEmplacement = new List<EmplacementMetierAbstract> ();

			List<EmplacementMetierAbstract> listEmplacementRangerSup = EmplacementUtils.getRangerSuperieur(emplacementOrigin,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigin.NumColonne,netidJoueur));
			List<EmplacementMetierAbstract> listEmplacementRangerInf = EmplacementUtils.getRangerInferieur(emplacementOrigin,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigin.NumColonne,netidJoueur));

			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRangerSup, emplacementOrigin.NumColonne -1));
			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRangerInf, emplacementOrigin.NumColonne +1));

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				if (isProvenance
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && emplacementCible.isConteneurAllier (netidJoueur))
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && !emplacementCible.isConteneurAllier (netidJoueur)))
				{
					listEmplacementsCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsDiagonalMontante (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigin = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementMetierAbstract> listEmplacement = new List<EmplacementMetierAbstract> ();

			List<EmplacementMetierAbstract> listEmplacementRangerSup = EmplacementUtils.getRangerSuperieur(emplacementOrigin,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigin.NumColonne,netidJoueur));
			List<EmplacementMetierAbstract> listEmplacementRangerInf = EmplacementUtils.getRangerInferieur(emplacementOrigin,netidJoueur,EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigin.NumColonne,netidJoueur));

			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRangerSup, emplacementOrigin.NumColonne +1));
			listEmplacement.AddRange (EmplacementUtils.getEmplacementByNumColonne (listEmplacementRangerInf, emplacementOrigin.NumColonne -1));

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				if (isProvenance 
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && emplacementCible.isConteneurAllier (netidJoueur))
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && !emplacementCible.isConteneurAllier (netidJoueur)))
				{
					listEmplacementsCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsLigneHorizontal (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigine = (EmplacementMetierAbstract)conteneurOrigin;

			List<EmplacementMetierAbstract> listEmplacementRanger = EmplacementUtils.getRanger(emplacementOrigine);

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacementRanger) {
				if (isProvenance
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && emplacementCible.isConteneurAllier (netidJoueur))
					|| (conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE) && !emplacementCible.isConteneurAllier (netidJoueur)))
				{
					listEmplacementsCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsLigneVertical (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, carteCible.getConteneur (),isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigne = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementMetierAbstract> listEmplacement = new List<EmplacementMetierAbstract>();

			if (isProvenance || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER)) {
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementMetierAbstract>(netidJoueur));
			}

			if (isProvenance || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE)) {
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementMetierAbstract>(EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigne.NumColonne, netidJoueur)));
			}

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				if (emplacementCible.NumColonne == emplacementOrigne.NumColonne)
				{
					listEmplacementsCible.Add (emplacementCible);
				}
			}
		}

		return listEmplacementsCible;
	}


	public static List<IConteneurCarte>	getEmplacementsLigneAttaquant (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurCible;

		if (null != carteCible) {
			conteneurCible = carteCible.getConteneur ();
		} else {
			conteneurCible = null;
		}


		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, conteneurCible,isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigne = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementAttaque> listEmplacement = new List<EmplacementAttaque> ();
			if ((isProvenance && emplacementOrigne.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER)) {
				listEmplacement.AddRange (EmplacementUtils.getListEmplacementJoueur<EmplacementAttaque> (netidJoueur));
			}

			if ((isProvenance && emplacementOrigne.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE)) {
				//TODO voir cas si plusieur ennemie
				listEmplacement.AddRange (EmplacementUtils.getListEmplacementJoueur<EmplacementAttaque> (EmplacementUtils.netIdJoueurEnFaceEmplacement (emplacementOrigne.NumColonne, netidJoueur)));
			}

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				listEmplacementsCible.Add (emplacementCible);
			}
		}

		return listEmplacementsCible;
	}

	public static List<IConteneurCarte>	getEmplacementsLigneAtmosphere (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurCible;

		if (null != carteCible) {
			conteneurCible = carteCible.getConteneur ();
		} else {
			conteneurCible = null;
		}


		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, conteneurCible,isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigne = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementAtomsphereMetier> listEmplacement = new List<EmplacementAtomsphereMetier> ();

			if ((isProvenance && emplacementOrigne.isConteneurAllier(netidJoueur))  || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER)) {
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementAtomsphereMetier>(netidJoueur));
			}

			if ((isProvenance && emplacementOrigne.isConteneurAllier(netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE)) {
				//TODO voir cas si plusieur ennemie
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementAtomsphereMetier>(EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigne.NumColonne, netidJoueur)));
			}

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				listEmplacementsCible.Add (emplacementCible);
			}
		}

		return listEmplacementsCible;
	}

	public static List<IConteneurCarte>	getEmplacementsLigneSol (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin, CarteMetierAbstract carteCible){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);
		IConteneurCarte conteneurCible;

		if (null != carteCible) {
			conteneurCible = carteCible.getConteneur ();
		} else {
			conteneurCible = null;
		}


		IConteneurCarte conteneurOrigin = fillConteneur(emplacementCarteOrigin, conteneurCible,isProvenance);

		if (null != conteneurOrigin && conteneurOrigin is EmplacementMetierAbstract) {
			EmplacementMetierAbstract emplacementOrigne = (EmplacementMetierAbstract)conteneurOrigin;
			List<EmplacementSolMetier> listEmplacement = new List<EmplacementSolMetier> ();

			if ((isProvenance && emplacementOrigne.isConteneurAllier(netidJoueur))  || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER)) {
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementSolMetier>(netidJoueur));
			}

			if ((isProvenance && emplacementOrigne.isConteneurAllier(netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE)) {
				//TODO voir cas si plusieur ennemie
				listEmplacement.AddRange(EmplacementUtils.getListEmplacementJoueur<EmplacementSolMetier>(EmplacementUtils.netIdJoueurEnFaceEmplacement(emplacementOrigne.NumColonne, netidJoueur)));
			}

			foreach (EmplacementMetierAbstract emplacementCible in listEmplacement) {
				listEmplacementsCible.Add (emplacementCible);
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsMain (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);

			Mains[] listMains = GameObject.FindObjectsOfType<Mains> ();

		foreach (Mains mains in listMains) {
			if (mains.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER))) {
				listEmplacementsCible.Add (mains);
			}

			if (!mains.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE))) {
				//TODO voir cas si plusieur ennemie
				listEmplacementsCible.Add (mains);
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getEmplacementsDeck (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);

			DeckMetierAbstract[] listDecks = GameObject.FindObjectsOfType<DeckMetierAbstract> ();

		foreach (DeckMetierAbstract deck in listDecks) {
			if (deck.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER))) {
				listEmplacementsCible.Add (deck);
			}

			if (!deck.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE))) {
				//TODO voir cas si plusieur ennemie
				listEmplacementsCible.Add (deck);
			}
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	//TODO attention si le filtre carte n'est pas bon, pas de sélection doit on crée une liste Hasardeuse
	public static List<IConteneurCarte>	getEmplacementsHasard (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);

		List<IConteneurCarte> listConteneurs = GameObject.FindObjectsOfType<MonoBehaviour> ().OfType<IConteneurCarte>().ToList<IConteneurCarte>();
		int nbIteration = listConteneurs.Count * 2;

		while (nbIteration > 0) {
			IConteneurCarte conteneurHasard = listConteneurs [Random.Range (0, listConteneurs.Count)];

			if (conteneurHasard.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER))) {
				listEmplacementsCible.Add (conteneurHasard);
				break;
			}

			if (!conteneurHasard.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE))) {
				//TODO voir cas si plusieur ennemie
				listEmplacementsCible.Add (conteneurHasard);
				break;
			}

			nbIteration--;
		}

		return listEmplacementsCible;
	}

	//TODO revoir la definition de provenance pour la methode
	public static List<IConteneurCarte>	getCarteEmplacementPlanete (NetworkInstanceId netidJoueur, string conditionAllierEnnemie, IConteneurCarte emplacementCarteOrigin){
		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		bool isProvenance = conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE);

		CartePlaneteMetier[] listPlanetes = GameObject.FindObjectsOfType<CartePlaneteMetier> ();

		foreach (CartePlaneteMetier planete in listPlanetes) {
			if (planete.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER))) {
				listEmplacementsCible.Add (planete);
			}

			if (!planete.isConteneurAllier (netidJoueur) && ((isProvenance && null != emplacementCarteOrigin && emplacementCarteOrigin.isConteneurAllier (netidJoueur)) || conditionAllierEnnemie.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE))) {
				//TODO voir cas si plusieur ennemie
				listEmplacementsCible.Add (planete);
			}
		}

		return listEmplacementsCible;
	}







	private static IConteneurCarte fillConteneur(IConteneurCarte conteneurCarteOrigine, IConteneurCarte conteurCarteCible, bool provenanceCarte){
		IConteneurCarte conteneurOrigin;

		if (provenanceCarte) {
			conteneurOrigin = conteneurCarteOrigine;
		} else {
			conteneurOrigin = conteurCarteCible;
		}

		return conteneurOrigin;
	}
}
