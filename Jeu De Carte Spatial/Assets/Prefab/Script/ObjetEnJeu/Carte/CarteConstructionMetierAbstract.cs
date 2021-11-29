using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class CarteConstructionMetierAbstract : CarteMetierAbstract, IVulnerable {

	protected static int sequenceId;

	[SyncVar]
	protected int niveauActuel;

	[SyncVar]
	protected int pv;

	protected CarteConstructionDTO carteRef;

	protected DesignCarteConstructionV2 designCarte;

	public string initCarte (CarteConstructionDTO initCarteRef, bool isServer){
		carteRef = initCarteRef;
		initId ();
		NiveauActuel = 1;
		PV = carteRef.PointVieMax;

		if (isServer) {
			initEvent ();
		}

		return id;
	}

	public override bool initCarteRef (CarteDTO carteRef){
		bool initDo = false;
		if (null == carteRef && carteRef is CarteConstructionDTO) {
			this.carteRef = (CarteConstructionDTO) carteRef;
			initDo = true;
		}

		return initDo;
	}

	public override void reinitDebutTour (){
		if (null != designCarte) {
			designCarte.reinitDebutTour ();
		}
	}

	public int getCoutMetal(){
		return getCoutMetal(niveauActuel);
	}

	public int getCoutMetal(int numLvl){
		int coutMetal = 0;

		//La construction n'est pas au niveau maximum
		if (numLvl > 0 && numLvl <= carteRef.ListNiveau.Count) {
			//cout du prochain niveau
			coutMetal = CapaciteUtils.valeurAvecCapacite (carteRef.ListNiveau [numLvl - 1].Cout, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION);
		}
			
		return coutMetal;
	}

	/**Renvoi le coup de la carte et de ses evolution construite*/
	public int getCoutMetalReelCarte (){
		int coutMetal = 0;

		//La construction n'est pas au niveau maximum
		for(int niv = 1 ; niv <= NiveauActuel; niv++) {
			//cout du prochain niveau
			coutMetal += carteRef.ListNiveau [niv].Cout;
		}

		return coutMetal;
	}

	public int getPVMax(){
		return CapaciteUtils.valeurAvecCapacite (carteRef.PointVieMax, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_PV_MAX);
	}

	[Command]
	public override void CmdPiocheCard(NetworkInstanceId netIdJoueurPioche){
		this.idJoueurProprietaire = netIdJoueurPioche;

		this.JoueurProprietaire.RpcPutCardInHand (this.netId);

		byte[] carteRefData = SerializeUtils.SerializeToByteArray(this.getCarteRef());
		this.RpcGenerate(carteRefData, NetworkInstanceId.Invalid);
	}

	[Command]
	public void CmdCreateEvolTask(NetworkInstanceId netIdCarteSource, NetworkInstanceId netIdJoueurSource, int nbNivEvol){
		EventTask eventTask =  ActionEventManager.EventActionManager.CreateTask (netIdCarteSource, netIdJoueurSource, IdISelectionnable,
				ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE, NetworkInstanceId.Invalid, false);
		eventTask.InfoComp = nbNivEvol;
	}

	public void evolutionCarte (int nbNiveau, NetworkInstanceId netIdTask, bool avecStockNiv){
		int evolutionRestante = nbNiveau;
		if (nbNiveau > 0) {
			RessourceMetier ressourceMetal = JoueurProprietaire.RessourceMetal;
			RessourceMetier ressourceXp = JoueurProprietaire.RessourceXP;

			while (nbNiveau > 0) {
				if (niveauActuel <= 5 && JoueurProprietaire.RessourceMetal.Stock >= getCoutMetal (niveauActuel + 1)
				    && (!avecStockNiv || JoueurProprietaire.RessourceXP.StockWithCapacity > 0)) {
					ressourceMetal.Stock -= getCoutMetal (niveauActuel + 1);

					if (avecStockNiv) {
						ressourceXp.Stock--;
					}

					niveauActuel++;
					nbNiveau--;
				} else {
					break;
				}
			}

			JoueurProprietaire.RpcSyncRessourceStockAndProd (ressourceMetal.TypeRessource, ressourceMetal.Production, ressourceMetal.Stock);
			JoueurProprietaire.RpcSyncRessourceStockAndProd (ressourceXp.TypeRessource, ressourceXp.Production, ressourceXp.Stock);
			//TODO null sur le serveur
			designCarte.setNiveauActuel (niveauActuel);

		} else if (nbNiveau < 0) {
			//Si perte de niveau, pas de consommation de ressource
			niveauActuel += nbNiveau;
			designCarte.setNiveauActuel (niveauActuel);
		}
	}

	[Command]
	public void CmdUseCapacityManuelle ( int numLvl, int indexCapaciteAppelee){
		CapaciteMannuelleDTO capaUsed;
		if (carteRef.ListNiveau.Count > numLvl
			&& carteRef.ListNiveau [numLvl-1].CapaciteManuelle.Count > indexCapaciteAppelee) {
			capaUsed = carteRef.ListNiveau [numLvl-1].CapaciteManuelle [indexCapaciteAppelee];
		} else {
			capaUsed = null;
		}

		if (null != capaUsed) {
			foreach (CapaciteDTO capaAppeler in capaUsed.CapaciteEffet) {
				CapaciteUtils.callCapacite (this, this, this, capaAppeler, this.NetIdJoueurPossesseur
					, ConstanteIdObjet.ID_CONDITION_ACTION_UTILISE, NetworkInstanceId.Invalid); 
			}
		}
	}

	//Affiche la carte si clique dessus
	public virtual void generateVisualCard() {
		if (!JoueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();

			if (null == designCarte) {
				float height = panelGO.GetComponent<RectTransform> ().rect.height;
				float width = panelGO.GetComponent<RectTransform> ().rect.width;

				CarteConstructionDTO carteSource = getCarteRef ();

				designCarte = new DesignCarteConstructionV2 (this, panelGO, height, width, JoueurUtils.getJoueurLocal ());

				designCarte.setTitre (carteSource.TitreCarte);
				designCarte.setImage (Resources.Load<Sprite> (carteSource.ImagePath));

				designCarte.setMetal (getCoutMetal(1));
				designCarte.setNiveauActuel (NiveauActuel);
				designCarte.setCarburant (0);
				//designCarte.setDescription ("Ceci est une description de la carte");
				//designCarte.setCitation ("Il était une fois une carte");

				bool premierNivCache = false;
				for (int index = 0; index < carteSource.ListNiveau.Count; index++) {
					NiveauDTO niveau = carteSource.ListNiveau [index];

					//ne rempie pas le premier titre s'il est vide
					if (index == 0 && niveau.TitreNiveau == "") {
						premierNivCache = true;
						continue;
					}
				}

				//TODO calcul PA, PD, ...
				designCarte.setPA (0);
				designCarte.setPD (this.PV);
			} else {
				designCarte.setMetal (getCoutMetal());
				designCarte.setNiveauActuel (NiveauActuel);
				designCarte.setPD (this.PV);
			}
		}
	}

	/**
	 * Genere le visuel de la carte chez le client
	 * goScript : GameObject de prefab avec le script de la carte et quelque variable sync
	 * dataObject : carteRef sérialize en bytes
	 * networkIdJoueur : id du jour chez qui générer la carte, si NetworkInstanceId.Invalid alors générer chez tous le monde
	 * 
	 * */
	[ClientRpc]
	public void RpcGenerate(byte[] dataObject, NetworkInstanceId networkIdJoueur)
	{
		Debug.Log ("ClientRpc");

		if (NetworkInstanceId.Invalid == networkIdJoueur || networkIdJoueur == this.netId) {

			CarteConstructionDTO carteRef = null;

			carteRef = SerializeUtils.Deserialize<CarteConstructionDTO> (dataObject);

			initCarte (carteRef, false);
			generateGOCard ();
			CmdAssignCard ();
		}
	}

	public virtual void generateGOCard(){
		base.generateGOCard ();
		GenerateCardUtils.generateConstructionPartCard (this, id, beanTextCarte);
	}

	protected override void updateVisuals (){
		//Update carte physique
		beanTextCarte.txtMetal.text = "M - " + getCoutMetal ();
		beanTextCarte.txtPointDefense.text = "Def - " + PV;
		beanTextCarte.goNiveau.GetComponent<Renderer> ().material.SetTexture ("_DetailAlbedoMap", GenerateCardUtils.getSpriteNiveau (NiveauActuel).texture);

		//Update carte virtuel
		designCarte.setPD (this.PV);
		designCarte.setMetal (getCoutMetal());
		designCarte.setNiveauActuel (NiveauActuel);
	
	}

	/***********************IVulnerable*****************/

	//Retourne PV restant
	public void recevoirAttaque (CarteMetierAbstract sourceDegat, NetworkInstanceId netdTaskEvent, bool attaqueSimultane){
		bool invulnerable = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_INVULNERABLE);

		if (!invulnerable) { //TODO calcul degat
			JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, sourceDegat.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netdTaskEvent, attaqueSimultane); 
		}
	}

	//Retourne PV restant
	public int recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat, NetworkInstanceId netdTaskEvent){
		bool invulnerable = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_INVULNERABLE);

		if (!invulnerable && nbDegat > 0) {
			PV -= nbDegat;
			RessourceUtils.gainXPDegat (nbDegat, sourceDegat.JoueurProprietaire);
			if (PV <= 0) {
				JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, sourceDegat.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE, netdTaskEvent, false); 
			}
		}

		return PV;
	}


	public void destruction (Joueur joueurSource ,NetworkInstanceId netdTaskEvent){
		if (JoueurProprietaire.isServer) {
			RessourceUtils.gainXPDestruction (NiveauActuel, joueurSource);
			CapaciteUtils.deleteEffectCapacityOfCard (this.netId);
			NetworkUtils.unassignObjectFromPlayer (GetComponent<NetworkIdentity> (), JoueurProprietaire .GetComponent<NetworkIdentity> (), -1);
			JoueurProprietaire.CimetiereConstruction.addCarte (this);
		}
	}

	/*****************ISelectionnable*****************/
	public override void onClick(){
		Joueur joueurLocal = JoueurUtils.getJoueurLocal ();

		if (null != joueurLocal) {
			TourJeuSystem systemTour = TourJeuSystem.getTourSystem ();

			//Si un joueur clique sur une carte capable d'attaquer puis sur une carte ennemie cela lance une attaque
			EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
			if (systemTour.getPhase (joueurLocal.netId) == TourJeuSystem.PHASE_ATTAQUE
				&& null != joueurLocal.CarteSelectionne && joueurLocal.CarteSelectionne.JoueurProprietaire != JoueurProprietaire
				&& joueurLocal.CarteSelectionne is IAttaquer && ((IAttaquer)joueurLocal.CarteSelectionne).isCapableAttaquer ()
				&&! (null != eventTask && eventTask is EventTaskChoixCible)) {//On ne peut attaquer si choix de defense en cours

				//TODO vérifier l'emplacement sol
				JoueurUtils.getJoueurLocal ().CmdCreateTask(joueurLocal.CarteSelectionne.netId, joueurLocal.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE, NetworkInstanceId.Invalid, false);
			} else {
				base.onClick ();
			}
		} else {
			base.onClick ();
		}
	}


	/************************************Envent catcher****/
	private void initEvent(){
		ActionEventManager.onStartTurn += useStartTurnCapacity;
		ActionEventManager.onPiocheConstruction += usePiocheConstructionPhaseCapacity;
		ActionEventManager.onFinPhaseAttaque += useEndPhaseAttaqueCapacity;
		ActionEventManager.onEndTurn += useEndTurnCapacity;

		ActionEventManager.onPoseConstruction += usePoseConstructionCapacity;
		ActionEventManager.onAttaque += useAttaqueCapacity;
		ActionEventManager.onDefense += useDefenseCapacity;
		ActionEventManager.onDestruction += useDestructionCapacity;
		ActionEventManager.onInvocation += useInvocationCapacity;
		ActionEventManager.onRecoitDegat += useRecoitDegatCapacity;
		ActionEventManager.onCardDeplacement += useDeplacementCapacity;
		ActionEventManager.onCardEvolution += useEvolutionCapacity;
	}


	public void useStartTurnCapacity(NetworkInstanceId netIdJoueur, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {

			List<CapaciteDTO> capaciteStartTurn = getListCapaciteToCall (netIdJoueur, netId, ConstanteIdObjet.ID_CONDITION_ACTION_DEBUT_TOUR);

			foreach (CapaciteDTO capacite in capaciteStartTurn) {
				CapaciteUtils.callCapacite (this, null, null, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_DEBUT_TOUR, netIdTaskEvent);
			}
		}
	}

	public void useEndPhaseAttaqueCapacity(NetworkInstanceId netIdJoueur, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteEndAttaque = getListCapaciteToCall (netIdJoueur, netId, ConstanteIdObjet.ID_CONDITION_ACTION_FIN_ATTAQUE);

			foreach (CapaciteDTO capacite in capaciteEndAttaque) {
				CapaciteUtils.callCapacite (this, null, null, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_FIN_ATTAQUE, netIdTaskEvent);
			}
		}
	}

	public void useEndTurnCapacity(NetworkInstanceId netIdJoueur, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteEndTurn = getListCapaciteToCall (netIdJoueur, netId, ConstanteIdObjet.ID_CONDITION_ACTION_FIN_TOUR);

			foreach (CapaciteDTO capacite in capaciteEndTurn) {
				CapaciteUtils.callCapacite (this, null, null, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_FIN_TOUR, netIdTaskEvent);
			}
		}
	}


	public void usePiocheConstructionPhaseCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (CarteUtils.checkCarteActive(this) && 
			(this.getConteneur () is EmplacementMetierAbstract || carteSourceAction.netId == this.netId)) {
			List<CapaciteDTO> capacitePicheConstr = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION);

			foreach (CapaciteDTO capacite in capacitePicheConstr) {
				CapaciteUtils.callCapacite (this, null, null, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION, netIdTaskEvent);
			}
		}
	}

	public void usePoseConstructionCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (CarteUtils.checkCarteActive(this) && 
			(this.getConteneur () is EmplacementMetierAbstract || carteSourceAction.netId == this.netId)) {
			List<CapaciteDTO> capacitePoseConstruction = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION);

			foreach (CapaciteDTO capacite in capacitePoseConstruction) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_POSE_CONSTRUCTION, netIdTaskEvent);
			}
		}
	}

	public void useAttaqueCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteAttaque = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE);

			foreach (CapaciteDTO capacite in capaciteAttaque) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_ATTAQUE, netIdTaskEvent);
			}
		}
	}

	public void useDefenseCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteDefense = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND);

			foreach (CapaciteDTO capacite in capaciteDefense) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND, netIdTaskEvent);
			}
		}
	}

	public void useDestructionCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteDestruction = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE);

			foreach (CapaciteDTO capacite in capaciteDestruction) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE, netIdTaskEvent);
			}
		}
	}

	public void useInvocationCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {//TODO uniquement carte en jeu affecte?
			List<CapaciteDTO> capaciteInvocation = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION);

			foreach (CapaciteDTO capacite in capaciteInvocation) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_INVOCATION, netIdTaskEvent);
			}
		}
	}

	public void useRecoitDegatCapacity(NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteDegatRecu = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT);

			foreach (CapaciteDTO capacite in capaciteDegatRecu) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_RECOIT_DEGAT, netIdTaskEvent);
			}
		}
	}

	public void useDeplacementCapacity (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (cible is EmplacementAttaque && this.getConteneur () is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteDeplacementAttaque = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE);

			foreach (CapaciteDTO capacite in capaciteDeplacementAttaque) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE, netIdTaskEvent);
			}
		} else if (cible is EmplacementMetierAbstract) {
			List<CapaciteDTO> capaciteDeplacementAttaque = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART);

			foreach (CapaciteDTO capacite in capaciteDeplacementAttaque) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART, netIdTaskEvent);
			}
		}
	}

	public void useEvolutionCapacity (NetworkInstanceId netIdJoueur, CarteMetierAbstract carteSourceAction, ISelectionnable cible, NetworkInstanceId netIdTaskEvent){
		if (this.getConteneur () is EmplacementMetierAbstract || this == cible) {//TODO uniquement carte en jeu affecte?
			EventTask eventTask = ConvertUtils.convertNetIdToScript<EventTask> (netIdTaskEvent, isLocalPlayer);

			//Si c'est cette carte qui évolue, on lui attribut ses futur niveau
			int nbNivCapaTeste = this.IdISelectionnable == cible.IdISelectionnable ? this.NiveauActuel + eventTask.InfoComp : this.NiveauActuel;
			List<CapaciteDTO> capacitesEvolution = getListCapaciteToCall (netIdJoueur, carteSourceAction.netId, nbNivCapaTeste, ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE);
			foreach (CapaciteDTO capacite in capacitesEvolution) {
				CapaciteUtils.callCapacite (this, carteSourceAction, cible, capacite, netIdJoueur, ConstanteIdObjet.ID_CONDITION_ACTION_EVOLUTION_CARTE, netIdTaskEvent);
			}
		}
	}

	private List<CapaciteDTO> getListCapaciteToCall(NetworkInstanceId netIdJoueur, NetworkInstanceId netCarteSource, int idTypActionCapacite){
		return getListCapaciteToCall (netIdJoueur, netCarteSource, this.NiveauActuel, idTypActionCapacite);
	}


	private List<CapaciteDTO> getListCapaciteToCall(NetworkInstanceId netIdJoueur, NetworkInstanceId netCarteSource, int nivCard, int idTypActionCapacite){
		List<CapaciteDTO> capaciteToCall = new List<CapaciteDTO> ();

		int valeurIncapacite = 0;
		if( null != listEffetCapacite){
			foreach(CapaciteMetier capaciteCourante in listEffetCapacite){
				if(capaciteCourante.isActif() && capaciteCourante.IdTypeCapacite.Equals (ConstanteIdObjet.ID_CAPACITE_ETAT_SANS_EFFET)){
					valeurIncapacite = capaciteCourante.getNewValue (valeurIncapacite);
				} 
			}
		}

		if (valeurIncapacite == 0) {
			for (int nivCapacity = 1; nivCapacity <= nivCard && nivCapacity < carteRef.ListNiveau.Count; nivCapacity++) {
				foreach (CapaciteDTO capacite in carteRef.ListNiveau[nivCapacity-1].Capacite) {
					if (CapaciteUtils.isCapaciteCall (capacite, idTypActionCapacite, netIdJoueur == this.idJoueurProprietaire, this.netId == netCarteSource)) {
						capaciteToCall.Add (capacite);
					}
				}
			}
		}

		return capaciteToCall;
	}

	/******************************Getter et Setter ************************/
	public CarteConstructionDTO getCarteRef ()
	{
		return carteRef;
	}

	public override CarteDTO getCarteDTORef ()
	{
		return carteRef;
	}

	public bool OnBoard { 
		get{ return (getConteneur() is EmplacementMetierAbstract); }
	}

	public int NiveauActuel {
		get{
			return CapaciteUtils.valeurAvecCapacite (this.niveauActuel, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_LVL); 
		}
		set{
			niveauActuel = value;
		}
	}

	public int PV {
		get {
			return CapaciteUtils.valeurAvecCapacite (this.pv, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_PV);
		}
		set{ pv = value; }
	}

}
