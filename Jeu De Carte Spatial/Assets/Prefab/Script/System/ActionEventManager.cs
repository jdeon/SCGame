using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActionEventManager : NetworkBehaviour {

	public delegate void TurnEventHandler(NetworkInstanceId netIdJoueur);

	public static event TurnEventHandler onStartTurn;
	public static event TurnEventHandler onPiocheConstruction;
	public static event TurnEventHandler onPiocheAmelioration;
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
	}

	[Command]
	public void CmdStartTurn (NetworkInstanceId netIdJoueur){
		if (null != onStartTurn) {
			onStartTurn (netIdJoueur);
		}
	}

	[Command]
	public void CmdPiocheConstruction (NetworkInstanceId netIdJoueur){
		if (null != onPiocheConstruction) {
			onPiocheConstruction (netIdJoueur);
		}
	}

	[Command]
	public void CmdPiocheAmelioration (NetworkInstanceId netIdJoueur){
		if (null != onPiocheAmelioration) {
			onPiocheAmelioration (netIdJoueur);
		}
	}

	[Command]
	public void CmdEndTurn (NetworkInstanceId netIdJoueur, int phasePrecedente){
		if (phasePrecedente == TourJeuSystem.PHASE_ATTAQUE && null != onFinPhaseAttaque) {
			onFinPhaseAttaque (netIdJoueur);
		}

		if (null != onEndTurn) {
			onEndTurn (netIdJoueur);
		}
	}


	public delegate void CardEventHandler(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible);

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

	[Command]
	public void CmdPoseCarte (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (carteOrigineAction is CarteConstructionMetierAbstract && null != onPoseConstruction) {
			onPoseConstruction (netIdJoueur, carteOrigineAction, cible);
		}

		//TODO carteAmeliration
		/*if (carteOrigineAction is CarteAmeliorationMetier && null != onPoseConstruction) {
			onPoseConstruction (netIdJoueur);
		}*/

		if (null != onInvocation) {
			//TODO carteInvoque?
			onInvocation (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdAttaque (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onAttaque) {
			onAttaque (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdDefense (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onDefense) {
			onDefense (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdUtilise (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onUtilise) {
			onUtilise (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdDestruction (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onDestruction) {
			onDestruction (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdXPGain (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onXPGain) {
			onXPGain (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdRecoitDegat (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onRecoitDegat) {
			onRecoitDegat(netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdCardDeplacement (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onCardDeplacement) {
			onCardDeplacement (netIdJoueur, carteOrigineAction, cible);
		}
	}

	/****************************Systeme coroutine**************************/
	[Command]
	public void CmdAddNewCoroutine(){
		int idCoroutine = sequenceIdCoroutine++;
		listIdCouroutineAtraiter.Add (idCoroutine);

		if(listIdCouroutineAtraiter.Count == 1){
			idCoroutineEnCours = idCoroutine;
		}

		RpcSetNewIdCoroutine (idCoroutine);
	}

	[Command]
	public void CmdAddNewCoroutinePrioriaire(){
		int idCoroutine = sequenceIdCoroutine++;
		listIdCouroutineAtraiterPrioritaire.Add (idCoroutine);

		if(listIdCouroutineAtraiterPrioritaire.Count == 1){
			idCoroutineEnCours = idCoroutine;
		}

		RpcSetNewIdCoroutine (idCoroutine);
	}

	[ClientRpc]
	public void RpcSetNewIdCoroutine(int newIdCoroutine){
		nextIdCoroutine = newIdCoroutine;
	}

	[Command]
	public void CmdEndOfCoroutine(){
		if(listIdCouroutineAtraiter.Count> 0){
			listIdCouroutineAtraiter.RemoveAt (0);
		}

		if(listIdCouroutineAtraiter.Count > 0){
			idCoroutineEnCours = listIdCouroutineAtraiter[0];
		} else {
			idCoroutineEnCours = 0;
		}
	}

	public int IdCoroutineEnCours{
		get { return idCoroutineEnCours; }
	}


	/*******************************************************/
	[ClientRpc]
	public void RpcDisplayCapacity (SelectionCiblesExecutionCapacite selectionCibles){

	}
}
