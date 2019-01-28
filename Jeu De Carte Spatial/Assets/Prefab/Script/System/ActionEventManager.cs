﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActionEventManager : NetworkBehaviour {

	public delegate void TurnEventHandler(NetworkInstanceId netIdJoueur);

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
	public void CmdStartTurn (NetworkInstanceId netIdJoueur){
		if (null != onStartTurn) {
			onStartTurn (netIdJoueur);
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

	[Command]
	public void CmdPiocheConstruction (NetworkInstanceId netIdJoueur,  NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){
		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != onPiocheConstruction) {
			onPiocheConstruction (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdPiocheAmelioration (NetworkInstanceId netIdJoueur,  NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){
		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != onPiocheAmelioration) {
			onPiocheAmelioration (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdPoseCarte (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction) {
			if (carteOrigineAction is CarteConstructionMetierAbstract){
				if (null != onPoseConstruction) {
					onPoseConstruction (netIdJoueur, carteOrigineAction, cible);
				}
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
	}

	[Command]
	public void CmdAttaque (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onAttaque) {
			onAttaque (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdDefense (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onDefense) {
			onDefense (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdUtilise (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onUtilise) {
			onUtilise (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdDestruction (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onDestruction) {
			onDestruction (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdXPGain (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != onXPGain) {
			onXPGain (netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdRecoitDegat (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onRecoitDegat) {
			onRecoitDegat(netIdJoueur, carteOrigineAction, cible);
		}
	}

	[Command]
	public void CmdCardDeplacement (NetworkInstanceId netIdJoueur, NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdCible){

		CarteMetierAbstract carteOrigineAction = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarteSource, false);
		ISelectionnable cible = convertNetIdToISelectionnable (netIdCible, false);

		if (null != carteOrigineAction && null != onCardDeplacement) {
			onCardDeplacement (netIdJoueur, carteOrigineAction, cible);
		}
	}

	private ISelectionnable convertNetIdToISelectionnable(NetworkInstanceId netId, bool localPlayer){
		ISelectionnable result;
		NetworkBehaviour nBScripObject = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (netId, localPlayer);

		if (null != nBScripObject && nBScripObject is ISelectionnable) {
			result = (ISelectionnable)nBScripObject;
		} else {
			result = null;
		}

		return result;
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
}