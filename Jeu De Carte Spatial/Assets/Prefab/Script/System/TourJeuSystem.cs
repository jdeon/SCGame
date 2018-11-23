using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TourJeuSystem : NetworkBehaviour {

	public static readonly int EN_ATTENTE = -1;
	public static readonly int DEBUT_TOUR = 0;
	public static readonly int PHASE_DEPLACEMENT = 1;
	public static readonly int PHASE_ATTAQUE = 2;
	public static readonly int PHASE_DEFENSE = 3;
	public static readonly int FIN_TOUR = 4;

	private static List<JoueurMinimalDTO> listJoueurs;

	private static int indexPlayerPlaying;


	private static int phase;

	private static int nbTurn;

	private static TourJeuSystemClientRpc transferRPC;

	public static void addJoueur(JoueurMinimalDTO joueur){

		if (null == listJoueurs) {
			listJoueurs = new List<JoueurMinimalDTO> ();
			transferRPC = new TourJeuSystemClientRpc ();
		}

		listJoueurs.Add (joueur);

		//TODO decommenter
		//if (listJoueurs.Count >= 2) {
			chooseFirstPlayer ();
		//}
	}

	public static void chooseFirstPlayer(){
		indexPlayerPlaying = Random.Range (0, listJoueurs.Count);
		nbTurn = 0;
		phase = 0;

		//TODO declencher pioche

		phase++;

		transferRPC.RpcReinitBoutonNewTour(listJoueurs[indexPlayerPlaying].netIdBtnTour);
	}

	public static void progressStep(int actionPlayer){
		//TODO
	}

	public static int getPhase(){
		return phase;
	}

	public static int getPhase(NetworkInstanceId idJoueur){
		int phaseJoueur = EN_ATTENTE;

		if (idJoueur == listJoueurs [indexPlayerPlaying].netIdJoueur) {
			phaseJoueur = getPhase();
		}

		return phaseJoueur;
	}
}
