using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Joueur : NetworkBehaviour {

	public string pseudo = "Test"; //TODO rechercher autrement

	public CarteMetierAbstract carteSelectionne;

	public bool carteEnVisuel;

	public DeckConstructionMetier deckConstruction;

	public DeckConstructionMetier cimetiereConstruction;

	public CartePlaneteMetier cartePlanetJoueur;

	public GameObject main;

	public GameObject goPlateau;

	public static Joueur getJoueurLocal(){
		Joueur joueurResult = null;

		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();
	
		if (null != listJoueur && listJoueur.Length > 0) {
			foreach (Joueur joueur in listJoueur) {
				if (joueur.isLocalPlayer) {
					joueurResult = joueur;
					break;
				}
			}
		}

		return joueurResult;
	}

	public static Joueur getJoueur(NetworkInstanceId netIdJoueur){
		Joueur joueurResult = null;

		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();

		if (null != listJoueur && listJoueur.Length > 0) {
			foreach (Joueur joueur in listJoueur) {
				if (joueur.netId == netIdJoueur) {
					joueurResult = joueur;
					break;
				}
			}
		}

		return joueurResult;
	}

	void Start (){
		if (isLocalPlayer) {
			CmdGenerateCardAlreadyLaid (this.netId);

			CmdInitDeck ();
			deckConstruction.setClientNetIdJoueur (netId);

			initPlateau ();
			CmdInitSystemeTour();
		} else {
			string nomPlateau = transform.localPosition.z < 0 ? "Plateau1" : "Plateau2"; //TODO mettre en constante
			goPlateau = GameObject.Find(nomPlateau);
			transform.Find ("VueJoueur").gameObject.SetActive(false); //TODO créer en constante
		}
	}

	private void initPlateau (){
		string nomPlateau = transform.localPosition.z < 0 ? "Plateau1" : "Plateau2"; //TODO mettre en constante
		goPlateau = GameObject.Find(nomPlateau);

		EmplacementMetierAbstract[] tabEmplacement = goPlateau.GetComponentsInChildren<EmplacementMetierAbstract> ();

		//On met l'id du jour sur tous les emplacement
		for(int index = 0; index < tabEmplacement.Length; index++){
			tabEmplacement[index].setIdJoueurPossesseur(this.netId);
		}

		CmdInitPlanete (this.netId, nomPlateau);
	}

	[Command]
	private void CmdInitSystemeTour (){
		BoutonTour boutonTour = goPlateau.GetComponentInChildren<BoutonTour> ();
		NetworkUtils.assignObjectToPlayer (boutonTour, GetComponent<NetworkIdentity> ());

		TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();
		systemTour.addInSystemeTour (netId, pseudo, boutonTour.netId);
	}

	[Command]
	public void CmdInitDeck(){
		Debug.Log ("Begin CmdInitDeck");

		deckConstruction.intiDeck (this.netId);

		Debug.Log ("End CmdInitDeck");
	}

	[Command]
	public void CmdGenerateCardAlreadyLaid (NetworkInstanceId networkIdJoueur){
		Debug.Log ("Begin CmdGenerateCardAlreadyLaid");
		CarteMetierAbstract[] listeCarteDejaPose = GameObject.FindObjectsOfType<CarteMetierAbstract> ();

		if (null != listeCarteDejaPose) {
			for (int index = 0; index < listeCarteDejaPose.Length; index++) {
				CarteMetierAbstract carteDejaPosee = listeCarteDejaPose [index];
				if (null != carteDejaPosee) {
					//carteDejaPosee.generateGOCard ();

					if (carteDejaPosee is CartePlaneteMetier) {//pas besoin de serialisation pour les planete
						RpcGeneratePlanete (carteDejaPosee.gameObject, networkIdJoueur);
					} else if (carteDejaPosee is CarteConstructionMetierAbstract) {
						byte[] carteRefData = SerializeUtils.SerializeToByteArray (carteDejaPosee.getCarteDTORef ());
						((CarteConstructionMetierAbstract) carteDejaPosee).RpcGenerate (carteRefData, networkIdJoueur);
					} else {
						//TODO carteAmeliration
					}
				}
			}
		}

		Debug.Log ("End CmdGenerateCardAlreadyLaid");
	}

	[Command]
	public void CmdInitPlanete(NetworkInstanceId networkIdJoueur, string nomPlateau){
		Debug.Log ("Begin CmdInitPlanete");
		GameObject goPlateau = GameObject.Find(nomPlateau);

		GameObject carteplaneteGO = Instantiate<GameObject> (ConstanteInGame.cartePlanetePrefab);
		carteplaneteGO.transform.SetParent (goPlateau.transform);
		carteplaneteGO.transform.localPosition = new Vector3 (0, 0, -3);
		carteplaneteGO.transform.localRotation =Quaternion.identity;
		carteplaneteGO.transform.localScale =Vector3.one;

		cartePlanetJoueur = carteplaneteGO.GetComponent<CartePlaneteMetier> ();
		if (null != cartePlanetJoueur) {
			cartePlanetJoueur.initPlanete(this.netId, pseudo);
		}

		NetworkServer.Spawn (carteplaneteGO);

		RpcGeneratePlanete(carteplaneteGO, NetworkInstanceId.Invalid);

		Debug.Log ("End CmdInitPlanete");
	}

	[Command]
	public void CmdPiocheCarte(){
		Debug.Log ("command");
		deckConstruction.piocheDeckConstructionByServer (main);
	}

	/**
	 * Genere le visuel de la carte planete chez le client
	 * goScript : GameObject de prefab avec le script de la carte et quelque variable sync3
	 * networkIdJoueur : id du jour chez qui générer la carte, si NetworkInstanceId.Invalid alors générer chez tous le monde
	 * 
	 * */
	[ClientRpc]
	public void RpcGeneratePlanete (GameObject goScript, NetworkInstanceId networkIdJoueur)
	{
		Debug.Log ("Begin RpcGeneratePlanete");

		if (NetworkInstanceId.Invalid == networkIdJoueur || networkIdJoueur == this.netId) {

			CartePlaneteMetier cartePlaneteScript = goScript.GetComponent<CartePlaneteMetier> ();
			this.cartePlanetJoueur = cartePlaneteScript;
			cartePlaneteScript.generateGOCard ();

		}

		Debug.Log ("End RpcGeneratePlanete");
	}

	public bool getIsLocalJoueur(){
		return isLocalPlayer;
	}
}
