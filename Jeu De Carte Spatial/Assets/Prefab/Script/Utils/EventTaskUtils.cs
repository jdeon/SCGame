using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTaskUtils  {

	public static EventTask getEventTaskEnCours(){
		EventTask eventTaskResult = null;

		EventTask[] listeventTask = GameObject.FindObjectsOfType<EventTask> ();

		if (null != listeventTask && listeventTask.Length > 0) {
			foreach (EventTask eventTask in listeventTask) {
				if (eventTask.EnCours) {
					eventTaskResult = eventTask;
					break;
				}
			}
		}

		return eventTaskResult;
	}

	public static void launchEventCapacity (int idActionEvent, NetworkInstanceId netIdSourceAction, NetworkInstanceId netIdJoueurSourceAction,int idSelectionCible, NetworkInstanceId netIdEventTask){
		NetworkBehaviour ScriptSource = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (netIdSourceAction, false);
		//TODO remplacer les -1

		if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION) {
			ActionEventManager.EventActionManager.CmdPiocheConstruction (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_AMELIORATION) {
			ActionEventManager.EventActionManager.CmdPiocheAmelioration (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION || idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_POSE_AMELIORATION) {
			ActionEventManager.EventActionManager.CmdPoseCarte (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEBUT_TOUR) {
			ActionEventManager.EventActionManager.CmdStartTurn (netIdJoueurSourceAction, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_FIN_TOUR) {
			ActionEventManager.EventActionManager.CmdEndTurn (netIdJoueurSourceAction, -1, netIdEventTask);//TODO remplacer-1
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE) {
			ActionEventManager.EventActionManager.CmdAttaque (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND) {
			ActionEventManager.EventActionManager.CmdDefense (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_UTILISE) {
			ActionEventManager.EventActionManager.CmdUtilise (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE) {
			ActionEventManager.EventActionManager.CmdDestruction (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_FIN_ATTAQUE) {
			//TODO ActionEventManager.EventActionManager.CmdDestruction (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_GAIN_XP) {
			ActionEventManager.EventActionManager.CmdXPGain (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {
			//TODO action invocation ActionEventManager.EventActionManager.CmdDestruction (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT) {
			ActionEventManager.EventActionManager.CmdRecoitDegat (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE) {
			// TODO ActionEventManager.EventActionManager.CmdRecoitDegat (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE) {
			//TODO ActionEventManager.EventActionManager.CmdRecoitDegat (netIdJoueurSourceAction, netIdSourceAction, idSelectionCible, netIdEventTask);
		} else {
			//TODO ???
		}
	}


		public static void launchEventAction (int idActionEvent, NetworkInstanceId netIdSourceAction, NetworkInstanceId netIdJoueurSourceAction,int idSelectionCible, NetworkInstanceId netIdEventTask){
		NetworkBehaviour scriptSource = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (netIdSourceAction, false);
		//TODO remplacer les -1

		if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION) {

			if (scriptSource is CarteConstructionMetierAbstract) {
				((CarteConstructionMetierAbstract)scriptSource).CmdPiocheCard (netIdJoueurSourceAction);
			} else {
				aucuneActionEffectuer ();
			}
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_AMELIORATION) {
			//TODO
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION 
			|| idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_POSE_AMELIORATION
			|| idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART
			|| idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE) {

			ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionCible);

			if (null != cible && cible is IConteneurCarte && scriptSource is CarteConstructionMetierAbstract) {

				if (((CarteConstructionMetierAbstract)scriptSource).isDeplacable()) {
					EmplacementUtils.putCardFromServer ((IConteneurCarte)cible, (CarteConstructionMetierAbstract) scriptSource);
				}
			} else {
				aucuneActionEffectuer ();
			}

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEBUT_TOUR) {
			
			Joueur joueur = ConvertUtils.convertNetIdToScript<Joueur> (netIdJoueurSourceAction, false);
			if (null != joueur) {
				TourJeuSystem.getTourSystem ().debutTour (joueur);
			} else {
				aucuneActionEffectuer ();
			}
				
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_FIN_TOUR) {

			Joueur joueur = ConvertUtils.convertNetIdToScript<Joueur> (netIdJoueurSourceAction, false);
			if (null != joueur) {
				TourJeuSystem.getTourSystem ().finTour (joueur);
			} else {
				aucuneActionEffectuer ();
			}

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE && scriptSource is IAttaquer) {
			ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionCible);

			if (null != cible && cible is CarteConstructionMetierAbstract) {
				((IAttaquer)scriptSource).attaqueCarte ((CarteConstructionMetierAbstract)cible, netIdEventTask);
			} else if (null != cible && cible is CartePlaneteMetier) {
				((IAttaquer)scriptSource).attaquePlanete ((CartePlaneteMetier)cible, netIdEventTask);
			} else {
				aucuneActionEffectuer ();
			}
				
		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND  && scriptSource is CarteMetierAbstract) {
			ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionCible);

			if (null != cible && cible is CarteVaisseauMetier && scriptSource is IDefendre) {
				((IDefendre)scriptSource).defenseSimultanee ((CarteVaisseauMetier)cible, netIdEventTask);
			} else if (scriptSource is CartePlaneteMetier) {

				List<CarteConstructionMetierAbstract> listDefenseur = CarteUtils.getListCarteCapableDefendrePlanete (((CartePlaneteMetier)scriptSource).JoueurProprietaire);

				if (null != listDefenseur && listDefenseur.Count > 0) {
					SelectionCiblesExecutionCapacite selectionCible = new SelectionCiblesExecutionCapacite (1, (CartePlaneteMetier)scriptSource, idActionEvent);

					foreach (CarteConstructionMetierAbstract defenseur in listDefenseur) {
						selectionCible.ListIdCiblesProbables.Add (defenseur.IdISelectionnable);
					}

					ActionEventManager.EventActionManager.createTaskChooseTarget (selectionCible, scriptSource.netId, ((CartePlaneteMetier)scriptSource).JoueurProprietaire.netId, cible.IdISelectionnable, idActionEvent, netIdEventTask);

				} else {
					ActionEventManager.EventActionManager.CreateTask (scriptSource.netId, ((CartePlaneteMetier)scriptSource).JoueurProprietaire.netId, cible.IdISelectionnable , ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netIdEventTask, false);
				}
					
			}else {
				aucuneActionEffectuer ();
			}

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_UTILISE) {

			//TODO 

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE && scriptSource is IVulnerable) {
				
			((IVulnerable)scriptSource).destruction (netIdEventTask);

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_FIN_ATTAQUE) {

			ActionEventManager.EventActionManager.CreateTask (NetworkInstanceId.Invalid, netIdJoueurSourceAction, -1, ConstanteIdObjet.ID_CONDITION_ACTION_FIN_TOUR, NetworkInstanceId.Invalid, false);

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_GAIN_XP) {

			//TODO

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION) {

			//TODO 

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT && scriptSource is IVulnerable) {

			ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionCible);

			if (null != cible && cible is CarteVaisseauMetier) {
				((IVulnerable)scriptSource).recevoirDegat (((CarteVaisseauMetier)cible).getPointDegat(), (CarteVaisseauMetier)cible, netIdEventTask);
			} else if (null != cible && cible is CarteDefenseMetier) {
				((IVulnerable)scriptSource).recevoirDegat (((CarteDefenseMetier)cible).getPointDegat(), (CarteDefenseMetier)cible, netIdEventTask);
			} else {
				aucuneActionEffectuer ();
			}

		} else if (idActionEvent == ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE) {
			
			//TODO 

		} else {
			aucuneActionEffectuer ();
		}
	}

	public static void aucuneActionEffectuer (){
		//TODO ???
	}
}
