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


	public bool tester;

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

	private string onGUIPseudo;


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
	private void addJoueur(JoueurMinimalDTO joueurToAdd){
		if (isServer) {
			if (null == listJoueurs) {
				listJoueurs = new List<JoueurMinimalDTO> ();
			}

			listJoueurs.Add (joueurToAdd);

			//TODO chercher mode pour nb joueur
			if ((listJoueurs.Count >= 2 || tester) &&  !gameBegin) {
				gameBegin = true;

				//Remplissage de la main initial
				foreach (JoueurMinimalDTO joueurDTO in listJoueurs) {
					Joueur joueur = JoueurUtils.getJoueur (joueurDTO.netIdJoueur);

					if (null != joueur) {
						joueur.DeckConstruction.piocheDeckConstructionByServer (joueur.Main);
						joueur.DeckConstruction.piocheDeckConstructionByServer (joueur.Main);
					}
				}

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
				Joueur joueurTour = JoueurUtils.getJoueur (listJoueurs [indexPlayerPlaying].netIdJoueur);

				int phasePrecedente = phase;

				phase = FIN_TOUR;

				ActionEventManager.EventActionManager.CmdEndTurn (joueurTour.netId, phasePrecedente);

				GameObject goBtnLastPlayer = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);

				if (null != goBtnLastPlayer && null != goBtnLastPlayer.GetComponent<BoutonTour> ()) {
					BoutonTour boutonTour = goBtnLastPlayer.GetComponent<BoutonTour> ();
					boutonTour.setEtatBoutonServer (BoutonTour.enumEtatBouton.enAttente);
				}
				

				bool tourSupJoueur = 0 < CapaciteUtils.valeurAvecCapacite (0, joueurTour.CartePlaneteJoueur.containCapacityOfType (ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU), ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU);

				if (!tourSupJoueur) { //Pas de tour supplementaire

					if (indexPlayerPlaying < listJoueurs.Count - 1) {
						indexPlayerPlaying++;
					} else {
						indexPlayerPlaying = 0;
						nbTurn++;
					}

					this.idJoueurTour = listJoueurs [indexPlayerPlaying].netIdJoueur;
					joueurTour = JoueurUtils.getJoueur (listJoueurs [indexPlayerPlaying].netIdJoueur);
				}

				RpcAffichagePseudo (listJoueurs [indexPlayerPlaying].Pseudo);
			
				ActionEventManager.EventActionManager.CmdStartTurn (joueurTour.netId);

				bool perteTour = 0 > CapaciteUtils.valeurAvecCapacite (0, joueurTour.CartePlaneteJoueur.containCapacityOfType (ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU), ConstanteIdObjet.ID_CAPACITE_PERTE_TOUR_JEU);
				initTour(joueurTour);

				if (perteTour) {//Perte de tour
					progressStepServer(FIN_TOUR);
				} else {
					phase++;
					GameObject goBtnNewPlayer = NetworkServer.FindLocalObject (listJoueurs [indexPlayerPlaying].netIdBtnTour);

					if (null != goBtnNewPlayer && null != goBtnNewPlayer.GetComponent<BoutonTour> ()) {
						BoutonTour boutonTour = goBtnNewPlayer.GetComponent<BoutonTour> ();
						boutonTour.setEtatBoutonServer (BoutonTour.enumEtatBouton.terminerTour);
					}
				}
			}
		}
	}

	private void initTour(Joueur joueurInitTour){
		phase = DEBUT_TOUR;
		joueurInitTour.CmdProductionRessource ();
		RpcRemiseEnPlaceCarte (joueurInitTour.netId);
		joueurInitTour.DeckConstruction.piocheDeckConstructionByServer (joueurInitTour.Main);

	}

	[ClientRpc]
	public void RpcRemiseEnPlaceCarte(NetworkInstanceId netIdJoueur){
		List<CarteMetierAbstract> listCarteJoueur = CarteUtils.getListCarteJoueur (netIdJoueur);

		foreach (CarteMetierAbstract carteJoueur in listCarteJoueur) {
			if (carteJoueur is IAttaquer) {
				((IAttaquer)carteJoueur).AttaqueCeTour = false;
			}

			if (carteJoueur is IDefendre) {
				((IDefendre)carteJoueur).reinitDefenseSelectTour ();
			}
		}
			
		List<EmplacementAttaque> listEmplacementAttaqueOccuper = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementAttaque> (netIdJoueur);

		if (listEmplacementAttaqueOccuper.Count > 0) {
			List<EmplacementAtomsphereMetier> listEmplacementAtmosJoueurLibre = EmplacementUtils.getListEmplacementLibreJoueur <EmplacementAtomsphereMetier> (netIdJoueur);
				
			//On essaye d'abord de replacer les vaisseaux au bonne endroit
			if (listEmplacementAtmosJoueurLibre.Count > 0) {
				List<EmplacementAttaque> listEmplacementAttaqueToujoursOccuper = new List<EmplacementAttaque> (listEmplacementAttaqueOccuper);
				List<EmplacementAtomsphereMetier> listEmplacementAtmosToujoursLibre = new List<EmplacementAtomsphereMetier> (listEmplacementAtmosJoueurLibre);

				foreach (EmplacementAttaque emplacementAttaqueJoueur in listEmplacementAttaqueOccuper) {
					foreach (EmplacementAtomsphereMetier emplacementAtmosJoueur in listEmplacementAtmosJoueurLibre) {
						if (emplacementAttaqueJoueur.NumColonne == emplacementAtmosJoueur.NumColonne) {
							CarteConstructionMetierAbstract carteADeplacer = emplacementAttaqueJoueur.gameObject.GetComponentInChildren<CarteConstructionMetierAbstract> ();
							emplacementAtmosJoueur.putCard (carteADeplacer);
							listEmplacementAttaqueToujoursOccuper.Remove (emplacementAttaqueJoueur);
							listEmplacementAtmosToujoursLibre.Remove (emplacementAtmosJoueur);
							break;
						}
					}
				}


				listEmplacementAttaqueToujoursOccuper.Sort ((p1, p2) => p1.NumColonne.CompareTo (p2.NumColonne));
				listEmplacementAtmosToujoursLibre.Sort ((p1, p2) => p1.NumColonne.CompareTo (p2.NumColonne));
				while (0 < listEmplacementAttaqueToujoursOccuper.Count && 0 < listEmplacementAtmosToujoursLibre.Count) {
					CarteConstructionMetierAbstract carteADeplacer = listEmplacementAttaqueToujoursOccuper [0].gameObject.GetComponentInChildren<CarteConstructionMetierAbstract> ();
					listEmplacementAtmosToujoursLibre [0].putCard (carteADeplacer);
					listEmplacementAttaqueToujoursOccuper.RemoveAt (0);
					listEmplacementAtmosToujoursLibre.RemoveAt (0);
				}

				if (listEmplacementAttaqueToujoursOccuper.Count > 0) {
					foreach (EmplacementAttaque emplacementAVider   in listEmplacementAttaqueToujoursOccuper) {
						CarteConstructionMetierAbstract carteADeplacer = emplacementAVider.gameObject.GetComponentInChildren<CarteConstructionMetierAbstract> ();

						if (carteADeplacer is CarteVaisseauMetier) {
							((CarteVaisseauMetier)carteADeplacer).sacrificeCarte ();
						} else {
							//TODO
						}
					}
				}

				//On fait de même avec les emplacement de sol
				if (listEmplacementAtmosToujoursLibre.Count > 0) {
					List<EmplacementSolMetier> listEmplacementSolJoueur = EmplacementUtils.getListEmplacementOccuperJoueur<EmplacementSolMetier> (netIdJoueur);


					List<EmplacementSolMetier> listEmplacementSolAvecCarteVaisseau = new List<EmplacementSolMetier> (listEmplacementSolJoueur);
					List<EmplacementAtomsphereMetier> listEmplacementAtmosToujoursLibre2 = new List<EmplacementAtomsphereMetier> (listEmplacementAtmosJoueurLibre);

					foreach (EmplacementSolMetier emplacementSolJoueur in listEmplacementSolJoueur) {
						foreach (EmplacementAtomsphereMetier emplacementAtmosJoueur in listEmplacementAtmosJoueurLibre) {
							if (emplacementSolJoueur.NumColonne == emplacementAtmosJoueur.NumColonne) {
								CarteConstructionMetierAbstract carteADeplacer = emplacementSolJoueur.gameObject.GetComponentInChildren<CarteConstructionMetierAbstract> ();
								if (null != carteADeplacer && carteADeplacer is CarteVaisseauMetier) {
									emplacementAtmosJoueur.putCard (carteADeplacer);
									listEmplacementSolAvecCarteVaisseau.Remove (emplacementSolJoueur);
									listEmplacementAtmosToujoursLibre2.Remove (emplacementAtmosJoueur);
								} else {
									listEmplacementSolAvecCarteVaisseau.Remove (emplacementSolJoueur);
								}
									break;
							}
						}
					}


					listEmplacementSolAvecCarteVaisseau.Sort ((p1, p2) => p1.NumColonne.CompareTo (p2.NumColonne));
					listEmplacementAtmosToujoursLibre.Sort ((p1, p2) => p1.NumColonne.CompareTo (p2.NumColonne));
					while (0 < listEmplacementSolAvecCarteVaisseau.Count && 0 < listEmplacementAtmosToujoursLibre.Count) {
						CarteConstructionMetierAbstract carteADeplacer = listEmplacementSolAvecCarteVaisseau [0].gameObject.GetComponentInChildren<CarteConstructionMetierAbstract> ();
						listEmplacementAtmosToujoursLibre [0].putCard (carteADeplacer);
						listEmplacementSolAvecCarteVaisseau.RemoveAt (0);
						listEmplacementAtmosToujoursLibre.RemoveAt (0);
					}
				}

			}
		}
	}

	[ClientRpc]
	public void RpcAffichagePseudo(string pseudoJoueur){
		StartCoroutine(corroutineAffichagePseudo(pseudoJoueur));
	}
		
	private IEnumerator corroutineAffichagePseudo(string pseudoJoueur){
		onGUIPseudo = pseudoJoueur;
		yield return new WaitForSeconds (2);
		onGUIPseudo = "";
		yield return null;
	}


	public void OnGUI()	{
		if (null != onGUIPseudo && onGUIPseudo != "") {
			Rect rect = new Rect(0, 0, Screen.width, Screen.height / 4);
			GUI.TextField (rect, "Début du tour : " + onGUIPseudo);
		}
	}

	/************************Getter et Setter****************/
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
