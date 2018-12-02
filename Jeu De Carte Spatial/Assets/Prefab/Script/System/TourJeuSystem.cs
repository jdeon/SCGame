using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * Cette class sera uniquement modifier sur le server pour être sure d'une synchro parfaite
 * */
public class TourJeuSystem : NetworkBehaviour {

	public static readonly int EN_ATTENTE = -1;
	public static readonly int DEBUT_TOUR = 1;
	public static readonly int PHASE_DEPLACEMENT = 2;
	public static readonly int PHASE_ATTAQUE = 3;
	public static readonly int PHASE_DEFENSE = 4;
	public static readonly int FIN_TOUR = 5;

	private List<JoueurMinimalDTO> listJoueurs;

	private int indexPlayerPlaying;

	[SyncVar]
	private int phase;

	[SyncVar]
	private int nbTurn;

	[SyncVar]
	private bool gameBegin;

	[SyncVar]
	private NetworkInstanceId idJoueurTour;


	public static TourJeuSystem getTourSystem(){
		return GameObject.FindObjectOfType<TourJeuSystem> ();
	}

	public void addInSystemeTour(NetworkInstanceId idNetworkJoueur, string pseudo, NetworkInstanceId idNetworkBouton){
		
		Debug.Log ("Begin CmdAddInSystemeTour");

		if (isServer) {
			JoueurMinimalDTO joueurMin = new JoueurMinimalDTO ();
			joueurMin.netIdJoueur = idNetworkJoueur;
			joueurMin.Pseudo = pseudo;
			joueurMin.netIdBtnTour = idNetworkBouton;

			addJoueur (joueurMin);
		}

		Debug.Log ("End CmdAddInSystemeTour");
	}


	/**
	 * Appeler sur server
	 * */
	private void addJoueur(JoueurMinimalDTO joueur){
		if (isServer) {
			if (null == listJoueurs) {
				listJoueurs = new List<JoueurMinimalDTO> ();
			}

			listJoueurs.Add (joueur);

			//TODO decommenter
			if (/*listJoueurs.Count >= 2 && */ !gameBegin) {
				chooseFirstPlayer ();
			}
		}
	}

	/**
	 * Appeler sur server
	 * */
	private void chooseFirstPlayer(){
		if (isServer) {
			indexPlayerPlaying = Random.Range (0, listJoueurs.Count);
			nbTurn = 0;
			phase = DEBUT_TOUR;
			gameBegin = true;

			//TODO declencher pioche

			phase++;

			this.idJoueurTour = listJoueurs [indexPlayerPlaying].netIdJoueur;
			GameObject goBtn = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);

			if (null != goBtn && null != goBtn.GetComponent<BoutonTour> ()) {
				BoutonTour boutonTour = goBtn.GetComponent<BoutonTour> ();
				boutonTour.setEtatBoutonServer(BoutonTour.enumEtatBouton.terminerTour);
			}
		}
	}

	public void progressStepServer(int actionPlayer){
		if (isServer) {
			if (actionPlayer == PHASE_ATTAQUE) {
				phase = PHASE_ATTAQUE;

				GameObject goBtnLastPlayer = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);
				if (null != goBtnLastPlayer && null != goBtnLastPlayer.GetComponent<BoutonTour> ()) {
					BoutonTour boutonTour = goBtnLastPlayer.GetComponent<BoutonTour> ();
					boutonTour.setEtatBoutonServer (BoutonTour.enumEtatBouton.terminerTour);
				}

			} else if (actionPlayer == FIN_TOUR) {
				phase = FIN_TOUR;
				//TODO appeler capciter de fin de tour

				GameObject goBtnLastPlayer = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);

				if (null != goBtnLastPlayer && null != goBtnLastPlayer.GetComponent<BoutonTour> ()) {
					BoutonTour boutonTour = goBtnLastPlayer.GetComponent<BoutonTour> ();
					boutonTour.setEtatBoutonServer (BoutonTour.enumEtatBouton.enAttente);
				}
				

				if (indexPlayerPlaying < listJoueurs.Count - 1) {
					indexPlayerPlaying++;
				} else {
					indexPlayerPlaying = 0;
					nbTurn++;
				}

				this.idJoueurTour = listJoueurs [indexPlayerPlaying].netIdJoueur;

				//TODO afficher "debut tour pseudo";
				phase = DEBUT_TOUR;
				//TODO appeler capaciter de debut de tour
				//TODO declencher pioche
				phase++;

				GameObject goBtnNewPlayer = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);

				if (null != goBtnNewPlayer && null != goBtnNewPlayer.GetComponent<BoutonTour> ()) {
					BoutonTour boutonTour = goBtnNewPlayer.GetComponent<BoutonTour> ();
					boutonTour.setEtatBoutonServer (BoutonTour.enumEtatBouton.terminerTour);
				}
			}
		}
	}

	public int getPhase(){
		return phase;
	}

	public int getPhase(NetworkInstanceId idJoueur){
		int phaseJoueur = EN_ATTENTE;

		if (idJoueur == this.idJoueurTour) {
			phaseJoueur = getPhase();
		}

		return phaseJoueur;
	}
}
