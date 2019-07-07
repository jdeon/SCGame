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

	/*protected GameObject paternCarteConstruction;

	protected GameObject paternRessourceCarbu;*/

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

	public int getCoutMetal(){
		int coutMetal = 0;

		//La construction n'est pas au niveau maximum
		if (NiveauActuel < carteRef.ListNiveau.Count) {
			//cout du prochain niveau
			coutMetal = CapaciteUtils.valeurAvecCapacite (carteRef.ListNiveau [NiveauActuel].Cout, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION);
		}
			
		return coutMetal;
	}

	public int getCoutMetal(int numLvl){
		int coutMetal = 0;

		//La construction n'est pas au niveau maximum
		if (numLvl < carteRef.ListNiveau.Count) {
			//cout du prochain niveau
			coutMetal = CapaciteUtils.valeurAvecCapacite (carteRef.ListNiveau [numLvl].Cout, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_COUT_CONSTRUCTION);
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

	//Affiche la carte si clique dessus
	public virtual void generateVisualCard() {
		if (!JoueurProprietaire.CarteEnVisuel) {
			base.generateVisualCard ();
			JoueurProprietaire.CarteEnVisuel = true;
			float height = panelGO.GetComponent<RectTransform> ().rect.height;
			float width = panelGO.GetComponent<RectTransform> ().rect.width;

			CarteConstructionDTO carteSource = getCarteRef ();
			int nbNiveau = carteSource.ListNiveau.Count;

			//On supprime le premier niveau s'il est vide
			if (nbNiveau > 1 && carteSource.ListNiveau [0].TitreNiveau == "") {
				nbNiveau--;
			}

			//TODO le joueur envoyé devrait être celui qui clique
			designCarte = new DesignCarteConstructionV2 (panelGO, height, width, nbNiveau,JoueurProprietaire);

			designCarte.setTitre (carteSource.TitreCarte);
			designCarte.setImage (Resources.Load<Sprite>(carteSource.ImagePath));

			designCarte.setMetal (carteSource.ListNiveau [0].Cout);//TODO passer par getCout(qui vérifie s'il y a des capacité malus au bonus vert ou roge)
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

				//On affiche le cout uniquement
				int cout = NiveauActuel < index + 1 ? niveau.Cout : 0;

				designCarte.setNiveau (premierNivCache ? index : index + 1, niveau.TitreNiveau, niveau.DescriptionNiveau, niveau.Cout);
			}

			//TODO calcul PA, PD, ...
			designCarte.setPA (0);
			designCarte.setPD (this.PV);

			/*paternCarteConstruction = (GameObject) Instantiate(Resources.Load("Graphique/CarteConstructionPatern"));
		paternCarteConstruction.transform.SetParent (panelGO.transform);
		paternCarteConstruction.transform.localPosition = Vector3.zero;
		paternCarteConstruction.GetComponent<RectTransform>().ForceUpdateRectTransforms();

		float height = panelGO.GetComponent<RectTransform>().rect.height;
		float width = panelGO.GetComponent<RectTransform>().rect.width;
		paternCarteConstruction.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		GameObject paternTitre = paternCarteConstruction.transform.Find ("Titre").gameObject;
		GameObject paternImage = paternCarteConstruction.transform.Find ("Image").gameObject;
		GameObject paternRessource = paternCarteConstruction.transform.Find ("Ressource").gameObject;
		GameObject paternRessourceMetal = paternRessource.transform.Find ("Metal").gameObject;
		GameObject paternRessourceNiveau = paternRessource.transform.Find ("NiveauActuel").gameObject;
		paternRessourceCarbu = paternRessource.transform.Find ("Carburant").gameObject;
		GameObject paternDescription = paternCarteConstruction.transform.Find ("Description").gameObject;
		GameObject paternCitation = paternCarteConstruction.transform.Find ("Citation").gameObject;
		GameObject paternNiveaux = paternCarteConstruction.transform.Find ("Niveaux").gameObject;
		GameObject paternUtilise = paternCarteConstruction.transform.Find ("Utilise").gameObject;

		CarteConstructionAbstractData carteRef = (CarteConstructionAbstractData) getCarteRef ();

		paternTitre.GetComponent<Text>().text = carteRef.titreCarte;
		//paternImage.GetComponent<Image> ().sprite = carteRef.image; //TODO carte Ref doit être un sprite
		paternRessourceMetal.GetComponentInChildren<Text>().text = "" + carteRef.listNiveau[0].cout; //TODO passer par getCout(qui vérifie s'il y a des capacité malus au bonus vert ou roge)
		paternRessourceNiveau.GetComponentInChildren<Text> ().text = "" + this.niveauActuel; //TODO rajouter niveau actuelle dans la carte

		paternRessourceCarbu.SetActive (false);

		paternDescription.GetComponent<Text> ().text = carteRef.libelleCarte;
		paternCitation.GetComponent<Text> ().text = "\"" + carteRef.citationCarte+ "\"";*/
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
		GameObject faceCarteGO = transform.Find("faceCarte_" + id).gameObject;

		GameObject ressource = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject ressource = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		ressource.name = "Ressource_" + id;
		ressource.transform.SetParent (faceCarteGO.transform);
		ressource.transform.localRotation = Quaternion.identity;
		ressource.transform.localPosition = new Vector3 (0, 0.01f,-0.5f);
		ressource.transform.localScale = new Vector3(.9f,1,.15f);
		ressource.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject metal = new GameObject("Metal_" + id);
		//GameObject metal = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		//metal.name = "Metal_" + getCarteRef ().idCarte;
		metal.transform.SetParent (ressource.transform);
		metal.transform.localPosition = new Vector3 (-4, .01f, 0);
		metal.transform.localRotation = Quaternion.identity;
		metal.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		metal.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtmetal = metal.AddComponent<TextMesh> ();
		txtmetal.text = "M-" + carteRef.ListNiveau[0].Cout;
		txtmetal.color = Color.black;
		txtmetal.fontSize = 20;
		txtmetal.font = ConstanteInGame.fontChintzy;
		txtmetal.anchor = TextAnchor.MiddleLeft;
		metal.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;


		GameObject niveau = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject niveau = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		niveau.name = "Niveau_" + id;
		niveau.transform.SetParent (ressource.transform);
		niveau.transform.localPosition = new Vector3 (0, 0.01f, 0);
		niveau.transform.localScale = new Vector3 (.25f, .75f, 1);
		niveau.transform.localRotation = Quaternion.identity;
		niveau.transform.Rotate (ConstanteInGame.rotationImage);

		Material matNiveau = new Material(ConstanteInGame.shaderStandart);
		matNiveau.SetTexture ("_MainTex", getSpriteNiveau(NiveauActuel).texture);
		niveau.GetComponent<Renderer> ().material = matNiveau;


		GameObject cadreListNiveaux = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject cadreListNiveaux = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		cadreListNiveaux.name = "CadreListNiv_" + id;
		cadreListNiveaux.transform.SetParent (faceCarteGO.transform);
		cadreListNiveaux.transform.localPosition = new Vector3 (0, 0.01f, -2.78f);
		cadreListNiveaux.transform.localRotation = Quaternion.identity;
		cadreListNiveaux.transform.localScale = new Vector3 (0.75f, 1f, 0.25f);
		cadreListNiveaux.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		cadreListNiveaux.AddComponent<ClickableCardPart> ().setCarteMere (this);
		BoxCollider collCadre = cadreListNiveaux.AddComponent<BoxCollider> ();
		collCadre.size = new Vector3 (10, .1f, 10);

		GameObject listNiveaux = new GameObject ("TxtListNiv_" + id);
		//GameObject listNiveaux = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		//listNiveaux.name = "TxtListNiv_" + getCarteRef ().idCarte;
		listNiveaux.transform.SetParent (cadreListNiveaux.transform);
		listNiveaux.transform.localPosition = new Vector3 (-4.5f,.01f, 0);
		listNiveaux.transform.localRotation = Quaternion.identity;
		listNiveaux.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		listNiveaux.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtNiveaux = listNiveaux.AddComponent<TextMesh> ();
		string textNiv = "";
		for(int index = 0 ; index < carteRef.ListNiveau.Count; index++){
			NiveauDTO niveauDTO = carteRef.ListNiveau[index];
			if (textNiv != "") {
				textNiv += "\n";
			}

			if (niveauDTO.TitreNiveau != "") {
				textNiv += niveauDTO.TitreNiveau;
			}
		}

		txtNiveaux.text = textNiv;
		txtNiveaux.anchor = TextAnchor.MiddleLeft;
		txtNiveaux.color = Color.black;
		txtNiveaux.fontSize = 15;
		txtNiveaux.font = ConstanteInGame.fontChintzy;
		listNiveaux.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;


		GameObject cadrePD = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject cadrePD = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		cadrePD.name = "CadrePD_" + id;
		cadrePD.transform.SetParent (faceCarteGO.transform);
		cadrePD.transform.localPosition = new Vector3 (2.75f, 0.01f,-4.5f);
		cadrePD.transform.localRotation = Quaternion.identity;
		cadrePD.transform.localScale = new Vector3(.25f,1,.05f);
		cadrePD.GetComponent<Renderer> ().material = ConstanteInGame.materialBackgroundCarte;

		GameObject pointDefence = new GameObject("TxtPD_" + id);
		//GameObject pointDefence = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		pointDefence.transform.SetParent (cadrePD.transform);
		pointDefence.transform.localPosition = new Vector3(0,.01f,0);
		pointDefence.transform.localRotation = Quaternion.identity;
		pointDefence.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		pointDefence.transform.localScale = new Vector3(.5f,1,1);
		TextMesh txtPD = pointDefence.AddComponent<TextMesh> ();
		txtPD.text = "Def-" + carteRef.PointVieMax;	//TODO modif pour PV reelle
		txtPD.color = Color.black;
		txtPD.fontSize = 60;
		txtPD.font = ConstanteInGame.fontChintzy;
		txtPD.anchor = TextAnchor.MiddleCenter;
		pointDefence.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;
	}

	private Sprite getSpriteNiveau(int niveau){
		Sprite result = null;

		switch (niveau) {	
		case 1: 
			result = ConstanteInGame.spriteLvl1;
			break;
		case 2: 
			result = ConstanteInGame.spriteLvl2;
			break;
		case 3: 
			result = ConstanteInGame.spriteLvl3;
			break;
		case 4: 
			result = ConstanteInGame.spriteLvl4;
			break;
		case 5: 
			result = ConstanteInGame.spriteLvl5;
			break;
		}

		return result;
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
			if (PV <= 0) {
				JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, sourceDegat.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DESTRUCTION_CARTE, netdTaskEvent, false); 
			}
		}

		return PV;
	}


	public void destruction (NetworkInstanceId netdTaskEvent){
		if (JoueurProprietaire.isServer) {
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
		if (this.getConteneur () is EmplacementMetierAbstract) {
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

	private List<CapaciteDTO> getListCapaciteToCall(NetworkInstanceId netIdJoueur, NetworkInstanceId netCarteSource,int idTypActionCapacite){
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
			for (int nivCapacity = 1; nivCapacity <= this.NiveauActuel; nivCapacity++) {
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
		set{niveauActuel = value;}
	}

	public int PV {
		get {
			return CapaciteUtils.valeurAvecCapacite (this.pv, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_MODIF_PV);
		}
		set{ pv = value; }
	}

}
