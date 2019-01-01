using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Joueur : NetworkBehaviour, IAvecCapacite {

	[SerializeField][SyncVar]
	private string pseudo = "Test";

	[SerializeField]
	private Mains main;


	private CartePlaneteMetier cartePlanetJoueur;

	[SerializeField]
	private RessourceMetier ressourceXP;

	[SerializeField]
	private RessourceMetier ressourceMetal;

	[SerializeField]
	private RessourceMetier ressourceCarburant;


	[SerializeField]
	private DeckConstructionMetier deckConstruction;

	private DeckConstructionMetier cimetiereConstruction;



	private CarteMetierAbstract carteSelectionne;

	private bool carteEnVisuel;

	private GameObject goPlateau;

	private List<CapaciteMetier> listCapacite = new List<CapaciteMetier>();

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
		main.init(netId);

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

		if (isServer) {
			//TODO rechercher autrement
			pseudo = "Pseudo" + GameObject.FindObjectsOfType<Joueur> ().Length;
		}
	}

	private void initPlateau (){
		string nomPlateau = transform.localPosition.z < 0 ? "Plateau1" : "Plateau2"; //TODO mettre en constante
		goPlateau = GameObject.Find(nomPlateau);

		EmplacementMetierAbstract[] tabEmplacement = goPlateau.GetComponentsInChildren<EmplacementMetierAbstract> ();

		//On met l'id du jour sur tous les emplacement
		for(int index = 0; index < tabEmplacement.Length; index++){
			tabEmplacement[index].IdJoueurPossesseur = this.netId;
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

	public void invoquerCarte(GameObject carteAInvoquer, int niveauInvocation, IConteneurCarte emplacementCible){

		NetworkServer.Spawn (carteAInvoquer);

		CarteMetierAbstract carteScript = carteAInvoquer.GetComponent<CarteMetierAbstract> ();

		if (carteScript is CarteConstructionMetierAbstract) {
			((CarteConstructionMetierAbstract)carteScript).NiveauActuel = niveauInvocation;
		}

		emplacementCible.putCard (carteScript);

		NetworkUtils.assignObjectToPlayer (carteScript, GetComponent<NetworkIdentity> ());
		byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteScript.getCarteDTORef ());


		if (carteScript is CarteConstructionMetierAbstract) {
			((CarteConstructionMetierAbstract)carteScript).RpcGenerate(carteRefData, NetworkInstanceId.Invalid);
		} //TODO carte amelioration


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

			//TODO risque de mettre mauvaise instance ID
			ressourceXP.init (netId);
			ressourceMetal.init (netId);
			ressourceCarburant.init (netId);

		}

		Debug.Log ("End RpcGeneratePlanete");
	}

	[Command]
	public void CmdProductionRessource(){
		//TODO XP?
		ressourceMetal.productionDeRessourceByServer();
		ressourceCarburant.productionDeRessourceByServer();
	}

	public int addRessource(string type, int nb){
		int ressourceAdded = 0;

		if (type == "Metal") {
			ressourceAdded = -nb > ressourceMetal.Stock ? nb : -ressourceMetal.Stock; //Cas ou le nombre est negatif et plus grand que le stock
			ressourceMetal.Stock += ressourceAdded;
		} else if (type == "Carburant") {
			ressourceAdded = -nb > RessourceCarburant.Stock ? nb : -RessourceCarburant.Stock; //Cas ou le nombre est negatif et plus grand que le stock
			RessourceCarburant.Stock += ressourceAdded;
		} else if (type == "XP") {
			ressourceAdded = -nb > RessourceXP.Stock ? nb : -RessourceXP.Stock; //Cas ou le nombre est negatif et plus grand que le stock
			RessourceXP.Stock += ressourceAdded;
		}


		return ressourceAdded;
	}


	/*********************************IAvecCapacite*********************/
	public void addCapacity (CapaciteMetier capaToAdd){
		listCapacite.Add (capaToAdd);
		//TODO recalculate visual
	}

	public void removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapacite) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapacite.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}

	public void capaciteFinTour (){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapacite) {
			bool existeEncore = capacite.endOfTurn ();
			if (!existeEncore) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listCapacite.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}

	public List<CapaciteMetier>  containCapacityOfType(int idTypCapacity){
		List<CapaciteMetier> listCapaciteResult = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listCapacite) {
			if (capacite.IdTypeCapacite == idTypCapacity) {
				listCapaciteResult.Add (capacite);
			}
		}
		return listCapaciteResult;
	}

	public bool containCapacityWithId (int idCapacityDTO){
		bool contain = false;

		foreach (CapaciteMetier capacite in listCapacite) {
			if (capacite.IdCapaciteProvenance == idCapacityDTO) {
				contain = true;
				break;
			}
		}
		return contain;
	}


	public string Pseudo {
		get {return pseudo;}
	}

	public Mains Main{
		get { return main;}
	}
		
	public RessourceMetier RessourceXP {
		get { return ressourceXP; }
	}

	public RessourceMetier RessourceMetal {
		get { return ressourceMetal; }
	}

	public RessourceMetier RessourceCarburant {
		get { return ressourceCarburant; }
	}

	public DeckConstructionMetier DeckConstruction {
		get {return deckConstruction; }
	}

	public CarteMetierAbstract CarteSelectionne {
		get { return carteSelectionne; }
		set { carteSelectionne = value; }
	}

	public bool CarteEnVisuel {
		get { return carteEnVisuel; }
		set { carteEnVisuel = value; }
	}

	public DeckConstructionMetier CimetiereConstruction {
		get { return cimetiereConstruction; }
	}

	public CartePlaneteMetier CartePlaneteJoueur{
		get { return cartePlanetJoueur;}
	}

	public GameObject GoPlateau {
		get { return goPlateau; }
	}
		
	public bool getIsLocalJoueur(){
		return isLocalPlayer;
	}
}
