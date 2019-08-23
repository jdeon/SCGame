using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActionEventManager : NetworkBehaviour {

	public static Dictionary<int, CapaciteDTO> capacityInUse = new Dictionary<int, CapaciteDTO> ();

	public static int sequenceCapacityInUse = 1;

	public delegate void TurnEventHandler(NetworkInstanceId netIdJoueur, NetworkInstanceId netIdTaskEvent);

	public static event TurnEventHandler onStartTurn;
	public static event TurnEventHandler onFinPhaseAttaque;
	public static event TurnEventHandler onEndTurn;

	public static ActionEventManager EventActionManager;

	private static int sequenceIdCoroutine = 1;

	[SyncVar]
	private int idCoroutineEnCours = 0;

	[SyncVar]
	public int nextIdCoroutine = -1;

	private List<int> listIdCouroutineAtraiter;

	private List<int> listIdCouroutineAtraiterPrioritaire;

	public void Start(){
		EventActionManager = this;
		listIdCouroutineAtraiter = new List<int>();
		listIdCouroutineAtraiterPrioritaire = new List<int>();
	}

	[Command]
	public void CmdStartTurn (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdTaskEvent){
		if (null != onStartTurn) {
			onStartTurn (netIdJoueur, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdEndTurn (NetworkInstanceId netIdJoueur, int phasePrecedente, NetworkInstanceId netIdTaskEvent){
		if (phasePrecedente == TourJeuSystem.PHASE_ATTAQUE && null != onFinPhaseAttaque) {
			onFinPhaseAttaque (netIdJoueur, netIdTaskEvent);
		}

		if (null != onEndTurn) {
			onEndTurn (netIdJoueur, netIdTaskEvent);
		}
	}


	public delegate void CardEventHandler(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent);

	public static event CardEventHandler onPiocheConstruction;
	public static event CardEventHandler onPiocheAmelioration;
	public static event CardEventHandler onPoseConstruction;
	public static event CardEventHandler onPoseAmelioration;
	public static event CardEventHandler onAttaque;
	public static event CardEventHandler onDefense;
	public static event CardEventHandler onUtilise;
	public static event CardEventHandler onDestruction;
	public static event CardEventHandler onXPGain;
	public static event CardEventHandler onInvocation;
	public static event CardEventHandler onRecoitDegat;
	public static event CardEventHandler onCardDeplacement;
	public static event CardEventHandler onCardEvolution;

	[Command]
	public void CmdPiocheConstruction (NetworkInstanceId netIdJoueur,  NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){
		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != onPiocheConstruction) {
			onPiocheConstruction (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdPiocheAmelioration (NetworkInstanceId netIdJoueur,  NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){
		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != onPiocheAmelioration) {
			onPiocheAmelioration (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdPoseCarte (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction) {
			if (carteOrigineAction is CarteConstructionMetierAbstract){
				if (null != onPoseConstruction) {
					onPoseConstruction (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
				}
			}

			//TODO carteAmeliration
			/*if (carteOrigineAction is CarteAmeliorationMetier && null != onPoseConstruction) {
			onPoseConstruction (netIdJoueur);
		}*/
			/*
			if (null != onInvocation) {
				//TODO carteInvoque?
				onInvocation (netIdJoueur, carteOrigineAction, cible);
			}*/
		}
	}

	[Command]
	public void CmdAttaque (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onAttaque) {
			onAttaque (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdDefense (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onDefense) {
			onDefense (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdUtilise (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onUtilise) {
			onUtilise (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdDestruction (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onDestruction) {
			onDestruction (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdXPGain (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != onXPGain) {
			onXPGain (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdRecoitDegat (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onRecoitDegat) {
			onRecoitDegat(netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdCardDeplacement (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != carteOrigineAction && null != onCardDeplacement) {
			onCardDeplacement (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}

	[Command]
	public void CmdCardEvolution (NetworkInstanceId netIdJoueur,  NetworkInstanceId netIdCarteSource, int idSelectionnableCible, NetworkInstanceId netIdTaskEvent){
		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idSelectionnableCible);

		if (null != onPiocheConstruction&& null != onCardEvolution) {
			onCardEvolution (netIdJoueur, carteOrigineAction, cible, netIdTaskEvent);
		}
	}
		
	public void CreateTask(NetworkInstanceId netIdSourceAction, NetworkInstanceId netIdJoueurSourceAction, int idSelectionCible, int typeAction, NetworkInstanceId netIdParentTask, bool createTaskBrother){
		if (isServer) {
			GameObject eventTaskGO = Instantiate<GameObject> (ConstanteInGame.eventTaskPrefab);


			GameObject eventParnetTaskGO;
			if (netIdParentTask == NetworkInstanceId.Invalid) {
				eventParnetTaskGO = GameObject.Find ("SystemActionEvent");
			} else {
				eventParnetTaskGO = NetworkServer.FindLocalObject (netIdParentTask);
			}

			eventTaskGO.transform.SetParent (eventParnetTaskGO.transform);

			EventTask eventTask = eventTaskGO.GetComponent<EventTask> ();

			eventTask.initVariable (netIdSourceAction, netIdJoueurSourceAction, idSelectionCible, typeAction, createTaskBrother);

			NetworkServer.Spawn (eventTaskGO);
		} else {
			print ("Create Task call on client");
		}
	}
		
	public void createTaskChooseTarget(SelectionCiblesExecutionCapacite selectionCibles,NetworkInstanceId netIdSourceAction, NetworkInstanceId netIdJoueurSourceAction, int idSelectionCible, int typeAction, NetworkInstanceId netIdParentTask){
		if (isServer) {
			GameObject eventTaskChooseTargetGO = Instantiate<GameObject> (ConstanteInGame.eventTaskChooseTargetPrefab);

			GameObject eventParnetTaskGO;
			if (netIdParentTask == NetworkInstanceId.Invalid) {
				eventParnetTaskGO = GameObject.Find (ConstanteInGame.strSystemActionEvent);
			} else {
				eventParnetTaskGO = NetworkServer.FindLocalObject (netIdParentTask);
			}

			eventTaskChooseTargetGO.transform.SetParent (eventParnetTaskGO.transform);

			EventTaskChoixCible eventTask = eventTaskChooseTargetGO.GetComponent<EventTaskChoixCible> ();
			eventTask.initVariable (netIdSourceAction, netIdJoueurSourceAction, idSelectionCible, typeAction, false);

			NetworkServer.Spawn (eventTaskChooseTargetGO);

			eventTask.SelectionCibles = selectionCibles;
		} else {
			print ("Create TaskChooseTarget call on client");
		}
	}


	[Command]
	public void CmdExecuteCapacity(int[] listCibleProbable, NetworkInstanceId netIdEventTask){
		EventTaskChoixCible eventSource = ConvertUtils.convertNetIdToScript<EventTaskChoixCible>(netIdEventTask,false);
		eventSource.SelectionCibles.ListIdCiblesProbables.Clear();
		eventSource.SelectionCibles.ListIdCiblesProbables.AddRange (listCibleProbable);

		//TODO modifier
		CapaciteUtils.executeCapacity (eventSource.SelectionCibles, netIdEventTask);
		//TODO display capa

		eventSource.endOfTask ();
	}
}
