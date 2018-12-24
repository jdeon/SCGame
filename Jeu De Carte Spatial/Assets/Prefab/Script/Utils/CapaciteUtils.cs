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
				if(capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET)){
					valeurIncapacite = capaciteCourante.getNewValue (valeurIncapacite);
				} else if (capaciteCourante.getIdTypeCapacite ().Equals (idTypCapacite)) {
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
						if(capaciteCourante.getIdTypeCapacite ().Equals (ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET)){
							valeurIncapacite = capaciteCourante.getNewValue (valeurIncapacite);
						} 
					}
				}


				capable = true;
			}
		}	

		return capable;
	}

	public static void callCapacite(CarteMetierAbstract carteSource, ISelectionnable cible, CapaciteDTO capacite, NetworkInstanceId netIdJoueur, int actionAppelante, int nbCibleMax){

		int idCapacite = capacite.Capacite; 

		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_ACTION_HASARD) {
			idCapacite = Random.Range (1, 40);
		}


		if (ConditionCarteUtils.listIdCapacitePourCarte.Contains (idCapacite) && (null == cible || cible is CarteMetierAbstract)) {
			List<CarteMetierAbstract> cartesCible = getCartesCible (carteSource, (CarteMetierAbstract)cible, capacite, netIdJoueur);
			//TODO use ConstanteIdObjet.ID_CAPACITE_CONDITION
			while (nbCibleMax > 0 && cartesCible.Count > 0) {
				CarteMetierAbstract carteCible = cartesCible [Random.Range (0, cartesCible.Count)];

				if (listIdCapaciteEffetImmediat.Contains (idCapacite)) {
					//TODO Effet immediat
				} else {
				
					CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idCapacite, carteSource.netId);
					carteCible.addCapacity (capaciteMetier);
				}

				nbCibleMax--;
				cartesCible.Remove (carteCible);

			}
		} else {
			traitementSpeciaux (idCapacite , capacite, carteSource, netIdJoueur, NetworkInstanceId.Invalid /*TODO cible.netIdJoueurCible*/);

		}
	}

	public static int getNewValue(int oldValue, CapaciteDTO capacite){

		int newValue;

		switch (capacite.ModeCalcul) {
		case ConstanteEnum.TypeCalcul.RemiseA:
			newValue = capacite.Quantite;
			break;
		case ConstanteEnum.TypeCalcul.Ajout: 
			newValue = oldValue + capacite.Quantite;
			break;
		case ConstanteEnum.TypeCalcul.Multiplication :
			newValue = oldValue * capacite.Quantite;
			break;
		case ConstanteEnum.TypeCalcul.Division:	
			newValue = oldValue / capacite.Quantite;
			break;
		case ConstanteEnum.TypeCalcul.Des:
			newValue = (int) Random.Range(0,capacite.Quantite+1);
			break;
		case ConstanteEnum.TypeCalcul.Chance: 
			newValue = Random.Range(0,capacite.Quantite+1) >= capacite.Quantite ? 1 : 0;
			break;
		case ConstanteEnum.TypeCalcul.SuperieurARessource:
			int ressource = 2;//TODO modifier pour bonne value
			newValue = capacite.Quantite >= ressource ? ressource - capacite.Quantite : 0;
			break;
		default :
			newValue = oldValue;
			break;
		}

		return newValue;
	}

	public static CapaciteMetier convertCapaciteDTOToMetier(CapaciteDTO capaciteDTO,int idTypeCapacite, NetworkInstanceId netIdCard){
		//TODO prendre en compte module
		return new CapaciteMetier (idTypeCapacite,capaciteDTO.ModeCalcul,capaciteDTO.Quantite,netIdCard, capaciteDTO.LierACarte);
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

				listCartesCible.AddRange(ConditionCarteUtils.getMethodeCarteCible(idCible, tabConditionCible[1],listEmplacementsCible,carteOrigin,netIdJoueur));
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

	private static void traitementSpeciaux (int idTypeCapacite, CapaciteDTO capacite, CarteMetierAbstract carteSource, NetworkInstanceId netIdJoueurAction, NetworkInstanceId netIdJoueurCible){

		if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSource);

			foreach (RessourceMetier ressource in ressourcesCible) {
				int productionActuel = ressource.Production;
				int gain = getNewValue (productionActuel, capacite);

				if (-gain > productionActuel) {	//Cas ou les gain son negatif et superieur à la produciton
					gain = -productionActuel;
				}

				ressource.Production = gain;
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSource);

			foreach (RessourceMetier ressource in ressourcesCible) {
				int stockActuel = ressource.Stock;
				int gain = getNewValue (stockActuel, capacite);

				if (-gain > stockActuel) {	//Cas ou les gain son negatif et superieur à la stock
					gain = -stockActuel;
				}

				ressource.Stock = gain;
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_VOL_RESSOURCE && null != carteSource && null != carteSource.getJoueurProprietaire()) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSource);

			foreach (RessourceMetier ressource in ressourcesCible) {
				if (ressource.NetIdJoueur != carteSource.getJoueurProprietaire ().netId) { //on ne peut se voler soit même
					int stockActuel = ressource.Stock;
					int montantVoler = getNewValue (stockActuel, capacite);

					if (ressource.Stock < montantVoler) {
						montantVoler = ressource.Stock;
					}

					ressource.Stock -= montantVoler;
					int montantReelVole = carteSource.getJoueurProprietaire ().addRessource (ressource.TypeRessource, montantVoler);

					if (montantReelVole != montantVoler) {
						ressource.Stock += montantVoler - montantReelVole;
					}
				}
			}
		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_TYPE_RESSOURCE) {
			List<RessourceMetier> ressourcesCible = getRessourceCible (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSource);

			foreach (RessourceMetier ressource in ressourcesCible) {
					int stockActuel = ressource.Stock;
					int montantEchange = getNewValue (stockActuel, capacite);

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
			CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idTypeCapacite, carteSource.netId);
			List<DeckMetierAbstract> decksCible = getDecksCibles (capacite.ConditionsCible, netIdJoueurAction, netIdJoueurCible, carteSource);
			foreach (DeckMetierAbstract deck in decksCible) {
				deck.ListCapaciteDeck.Add (capaciteMetier);
			}

		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU || idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_DISSIMULER_PLATEAU) {
			Joueur joueurRessource = null;
			List<DeckMetierAbstract> listDeckCible = new List<DeckMetierAbstract>();

			foreach (string conditionCible in capacite.ConditionsCible) {
				HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurAction, netIdJoueurCible, carteSource);
				CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite,idTypeCapacite, carteSource.netId);
				foreach (Joueur joueurCible in joueursCible) {
					joueurCible.addCapacity (capaciteMetier);
				}
			}
				
		} else if (idTypeCapacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_PLACE_PLATEAU) {
			//TODO en cas de nombre nzgatif desactiver emplacement
			//TODO en cas de nombre positif mettre l'emplacement sur un autre

		}
	}

	private static List<RessourceMetier> getRessourceCible (List<string> listConditionsCible, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSouce){
		List<RessourceMetier> listRessourceCible = new List<RessourceMetier>();

		foreach (string conditionCible in listConditionsCible) {
			HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurSource, netIdJoueurCible, carteSouce);

			foreach (Joueur joueurRessource in joueursCible) {
				if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_METAL.ToString ())) {
					listRessourceCible.Add (joueurRessource.RessourceMetal);
				} else if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_CARBURANT.ToString ())) {
					listRessourceCible.Add (joueurRessource.RessourceCarburant);
				}
			}
		}

		return listRessourceCible;
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