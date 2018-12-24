using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PhaseEventManager : MonoBehaviour {

	public delegate void TurnEventHandler(NetworkInstanceId netIdJoueur);

	public static event TurnEventHandler onStartTurn;
	public static event TurnEventHandler onPiocheConstruction;
	public static event TurnEventHandler onPiocheAmelioration;
	public static event TurnEventHandler onFinPhaseAttaque;
	public static event TurnEventHandler onEndTurn;

	public static void StartTurn (NetworkInstanceId netIdJoueur){
		if (null != onStartTurn) {
			onStartTurn (netIdJoueur);
		}
	}

	public static void PiocheConstruction (NetworkInstanceId netIdJoueur){
		if (null != onPiocheConstruction) {
			onPiocheConstruction (netIdJoueur);
		}
	}

	public static void PiocheAmelioration (NetworkInstanceId netIdJoueur){
		if (null != onPiocheAmelioration) {
			onPiocheAmelioration (netIdJoueur);
		}
	}

	public static void EndTurn (NetworkInstanceId netIdJoueur, int phasePrecedente){
		if (phasePrecedente == TourJeuSystem.PHASE_ATTAQUE && null != onFinPhaseAttaque) {
			onFinPhaseAttaque (netIdJoueur);
		}

		if (null != onEndTurn) {
			onEndTurn (netIdJoueur);
		}
	}


	public delegate void CardEventHandler(NetworkInstanceId netIdJoueur);

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

	public static void PoseCarte (NetworkInstanceId netIdJoueur, CarteMetierAbstract carte){
		if (carte is CarteConstructionMetierAbstract && null != onPoseConstruction) {
			onPoseConstruction (netIdJoueur);
		}

		//TODO carteAmeliration
		/*if (carte is CarteAmeliorationMetier && null != onPoseConstruction) {
			onPoseConstruction (netIdJoueur);
		}*/

		if (null != onInvocation) {
			onInvocation (netIdJoueur);
		}
	}

	public static void Attaque (NetworkInstanceId netIdJoueur){
		if (null != onAttaque) {
			onAttaque (netIdJoueur);
		}
	}

	public static void Defense (NetworkInstanceId netIdJoueur){
		if (null != onDefense) {
			onDefense (netIdJoueur);
		}
	}

	public static void Utilise (NetworkInstanceId netIdJoueur){
		if (null != onUtilise) {
			onUtilise (netIdJoueur);
		}
	}

	public static void Destruction (NetworkInstanceId netIdJoueur){
		if (null != onDestruction) {
			onDestruction (netIdJoueur);
		}
	}

	public static void XPGain (NetworkInstanceId netIdJoueur){
		if (null != onXPGain) {
			onXPGain (netIdJoueur);
		}
	}

	public static void RecoitDegat (NetworkInstanceId netIdJoueur){
		if (null != onRecoitDegat) {
			onRecoitDegat(netIdJoueur);
		}
	}

	public static void CardDeplacement (NetworkInstanceId netIdJoueur){
		if (null != onCardDeplacement) {
			onCardDeplacement (netIdJoueur);
		}
	}
}
