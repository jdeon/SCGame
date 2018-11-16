using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TourJeuSystem : NetworkBehaviour {

	private static List<JoueurMinimalDTO> listJoueurs;

	private static int indexPlayerPlaying;

	//0 début tour, 1 placement carte, 2 attaque, -1 en attente
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


	}



	

}
