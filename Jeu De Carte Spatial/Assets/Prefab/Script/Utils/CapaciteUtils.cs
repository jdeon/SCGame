using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CapaciteUtils {

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

	public static bool isCapaciteCall(CapaciteDTO capaciteTest, int idConditionAction, bool isAllier, bool isProvenance){
		bool capable = false;
		string allieOuEnnemi = isAllier ? ConstanteIdObjet.STR_CONDITION_POUR_ALLIER : ConstanteIdObjet.STR_CONDITION_POUR_ENNEMIE;

		foreach (string conditionAction in capaciteTest.ConditionsAction) {
			string[] tabConditionAction = conditionAction.Split (char.Parse("-"));
			if (tabConditionAction.Length >= 2 && tabConditionAction [0] == idConditionAction.ToString()
				&& (tabConditionAction [1].Contains (allieOuEnnemi) || (isProvenance && tabConditionAction [1].Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE)))) {
				capable = true;
			}
		}	

		return capable;
	}

	public static void callCapacite(CarteMetierAbstract carteSourceCapacite, CarteMetierAbstract carteSourceAction, ISelectionnable cible, CapaciteDTO capaciteSource, NetworkInstanceId netIdJoueur, int actionAppelante){
		SelectionCiblesExecutionCapacite selectionCiblesResult = getCiblesOfCapacity (carteSourceCapacite,carteSourceAction, cible, capaciteSource, netIdJoueur,actionAppelante);

		if (null != selectionCiblesResult && selectionCiblesResult.ListCiblesProbables.Count > 0) {
			if (capaciteSource.ChoixCible) {
				GameObject goJoueur = NetworkServer.FindLocalObject (netIdJoueur);
				if (null != goJoueur && null != goJoueur.GetComponent<Joueur> ()) {
					Joueur joueur = goJoueur.GetComponent<Joueur> ();
					byte[] selectionCibleByte = SerializeUtils.SerializeToByteArray (selectionCiblesResult);
					joueur.RpcDisplayCapacityChoice (selectionCibleByte);
				}

			} else {
				
				while (selectionCiblesResult.ListCiblesProbables.Count > selectionCiblesResult.NbCible) {
					int indexToDelete = Random.Range (0, selectionCiblesResult.ListCiblesProbables.Count);
					selectionCiblesResult.ListCiblesProbables.RemoveAt (indexToDelete);
				}

				executeCapacity(selectionCiblesResult);
			}
		}
	}
		
	public static void executeCapacity(SelectionCiblesExecutionCapacite selectionCiblesResult){
		Joueur joueurSource = Joueur.getJoueur(selectionCiblesResult.IdJoueurCarteSource);

		foreach (ISelectionnable cibleSelectionne in selectionCiblesResult.ListCiblesProbables) {
			if (listIdCapaciteEffetImmediat.Contains (selectionCiblesResult.IdTypeCapacite)) {
				if (cibleSelectionne is CarteMetierAbstract) {
					traitementCapaciteImmediateCarte ((CarteMetierAbstract) cibleSelectionne, selectionCiblesResult.Capacite, selectionCiblesResult.IdActionAppelante, joueurSource);
				} else {
					//TODO traitement capacite immediate autre
				}

			} else if (selectionCiblesResult is SelectionCiblesExecutionCapaciteRessource && cibleSelectionne is IAvecCapacite) {
				
				//TODO faire le trie ressource ou non
				traitementCapaciteRessource (selectionCiblesResult.Capacite,((SelectionCiblesExecutionCapaciteRessource)selectionCiblesResult).ListRessouceCible, (IAvecCapacite) cibleSelectionne, joueurSource, selectionCiblesResult.IdCarteSource);
				foreach (RessourceMetier ressourceCible in ((SelectionCiblesExecutionCapaciteRessource)selectionCiblesResult).ListRessouceCible) {
					ressourceCible.synchroniseListCapacite ();
				}



			} else if(cibleSelectionne is IAvecCapacite) {
				//TODO faire le trie ressource ou non
				traitementAutreCible ((IAvecCapacite) cibleSelectionne, selectionCiblesResult.Capacite, selectionCiblesResult.IdCarteSource);
				((IAvecCapacite)cibleSelectionne).synchroniseListCapacite();

			}
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

	public static CapaciteMetier convertCapaciteDTOToMetier(CapaciteDTO capaciteDTO, NetworkInstanceId netIdCard){
		//TODO prendre en compte module
		return new CapaciteMetier (capaciteDTO.Capacite,capaciteDTO.Id,capaciteDTO.ModeCalcul,capaciteDTO.Quantite,netIdCard, capaciteDTO.LierACarte);
	}

	private static SelectionCiblesExecutionCapacite getCiblesOfCapacity(CarteMetierAbstract carteSourceCapacite, CarteMetierAbstract carteSourceAction,ISelectionnable cible, CapaciteDTO capaciteSource, NetworkInstanceId netIdJoueur, int actionAppelante){
		SelectionCiblesExecutionCapacite selectionCiblesResult = null;

		CapaciteDTO capacite = capaciteSource.Clone ();

		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_ACTION_HASARD) {
			capacite.Capacite = Random.Range (1, 40);
		}


		if (ConditionCarteUtils.listIdCapacitePourCarte.Contains (capacite.Capacite) && (null == cible || cible is CarteMetierAbstract)) {
			List<CarteMetierAbstract> cartesCible = getCartesCible (carteSourceCapacite, (CarteMetierAbstract)cible, capacite, netIdJoueur);
			//TODO use ConstanteIdObjet.ID_CAPACITE_CONDITION
			selectionCiblesResult = new SelectionCiblesExecutionCapacite(capacite, carteSourceCapacite, actionAppelante);
			selectionCiblesResult.ListCiblesProbables.AddRange (ConvertUtils.convertToListParent<ISelectionnable,CarteMetierAbstract> (cartesCible));
		} else {
			selectionCiblesResult = findCiblesHorsCarte (capacite, carteSourceCapacite,carteSourceAction, netIdJoueur, NetworkInstanceId.Invalid /*TODO cible.netIdJoueurCible*/, actionAppelante);
		}

		return selectionCiblesResult;
	}

	private static SelectionCiblesExecutionCapacite findCiblesHorsCarte (CapaciteDTO capacite, CarteMetierAbstract carteSourceCapacite, CarteMetierAbstract carteSourceAction,NetworkInstanceId netIdJoueurAction, NetworkInstanceId netIdJoueurCible, int actionAppelante){
		SelectionCiblesExecutionCapacite selectionCible = null;

		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE 
			|| capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE 
			|| (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_VOL_RESSOURCE && null != carteSourceCapacite && null != carteSourceCapacite.getJoueurProprietaire())
			|| capacite.Capacite  == ConstanteIdObjet.ID_CAPACITE_MODIF_TYPE_RESSOURCE) {

			selectionCible = new SelectionCiblesExecutionCapaciteRessource (capacite, carteSourceCapacite, actionAppelante);

			List<RessourceMetier> ressourcesCible = getRessourceMetierCible (capacite.ConditionsEmplacement, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);
			((SelectionCiblesExecutionCapaciteRessource)selectionCible).ListRessouceCible.AddRange (ressourcesCible);

			if (capacite.NbCible == 1 && !capacite.ChoixCible 
				&& (actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE || actionAppelante == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION)
				&& isCardCibleCapacity(carteSourceAction, capacite.ConditionsCible, netIdJoueurAction)) {
				selectionCible.ListCiblesProbables.Add (carteSourceAction);
				//TODO prise en compte appel unique
			} else {
				HashSet<ISelectionnable> provenanceRessource = getRessourceProvenanceCible (capacite, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);
				selectionCible.ListCiblesProbables.AddRange (provenanceRessource);
			}

		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_CARTE_PIOCHE) {

			selectionCible = new SelectionCiblesExecutionCapacite (capacite, carteSourceCapacite, actionAppelante);

			List<DeckMetierAbstract> decksCible = getDecksCibles (capacite, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite);
			selectionCible.ListCiblesProbables.AddRange (ConvertUtils.convertToListParent<ISelectionnable,DeckMetierAbstract> (decksCible));

		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU || capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_DISSIMULER_PLATEAU) {
			HashSet<Joueur> joueursCible = new HashSet<Joueur> ();
			selectionCible = new SelectionCiblesExecutionCapacite (capacite, carteSourceCapacite, actionAppelante);

			foreach (string conditionCible in capacite.ConditionsCible) {
				joueursCible.UnionWith(getJoueursCible (conditionCible, netIdJoueurAction, netIdJoueurCible, carteSourceCapacite));
			}

			if (capacite.AppelUnique) {
				foreach (Joueur joueurCible in joueursCible) {
					if (!joueurCible.containCapacityWithId (capacite.Id)) {
						selectionCible.ListCiblesProbables.Add (joueurCible);
					}
				}
			} else {
				selectionCible.ListCiblesProbables.AddRange (ConvertUtils.convertToListParent<ISelectionnable,Joueur>(joueursCible));
			}


		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_PLACE_PLATEAU) {
			//TODO quelle est la cible (emplacement, plateau, ?)

		}

		return selectionCible;
	}

	private static List<CarteMetierAbstract> getCartesCible (CarteMetierAbstract carteOrigin, CarteMetierAbstract carteCible,CapaciteDTO capacite, NetworkInstanceId netIdJoueur){
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

	/**
	 * carteCibleCapacite : Carte vers laquelle la capacite va aller
	 * capaciteImmediate : capacite applicable immediatement
	 * carteOrigine : carte d'ou provient la capacite
	 * carteCibleAction : carte cibler lors de l'appel de l'event
	 * */
	private static void traitementCapaciteImmediateCarte(CarteMetierAbstract carteCibleCapacite, CapaciteDTO capaciteImmediate, int actionAppelante, Joueur joueur){
		if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_EMPLACEMENT_CARTE) {
			//TODO
		
		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_VOLE_CARTE) {
			//TODO Definir le conteneur mains, deck, emplacement
			carteCibleCapacite.deplacerCarte (joueur.Main, joueur.netId);

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_DESTRUCTION_CARTE) {
			//TODO que faire si pas IVulnerable?
			if (carteCibleCapacite is IVulnerable) {
				((IVulnerable)carteCibleCapacite).destruction ();
			}

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_INVOQUE_CARTE && null != capaciteImmediate.CarteInvocation) {
			GameObject carteGO = CarteUtils.convertCarteDTOToGameobject (capaciteImmediate.CarteInvocation);

			//TODO ne prends pas en compte l'emplacement cible ou l invocation chez l ennemie
			joueur.invoquerCarte (carteGO, capaciteImmediate.NiveauInvocation, joueur.Main);

		} else if (capaciteImmediate.Capacite == ConstanteIdObjet.ID_CAPACITE_REVELE_CARTE) {
			//TODO RPC carteCibleCapacite.generateVisualCard ();
		}
	}

	private static void traitementCapaciteRessource (CapaciteDTO capacite, List<RessourceMetier> ressourcesCible, IAvecCapacite cibleSelectionne, Joueur joueurCarteSource, NetworkInstanceId netIdCarteSource/*, NetworkInstanceId netIdJoueurAction, NetworkInstanceId netIdJoueurCible, int actionAppelante*/){
		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE 
			|| capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_STOCK_RESSOURCE) {
			//Cas ou l on rajoute une capaciteMetier
			foreach (RessourceMetier ressource in ressourcesCible) {	


				if (cibleSelectionne is CarteBatimentMetier || cibleSelectionne is CarteDefenseMetier || cibleSelectionne is CarteVaisseauMetier) {
					CapaciteDTO capaciteRessource = getRessourceFromCarte (capacite, cibleSelectionne, ressource.TypeRessource);
					CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capaciteRessource, netIdCarteSource);
					ressource.addCapacity (capaciteMetier);
				} else {

					CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite, netIdCarteSource);
					ressource.addCapacity (capaciteMetier);
				}
			}

		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_VOL_RESSOURCE && null != joueurCarteSource) {

			foreach (RessourceMetier ressource in ressourcesCible) {
				CapaciteDTO capaciteUtile = null;
				if (ressource.NetIdJoueur != joueurCarteSource.netId) { //on ne peut se voler soit même

					if (cibleSelectionne is CarteBatimentMetier || cibleSelectionne is CarteDefenseMetier || cibleSelectionne is CarteVaisseauMetier) {
						capaciteUtile = getRessourceFromCarte (capacite, cibleSelectionne, ressource.TypeRessource);
					} else {
						capaciteUtile = capacite;
					}



					int stockActuel = ressource.Stock;

					int newStock = getNewValue(stockActuel, capaciteUtile.Quantite, capaciteUtile.ModeCalcul);
					int montantVoler = newStock - stockActuel;

					if (ressource.Stock < montantVoler) {
						montantVoler = ressource.Stock;
					}

					ressource.Stock -= montantVoler;
					int montantReelVole = joueurCarteSource.addRessource (ressource.TypeRessource, montantVoler);

					if (montantReelVole != montantVoler) {
						ressource.Stock += montantVoler - montantReelVole;
					}
				}
			}
		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_TYPE_RESSOURCE) {

			foreach (RessourceMetier ressource in ressourcesCible) {
				CapaciteDTO capaciteUtile;
					if (cibleSelectionne is CarteBatimentMetier || cibleSelectionne is CarteDefenseMetier || cibleSelectionne is CarteVaisseauMetier) {
						capaciteUtile = getRessourceFromCarte (capacite, cibleSelectionne, ressource.TypeRessource);
					} else {
						capaciteUtile = capacite;
					}

				int stockActuel = ressource.Stock;

				int newStock = getNewValue(stockActuel,capaciteUtile.Quantite,capaciteUtile.ModeCalcul);
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
		}
	}




	private static void traitementAutreCible (IAvecCapacite cible , CapaciteDTO capacite, NetworkInstanceId netIdCarteSourceCapacite){
		//TODO utilisation de nbCible
		if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_CARTE_PIOCHE) {
			if (cible is DeckMetierAbstract) {
				CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite, netIdCarteSourceCapacite);
				cible.addCapacity (capaciteMetier);
			}
			//TODO autre cible?

		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU || capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_DISSIMULER_PLATEAU) {

			if (cible is Joueur) {
				CapaciteMetier capaciteMetier = convertCapaciteDTOToMetier (capacite, netIdCarteSourceCapacite);
				cible.addCapacity (capaciteMetier);
			}
			//TODO autre cible
				
		} else if (capacite.Capacite == ConstanteIdObjet.ID_CAPACITE_MODIF_NB_PLACE_PLATEAU) {
			//TODO en cas de nombre nzgatif desactiver emplacement
			//TODO en cas de nombre positif mettre l'emplacement sur un autre

		}
	}

	private static List<RessourceMetier> getRessourceMetierCible (List<string> listConditionsEmplacement, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSouce){
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

	private static HashSet<ISelectionnable> getRessourceProvenanceCible (CapaciteDTO capaciteSource, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSource){
		HashSet<ISelectionnable> listResult = new HashSet<ISelectionnable> ();

		foreach (string conditionCible in capaciteSource.ConditionsCible) {
			string[] tabConditionCible = conditionCible.Split (char.Parse("-"));
			if (tabConditionCible.Length >= 2) {
				int idConditionCible = int.Parse (tabConditionCible [0]);
				string allegeance = tabConditionCible [1];

				if (idConditionCible == ConstanteIdObjet.ID_CONDITION_CIBLE_RESSOURCE) {
					HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurSource, netIdJoueurCible, carteSource);
					foreach (Joueur joueurRessource in joueursCible) {
						if (!capaciteSource.AppelUnique || !joueurRessource.RessourceMetal.containCapacityWithId (capaciteSource.Id)) {
							listResult.Add (joueurRessource.RessourceMetal);
						}

						if (!capaciteSource.AppelUnique || !joueurRessource.RessourceCarburant.containCapacityWithId (capaciteSource.Id)) {
							listResult.Add (joueurRessource.RessourceCarburant);
						}

						if (!capaciteSource.AppelUnique || !joueurRessource.RessourceXP.containCapacityWithId (capaciteSource.Id)) {
							listResult.Add (joueurRessource.RessourceXP);
						}
					}

				} else if (ConditionCarteUtils.listIdCondtionCibleCarte.Contains(idConditionCible)){

				List<IConteneurCarte> emplacementsOccuper = ConvertUtils.convertToListParent<IConteneurCarte,EmplacementMetierAbstract>(EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementMetierAbstract> (NetworkInstanceId.Invalid));

				List<CarteMetierAbstract> listCartesCibleProbable = ConditionCarteUtils.getMethodeCarteCible(idConditionCible, allegeance, emplacementsOccuper,carteSource,netIdJoueurSource);

					if (capaciteSource.AppelUnique) {
						foreach (CarteMetierAbstract carteProbable in listCartesCibleProbable) {
							//On vérifie que la carte ne possède pas déjà l'effet
							if(! carteProbable.containCapacityWithId(capaciteSource.Id)){
								listResult.Add (carteProbable);
							}
						}
					} else {
						listResult.UnionWith(ConvertUtils.convertToListParent<ISelectionnable,CarteMetierAbstract> (listCartesCibleProbable));
					}
				}
			}
		}

		return listResult;
	}

	private static bool isCardCibleCapacity(CarteMetierAbstract carteTest, List<string> listConditionCibel, NetworkInstanceId netIdJoueur){
		bool result = false;

		foreach (string conditionCible in listConditionCibel) {
			if ((conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT.ToString ()) && carteTest is CarteBatimentMetier)
				|| (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE.ToString ()) && carteTest is CarteDefenseMetier)
				|| (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU.ToString ()) && carteTest is CarteVaisseauMetier)) {
				if (null != carteTest && null != carteTest.getJoueurProprietaire () &&
					((conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && carteTest.getJoueurProprietaire ().netId == netIdJoueur)
						|| (conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_ALLIER) && carteTest.getJoueurProprietaire ().netId != netIdJoueur)
						|| (conditionCible.Contains (ConstanteIdObjet.STR_CONDITION_POUR_PROVENANCE)))) {

					result = true;
					break;
				}
			}
		}

		return result;
	}

	private static CapaciteDTO getRessourceFromCarte (CapaciteDTO capaciteSource, IAvecCapacite cibleSelectionne, string typeRessource){
		CapaciteDTO capaciteResult = capaciteSource.Clone ();

		foreach (string conditionCible in capaciteSource.ConditionsCible) {
			if ((conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_BATIMENT.ToString ()) && cibleSelectionne is CarteBatimentMetier)
				|| (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DEFENSE.ToString ()) && cibleSelectionne is CarteDefenseMetier)
				|| (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_VAISSEAU.ToString ()) && cibleSelectionne is CarteVaisseauMetier)) {

				capaciteResult.ModeCalcul = ConstanteEnum.TypeCalcul.Ajout;
				int ajouterAuRessource = 0;
				if (typeRessource == "Metal") {
					ajouterAuRessource = getNewValue (((CarteConstructionMetierAbstract)cibleSelectionne).getCoutMetalReelCarte (), capaciteSource.Quantite, capaciteSource.ModeCalcul);
				} else if (cibleSelectionne is CarteVaisseauMetier && typeRessource == "Carburant") {
					ajouterAuRessource = getNewValue (((CarteVaisseauMetier)cibleSelectionne).getConsomationCarburant (), capaciteSource.Quantite, capaciteSource.ModeCalcul);
				} else if (typeRessource == "XP") {
					ajouterAuRessource = getNewValue (((CarteConstructionMetierAbstract)cibleSelectionne).NiveauActuel, capaciteSource.Quantite, capaciteSource.ModeCalcul);
				}
				//TODO autre cible ?

				capaciteResult.Quantite = ajouterAuRessource;
			}
		}

		return capaciteResult;
	}

	private static List<DeckMetierAbstract> getDecksCibles (CapaciteDTO capacite, NetworkInstanceId netIdJoueurSource, NetworkInstanceId netIdJoueurCible, CarteMetierAbstract carteSouce){
		List<DeckMetierAbstract> listDeckCible = new List<DeckMetierAbstract>();

		foreach (string conditionCible in capacite.ConditionsCible) {
			HashSet<Joueur> joueursCible = getJoueursCible (conditionCible, netIdJoueurSource, netIdJoueurCible, carteSouce);

			foreach (Joueur joueurRessource in joueursCible) {
				if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_CONSTRUCTION.ToString ()) && null != joueurRessource.DeckConstruction
					&& (!capacite.AppelUnique || !joueurRessource.DeckConstruction.containCapacityWithId(capacite.Id))) {
					listDeckCible.Add (joueurRessource.DeckConstruction);
				} else if (conditionCible.Contains (ConstanteIdObjet.ID_CONDITION_CIBLE_DECK_AMELIORATION.ToString ())) {
					//TODO ajouter deck amelioration 
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