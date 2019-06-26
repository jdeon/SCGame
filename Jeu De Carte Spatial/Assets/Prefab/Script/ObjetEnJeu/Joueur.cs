using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Joueur : NetworkBehaviour {

	[SerializeField][SyncVar]
	private string pseudo = "Test";

	[SerializeField]
	private Mains main;

	[SerializeField]
	private RessourceMetier ressourceXP;

	[SerializeField]
	private RessourceMetier ressourceMetal;

	[SerializeField]
	private RessourceMetier ressourceCarburant;

	private CartePlaneteMetier cartePlanetJoueur;

	[SerializeField]
	private DeckConstructionMetier deckConstruction;

	[SerializeField]
	private DeckConstructionMetier cimetiereConstruction;


	private GameObject goPlateau;

	private CarteMetierAbstract carteSelectionne;

	private bool carteEnVisuel;

	void Start (){
		JoueurUtils.initAllJoueurDictionnary ();

		if (isServer) {
			//TODO rechercher autrement
			pseudo = "Pseudo" + GameObject.FindObjectsOfType<Joueur> ().Length;
		} else {
			CmdReInitAllJoueurDict ();
		}
			
		main.init(this);

		deckConstruction.intiDeck (this, isServer);
		cimetiereConstruction.intiDeck (this, isServer);

		ressourceXP.init (this);
		ressourceMetal.init (this);
		ressourceCarburant.init (this);

		if (isLocalPlayer) {
			CmdGenerateCardAlreadyLaid (this.netId);

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
		CmdInitPlateau (nomPlateau);
	}

	[Command]
	private void CmdInitPlateau (string nomPlateau){
		goPlateau = GameObject.Find(nomPlateau);

		EmplacementMetierAbstract[] tabEmplacement = goPlateau.GetComponentsInChildren<EmplacementMetierAbstract> ();

		//On met l'id du jour sur tous les emplacement
		for(int index = 0; index < tabEmplacement.Length; index++){
			tabEmplacement[index].IdJoueurPossesseur = this.netId;
		}

		CmdInitPlanete (this.netId, nomPlateau);
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
			cartePlanetJoueur.initPlaneteServer(this.netId, pseudo);
		}

		NetworkServer.Spawn (carteplaneteGO);

		RpcGeneratePlanete(carteplaneteGO, NetworkInstanceId.Invalid);

		Debug.Log ("End CmdInitPlanete");
	}

	[Command]
	private void CmdInitSystemeTour (){
		BoutonTour boutonTour = goPlateau.GetComponentInChildren<BoutonTour> ();
		NetworkUtils.assignObjectToPlayer (boutonTour.GetComponent<NetworkIdentity> (), GetComponent<NetworkIdentity> (), -1);

		TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();
		systemTour.addInSystemeTour (netId, pseudo, boutonTour.netId);
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

	[Command]
	public void CmdSyncIdSelectionnableCarte (NetworkInstanceId netIdCarte){
		CarteMetierAbstract carte = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarte, false);

		if (null != carte) {
			if (null == carte.IdISelectionnable || carte.IdISelectionnable <= 0) {
				carte.initIdSelection ();
			}

			carte.RpcInitIdSelectionnable (carte.IdISelectionnable);
		}
	}

	[Command]
	public void CmdCreateTask(NetworkInstanceId netIdSourceAction, NetworkInstanceId netIdJoueurSourceAction, int idSelectionCible, int typeAction, NetworkInstanceId netIdParentTask, bool createTaskBrother){
		ActionEventManager.EventActionManager.CreateTask (netIdSourceAction, netIdJoueurSourceAction, idSelectionCible, typeAction, netIdParentTask, createTaskBrother);
	}

	[Command]
	private void CmdReInitAllJoueurDict(){
		JoueurUtils.initAllJoueurDictionnary ();
		RpcReInitAllJoueurDict ();
	}

	[ClientRpc]
	private void RpcReInitAllJoueurDict(){
		JoueurUtils.initAllJoueurDictionnary ();
	}

	/******************Gestion Deck********************/
	[ClientRpc]
	public void RpcSyncCapaciteListDeck(byte[] listeCapaData, string type){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);
		DeckMetierAbstract deckCible = getDeckByType (type);

		if (null != deckCible) {
			deckCible.syncListCapacityFromServer (listCapacite);
		}
	}

	[ClientRpc]
	public void RpcInitDeckIdSelectionnable(int idSelectionnableServer, string type){
		DeckMetierAbstract deckCible = getDeckByType (type);

		if (null != deckCible) {
			deckCible.IdISelectionnable = idSelectionnableServer;
		}
	}

	private DeckMetierAbstract getDeckByType(string type){
		DeckMetierAbstract deck = null;
		if (type == "Construction") {
			deck = DeckConstruction;
		} else if (type == "Amelioration") {
			//TODO deck = DeckAmelioration;
		} 

		return deck;
	}


	/******************Gestion Ressource****************/
	[Command]
	public void CmdProductionRessource(){
		//TODO XP?
		ressourceMetal.Stock += ressourceMetal.ProductionWithCapacity;
		RpcSyncRessourceStockAndProd (ressourceMetal.TypeRessource, ressourceMetal.Production, ressourceMetal.Stock);

		ressourceCarburant.Stock += ressourceCarburant.ProductionWithCapacity;
		RpcSyncRessourceStockAndProd (ressourceCarburant.TypeRessource, ressourceCarburant.Production, ressourceCarburant.Stock);
	}

	[Command]
	public void CmdPayerRessource(string type, int nbRessourcePaye){
		RessourceMetier ressource = getRessourceByType (type);

		if (null != ressource) {
			ressource.Stock -= nbRessourcePaye;
			RpcSyncRessourceStockAndProd(ressource.TypeRessource,ressource.Production,ressource.Stock);
		}
	}


	public int addRessourceServer(string type, int nb){
		int ressourceAdded = 0;

		RessourceMetier ressource = getRessourceByType (type);

		if (null != ressource) {
			ressourceAdded = -nb > ressource.Stock ? nb : -ressource.Stock; //Cas ou le nombre est negatif et plus grand que le stock
			ressource.Stock += ressourceAdded;
			RpcSyncRessourceStockAndProd (ressource.TypeRessource, ressource.Production, ressource.Stock);
		} 
			
		return ressourceAdded;
	}

	[ClientRpc]
	public void RpcSyncRessourceStockAndProd(string type, int prod, int stock){
		RessourceMetier ressource = getRessourceByType (type);

		if (null != ressource) {
			ressource.Production = prod;
			ressource.Stock = stock;
			ressource.updateVisual ();
		}
	}

	[ClientRpc]
	public void RpcSyncCapaciteListRessource(byte[] listeCapaData, string type){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);

		RessourceMetier ressource = getRessourceByType (type);

		if (null != ressource) {
			ressource.syncListCapacityFromServer (listCapacite);
		}
	}

	[ClientRpc]
	public void RpcInitRessourceIdSelectionnable(int idSelectionnableServer, string type){
		RessourceMetier ressource = getRessourceByType (type);

		if (null != ressource) {
			ressource.IdISelectionnable = idSelectionnableServer;
		}
	}


	private RessourceMetier getRessourceByType(string type){
		RessourceMetier ressource = null;
		if (type == RessourceMetal.TypeRessource) {
			ressource = RessourceMetal;
		} else if (type == RessourceCarburant.TypeRessource) {
			ressource = RessourceCarburant;
		} else if (type == RessourceXP.TypeRessource) {
			ressource = RessourceXP;
		}

		return ressource;
	}


	/***************************************************/
	[ClientRpc]
	public void RpcInitMainIdSelectionnable(int idSelectionnableServer){
		if (null != main) {
			main.IdISelectionnable = idSelectionnableServer;
		}
	}

	[ClientRpc]
	public void RpcPutCardInHand(NetworkInstanceId netIdCarte){

		CarteMetierAbstract carte = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdCarte, true);
		if (null != main) {
			main.putCard(carte);
		}
	}

	/*******************Getter et setter***************/
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
		set { 
			if (null != carteSelectionne) {
				carteSelectionne.EtatSelectionnable = SelectionnableUtils.ETAT_NON_SELECTION;
			}

			carteSelectionne = value; 

			if (null != carteSelectionne) {
				carteSelectionne.EtatSelectionnable = SelectionnableUtils.ETAT_SELECTIONNE;
			} 
		}
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
