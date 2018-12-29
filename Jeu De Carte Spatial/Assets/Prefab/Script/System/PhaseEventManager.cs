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

	public static void PoseCarte (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
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

	public static void Attaque (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onAttaque) {
			onAttaque (netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void Defense (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onDefense) {
			onDefense (netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void Utilise (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onUtilise) {
			onUtilise (netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void Destruction (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onDestruction) {
			onDestruction (netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void XPGain (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onXPGain) {
			onXPGain (netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void RecoitDegat (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onRecoitDegat) {
			onRecoitDegat(netIdJoueur, carteOrigineAction, cible);
		}
	}

	public static void CardDeplacement (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteOrigineAction, ISelectionnable cible){
		if (null != onCardDeplacement) {
			onCardDeplacement (netIdJoueur, carteOrigineAction, cible);
		}
	}
}
