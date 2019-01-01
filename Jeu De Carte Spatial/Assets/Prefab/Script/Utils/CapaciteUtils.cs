using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CapaciteUtils : MonoBehaviour {

	public static List<int> listIdCapaciteEffetImmediat = getListIdCapaciteEffetImmediat();

	public static int valeurAvecCapacite(int valeurBase, List<CapaciteMetier> listCapaciteCarte, int idTypCapacite){
		int valeurAvecCapacite = valeurBase;
		int valeurIncapacite = 0;

		if( null != listCapaciteCarte){
			foreach(CapaciteMetier capaciteCourante in listCapaciteCarte){
				if(capaciteCourante.IdTypeCapacite ==ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET){
					valeurIncapacite = capaciteCourante.getNewValue (valeurIncapacite);
				} else if (capaciteCourante.IdTypeCapacite == idTypCapacite) {
					valeurAvecCapacite = capaciteCourante.getNewValue (valeurAvecCapacite);
				}
			}
		}

		if(valeurIncapacite > 0){
			//Capacite ignore
			valeurAvecCapacite = valeurBase;
		}

		return valeurAvecCapacite;
	}

	public static bool isCapaciteCall(CapaciteDTO capaciteTest, List<CapaciteMetier> listCapaciteCarteSource, bool isAllier, int idConditionAction){
		bool capable = false;
		string allieOuEnnemi = isAllier ? ConstanteIdObjet.STR_CONDITION_POUR_ALLIER : ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE;

		foreach (string conditionAction in capaciteTest.ConditionsAction) {
			string[] tabConditionAction = conditionAction.Split (char.Parse("-"));
			if (tabConditionAction.Length >= 2 && tabConditionAction [0] == idConditionAction.ToString()
				&& (tabConditionAction [1].Contains (allieOuEnnemi) || tabConditionAction [1].Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE))) {
				int valeurIncapacite = 0;
				if( null != listCapaciteCarteSource){
					foreach(CapaciteMetier capaciteCourante in listCapaciteCarteSource){
						if(capaciteCourante.IdTypeCapacite.Equals (ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET)){
							valeurIncapacite = capaciteCourante.getNewValue (valeurIncapacite);
						} 
					}
				}


				capable = true;
			}
		}	

		return capable;
	}

	public static void callCapacite(CarteMetierAbstract carteSourceCapacite, CarteMetierAbstract carteSourceAction, ISelectionnable cible, CapaciteDTO capacite, NetworkInstanceId netIdJoueur, int actionAppelante){

		int idCapacite = capacite.Capacite; 

		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_ACTION_HASARD) {
			idCapacite = Random.Range (1, 40);
		}


		if (ConditionCarteUtils.listIdCapacitePourCarte.Contains (idCapacite) && (null == cible || cible is CarteMetierAbstract)) {
			List<CarteMetierAbstract> cartesCible = getCartesCible (carteSourceCapacite, (CarteMetierAbstract)cible, capacite, netIdJoueur);
			//TODO use ConstanteIdObjet.ID_CAPACITE_CONDITION

			int nbCibleMax = capacite.NbCible;
			while (nbCibleMax > 0 && cartesCible.Count > 0) {
				CarteMetierAbstract carteCible = cartesCible [Random.Range (0, cartesCible.Count)];

				if (listIdCapaciteEffetImmediat.Contains (idCapacite)) {
					//TODO Effet immediat
				} else {
				
					CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idCapacite, carteSourceCapacite.netId);
					carteCible.addCapacity (capaciteMetier);
				}

				nbCibleMax--;
				cartesCible.Remove (carteCible);

			}
		} else {
			traitementSpeciaux (idCapacite , capacite, carteSourceCapacite, carteSourceCapacite, netIdJoueur, NetworkInstanceId.Invalid /*TODO cible.netIdJoueurCible*/, actionAppelante);

		}
	}

	public static int getNewValue(int oldValue, int quantite, ConstanteEnum.TypeCalcul typeCalcul){

		int newValue;

		switch (typeCalcul) {
		case ConstanteEnum.TypeCalcul.RemiseA:
			newValue = quantite;
			break;
		case ConstanteEnum.TypeCalcul.Ajout: 
			newValue = oldValue + quantite;
			break;
		case ConstanteEnum.TypeCalcul.Multiplication :
			newValue = oldValue * quantite;
			break;
		case ConstanteEnum.TypeCalcul.Division:	
			newValue = oldValue / quantite;
			break;
		case ConstanteEnum.TypeCalcul.Des:
			newValue = (int) Random.Range(0,quantite+1);
			break;
		case ConstanteEnum.TypeCalcul.Chance: 
			newValue = Random.Range(0,quantite+1) >= quantite ? 1 : 0;
			break;
		case ConstanteEnum.TypeCalcul.SuperieurARessource:
			int ressource = 2;//TODO modifier pour bonne value
			newValue = quantite >= ressource ? ressource - quantite : 0;
			break;
		default :
			newValue = oldValue;
			break;
		}

		return newValue;
	}

	public static CapaciteMetier convertCapaciteDTOToMetier(CapaciteDTO capaciteDTO,int idTypeCapacite, NetworkInstanceId netIdCard){
		//TODO prendre en compte module
		return new CapaciteMetier (idTypeCapacite,capaciteDTO.Id,capaciteDTO.ModeCalcul,capaciteDTO.Quantite,netIdCard, capaciteDTO.LierACarte);
	}
		
	public static List<CarteMetierAbstract> getCartesCible (CarteMetierAbstract carteOrigin, CarteMetierAbstract carteCible,CapaciteDTO capacite, NetworkInstanceId netIdJoueur){
		List<CarteMetierAbstract> listCartesCible = new List<CarteMetierAbstract> ();

		List<IConteneurCarte> listEmplacementsCible = new List<IConteneurCarte> ();
		foreach (string conditionEmplacement in capacite.ConditionsEmplacement) {
			string[] tabConditionEmplacement = conditionEmplacement.Split (char.Parse("-"));
			if (tabConditionEmplacement.Length >= 2) {
				int idEmplacement = int.Parse (tabConditionEmplacement [0]);

				listEmplacementsCible.AddRange(ConditionEmplacementUtils.getMethodeEmplacement(idEmplacement, tabConditionEmplacement[1],carteOrigin.getConteneur(), carteCible,netIdJoueur));
			}
		}

		//TODO si capacite a cible pas besoin de faire la suite
		foreach (string conditionCible in capacite.ConditionsCible) {
			string[] tabConditionCible = conditionCible.Split (char.Parse("-"));
			if (tabConditionCible.Length >= 2) {
				int idCible = int.Parse (tabConditionCible [0]);
				List<CarteMetierAbstract> listCartesCibleProbable = ConditionCarteUtils.getMethodeCarteCible(idCible, tabConditionCible[1],listEmplacementsCible,carteOrigin,netIdJoueur);

				if (capacite.AppelUnique) {
					foreach (CarteMetierAbstract carteProbable in listCartesCibleProbable) {
						//On vérifie que la carte ne possède pas déjà l'effet
						if(! carteProbable.containCapacityWithId(capacite.Id)){
							listCartesCible.Add (carteProbable);
						}
					}
				} else {
					listCartesCible.AddRange(listCartesCibleProbable);
				}
			}
		}
			
		return listCartesCible;
	}

	/*
	 * carteCibleCapacite : Carte vers laquelle la capacite va aller
	 * capaciteImmediate : capacite applicable immediatement
	 * carteOrigine : carte d'ou provient la capacite
	 * carteCibleAction : carte cibler lors de l'appel de l'event
	 * */
	private static void traitementCapaciteImmediateCarte(CarteMetierAbstract carteCibleCapacite, CapaciteDTO capaciteImmediate, CarteMetierAbstract carteOrigine, CarteMetierAbstract carteCibleAction, int actionAppelante){
		if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_EMPLACEMENT_CARTE) {
			//TODO
		
		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_VOLE_CARTE) {
			//TODO Definir le conteneur mains, deck, emplacement
			carteCibleCapacite.deplacerCarte (carteOrigine.getJoueurProprietaire ().Main, carteOrigine.getJoueurProprietaire ().netId);

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_DESTRUCTION_CARTE) {
			//TODO que faire si pas IVulnerable?
			if (carteCibleCapacite is IVulnerable) {
				((IVulnerable)carteCibleCapacite).destruction ();
			}

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_INVOQUE_CARTE && null != capaciteImmediate.CarteInvocation) {
			GameObject carteGO = CarteUtils.convertCarteDTOToGameobject (capaciteImmediate.CarteInvocation);

			//TODO ne prends pas en compte l'emplacement cible ou l invocation chez l ennemie
			carteOrigine.getJoueurProprietaire ().invoquerCarte (carteGO, capaciteImmediate.NiveauInvocation, carteOrigine.getJoueurProprietaire ().Main);

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_REVELE_CARTE) {
			//TODO RPC carteCibleCapacite.generateVisualCard ();
		}
	}

	private static void traitementSpeciaux (int idTypeCapacite, CapaciteDTO capacite, CarteMetierAbstract carteSourceCapacite, CarteMetierAbstract carteSourceAction, NetworkInstanceId netIdJoueurAction, NetworkInstanceId netIdJoueurCible, int actionAppelante){
		//TODO utilisation de nbCible
		if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsEmplacement, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);

			CarteMetierAbstract carte;
			//En cas de destruction ou d invocation la carte rechercher pour les ressource est la carte d action
			if (null == carteSourceCapacite || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {
				carte = carteSourceAction;
			} else {
				carte = carteSourceCapacite;
			}

			foreach (RessourceMetier ressource in ressourcesCible) {	
				int productionActuel = ressource.Production;

				int newProd = getQuantiteForRessource(capacite,ressource,carte,false);

				ressource.Production = newProd > 0 ? newProd : 0;
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsEmplacement, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);

			CarteMetierAbstract carte;
			//En cas de destruction ou d invocation la carte rechercher pour les ressource est la carte d action
			if (null == carteSourceCapacite || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {
				carte = carteSourceAction;
			} else {
				carte = carteSourceCapacite;
			}

			foreach (RessourceMetier ressource in ressourcesCible) {
				int stockActuel = ressource.Stock;

				int newStock = getQuantiteForRessource(capacite,ressource,carte,true);

				ressource.Stock = newStock > 0 ? newStock : 0;
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_VOL_RESSOURCE && null != carteSourceCapacite && null != carteSourceCapacite.getJoueurProprietaire()) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsEmplacement, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);

			CarteMetierAbstract carte;
			//En cas de destruction ou d invocation la carte rechercher pour les ressource est la carte d action
			if (null == carteSourceCapacite || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {
				carte = carteSourceAction;
			} else {
				carte = carteSourceCapacite;
			}

			foreach (RessourceMetier ressource in ressourcesCible) {
				if (ressource.NetIdJoueur != carteSourceCapacite.getJoueurProprietaire ().netId) { //on ne peut se voler soit même
					int stockActuel = ressource.Stock;

					int newStock = getQuantiteForRessource(capacite,ressource,carte,true);
					int montantVoler = newStock - stockActuel;

					if (ressource.Stock < montantVoler) {
						montantVoler = ressource.Stock;
					}

					ressource.Stock -= montantVoler;
					int montantReelVole = carteSourceCapacite.getJoueurProprietaire ().addRessource (ressource.TypeRessource, montantVoler);

					if (montantReelVole != montantVoler) {
						ressource.Stock += montantVoler - montantReelVole;
					}
				}
			}
		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_TYPE_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsEmplacement, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);

			CarteMetierAbstract carte;
			//En cas de destruction ou d invocation la carte rechercher pour les ressource est la carte d action
			if (null == carteSourceCapacite || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {
				carte = carteSourceAction;
			} else {
				carte = carteSourceCapacite;
			}

			foreach (RessourceMetier ressource in ressourcesCible) {
				int stockActuel = ressource.Stock;

				int newStock = getQuantiteForRessource(capacite,ressource,carte,true);
				int montantEchange = newStock - stockActuel;

				if (ressource.Stock < montantEchange) {
					montantEchange = ressource.Stock;
					}

				ressource.Stock -= montantEchange;

				string ressourceOppose = "";
				if (ressource.TypeRessource == "Metal") {
					ressourceOppose = "Carburant";
				} else if (ressource.TypeRessource == "Carburant") {
					ressourceOppose = "Metal";
				}

				Joueur joueurRessource = Joueur.getJoueur (ressource.NetIdJoueur);

				int montantReelEchange = joueurRessource.addRessource (ressourceOppose, montantEchange);

				if (montantReelEchange != montantEchange) {
					ressource.Stock += montantEchange - montantReelEchange;
					}
			}
		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_CARTE_PIOCHE) {
			CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idTypeCapacite, carteSourceCapacite.netId);
			List<DeckMetierAbstract> decksCible = getDecksCibles (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);
			foreach (DeckMetierAbstract deck in decksCible) {
				deck.ListCapaciteDeck.Add (capaciteMetier);
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU || idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_DISSIMULER_PLATEAU) {
			Joueur joueurRessource = null;
			List<DeckMetierAbstract> listDeckCible = new List<DeckMetierAbstract>();

			foreach (string conditionCible in capacite.ConditionsCible) {
				HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);
				CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idTypeCapacite, carteSourceCapacite.netId);
				foreach (Joueur joueurCible in joueursCible) {
					joueurCible.addCapacity (capaciteMetier);
				}
			}
				
		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_PLACE_PLATEAU) {
			//TODO en cas de nombre nzgatif desactiver emplacement
			//TODO en cas de nombre positif mettre l'emplacement sur un autre

		}
	}

	private static List<RessourceMetier> getRessourceCible (List<string> listConditionsEmplacement, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSouce){
		List<RessourceMetier> listRessourceCible = new List<RessourceMetier>();

		foreach (string conditionEmplacement in listConditionsEmplacement) {
			HashSet<Joueur> joueursCible = getJoueursCible (conditionEmplacement, netIdJoueurSource, netIdJoueurCible, carteSouce);

			foreach (Joueur joueurRessource in joueursCible) {
				if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_METAL.ToString ())) {
					listRessourceCible.Add (joueurRessource.RessourceMetal);
				} else if (conditionEmplacement.Contains (ConstanteIdObjet.ID_CONDITION_EMPLACEMENT_RESSOURCE_CARBURANT.ToString ())) {
					listRessourceCible.Add (joueurRessource.RessourceCarburant);
				}
			}
		}
		return listRessourceCible;
	}

	private static int getQuantiteForRessource (CapaciteDTO capaciteSoutce, RessourceMetier ressource, CarteMetierAbstract carte, bool isStock){
		int result = isStock ? ressource.Stock : ressource.Production;

		foreach (string conditionCible in capaciteSoutce.ConditionsCible) {
			if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_RESSOURCE.ToString ())) {
				if (isStock) {
					result = getNewValue (ressource.Stock, capaciteSoutce.Quantite, capaciteSoutce.ModeCalcul);
				} else {
					result = getNewValue (ressource.Production, capaciteSoutce.Quantite, capaciteSoutce.ModeCalcul);
				}

			} else if ((conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT.ToString ()) && carte is CarteBatimentMetier) 
				|| (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE.ToString ()) && carte is CarteDefenseMetier)
				||(conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU.ToString ()) && carte is CarteVaisseauMetier)){
				int ajouterAuRessource = 0;
				if (ressource.TypeRessource == "Metal") {
					ajouterAuRessource = getNewValue (((CarteConstructionMetierAbstract) carte).getCoutMetalReelCarte (), capaciteSoutce.Quantite, capaciteSoutce.ModeCalcul);
				} else if (carte is CarteVaisseauMetier && ressource.TypeRessource == "Carburant") {
					ajouterAuRessource = getNewValue (((CarteVaisseauMetier)carte).getConsomationCarburant (), capaciteSoutce.Quantite, capaciteSoutce.ModeCalcul);
				} else if (ressource.TypeRessource == "XP") {
					ajouterAuRessource = getNewValue (((CarteConstructionMetierAbstract)carte).NiveauActuel, capaciteSoutce.Quantite, capaciteSoutce.ModeCalcul);
				}
				//TODO autre cible ?

				result += ajouterAuRessource;
			}
		}

		return result;
	}

	private static List<DeckMetierAbstract> getDecksCibles (List<string> listConditionsCible, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSouce){
		List<DeckMetierAbstract> listDeckCible = new List<DeckMetierAbstract>();

		foreach (string conditionCible in listConditionsCible) {
			HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurSource, netIdJoueurCible, carteSouce);

			foreach (Joueur joueurRessource in joueursCible) {
				if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_CONSTRUCTION.ToString ())) {
					listDeckCible.Add (joueurRessource.DeckConstruction);
				} else if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_AMELIORATION.ToString ())) {
					//TODO ajouter deck amelioration listDeckCible.Add (joueurRessource.RessourceCarburant);
				}
			}
		}

		return listDeckCible;
	}

	private static HashSet<Joueur> getJoueursCible(string conditionCible, NetworkInstanceId netIdJoueurSource,NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSource){
		HashSet<Joueur> joueursCible = new HashSet<Joueur>();

		if (netIdJoueurCible != netIdJoueurSource && conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE)) {
			joueursCible.Add(Joueur.getJoueur (netIdJoueurCible));
		} 
		if (netIdJoueurCible == netIdJoueurSource && conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER)) {
			joueursCible.Add(Joueur.getJoueur (netIdJoueurSource));
		} 

		if (null != carteSource && null != carteSource.getJoueurProprietaire () && conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE)) {
			joueursCible.Add(carteSource.getJoueurProprietaire ());
		}

		return joueursCible;
	}


	private static List<int> getListIdCapaciteEffetImmediat(){
		List<int> listIdCapacityEphemere = new List<int> ();
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_LVL);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_XP);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_PV);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_DEGAT_INFLIGE);
		listIdCapacityEphemere.Add(ConstanteIdObjet.ID_CAPACITE_MODIF_EMPLACEMENT_CARTE);
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_VOLE_CARTE);
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_INVOQUE_CARTE);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_REORIENTE_ATTAQUE);//TODO a debattre
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_DESTRUCTION_CARTE);
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_EVITE_ATTAQUE); //TODO a debattre
		//listIdCapacityForCard.Add (ConstanteIdObjet.ID_CAPACITE_ATTAQUE_OPPORTUNITE);//TODO a debattre
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_REVELE_CARTE);
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_CONDITION);

		//SYSTEM
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_VOL_RESSOURCE);
		listIdCapacityEphemere.Add (ConstanteIdObjet.ID_CAPACITE_MODIF_TYPE_RESSOURCE);
		return listIdCapacityEphemere;
	}

}
/*Capacite non utilise

	public static readonly int ID_CAPACITE_MODIF_XP = 2;
	public static readonly int ID_CAPACITE_MODIF_PUISSANCE_AMELIORATION = 13;
	public static readonly int ID_CAPACITE_ETAT_ATTAQUE_RETOUR = 16;
	public static readonly int ID_CAPACITE_ETAT_HORS_DE_CONTROLE = 21;
	public static readonly int ID_CAPACITE_DISSIMULER_PLATEAU = 25;
	public static readonly int ID_CAPACITE_MODIF_NB_PLACE_PLATEAU = 26;
	public static, readonly int ID_CAPACITE_MODIF_PRODUCTION_XP = 30; //Rejoins ressource
	public static readonly int ID_CAPACITE_ETAT_INVISIBLE = 38;
	public static readonly int ID_CAPACITE_CONDITION = 100;
*/

/** Action non implemente 

public static readonly int ID_CONDITION_ACTION_PIOCHE_AMELIORATION = 2;
public static readonly int ID_CONDITION_ACTION_POSE_AMELIORATION = 4;
public static readonly int ID_CONDITION_ACTION_UTILISE = 9;
public static readonly int ID_CONDITION_ACTION_GAIN_XP = 12;
ID_CONDITION_ACTION_EVOLUTION_CARTE = 16

*/