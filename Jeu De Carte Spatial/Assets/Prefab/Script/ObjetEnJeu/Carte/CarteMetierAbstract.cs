using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public abstract class CarteMetierAbstract : NetworkBehaviour, IAvecCapacite, ISelectionnable {

	[SyncVar]
	protected string id;

	[SyncVar]
	protected NetworkInstanceId idJoueurProprietaire;

	protected List<CapaciteMetier> listEffetCapacite = new List<CapaciteMetier> ();

	protected GameObject panelGO;

	protected GameObject faceCarteGO;

	protected TextesCarteBean beanTextCarte;

	[SerializeField]
	protected int idSelectionnable;

	protected int etatSelectionne;


	public abstract CarteDTO getCarteDTORef ();

	protected abstract void initId ();

	public abstract void reinitDebutTour ();

	/**Retourne si l'init est faite*/
	public abstract bool initCarteRef (CarteDTO carteRef);

	public void Start(){
		if (isServer) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;
			RpcInitIdSelectionnable (idSelectionnable);
		} else if (null == idSelectionnable || idSelectionnable <= 0) {
			JoueurUtils.getJoueurLocal().CmdSyncIdSelectionnableCarte (netId);
		}
	}

	void OnMouseOver()
	{
		if (!JoueurUtils.getJoueur(idJoueurProprietaire).CarteEnVisuel) {
			EtatSelectionnable = SelectionnableUtils.ETAT_MOUSE_OVER;
		}
	}

	void OnMouseExit()
	{
		if (!JoueurUtils.getJoueur (idJoueurProprietaire).CarteEnVisuel) {
			EtatSelectionnable = SelectionnableUtils.ETAT_RETOUR_ATTIERE;
		}
	}


	public IConteneurCarte getConteneur (){
		if (CarteUtils.checkCarteActive(this)){
			return transform.GetComponentInParent<IConteneurCarte> ();
		} else {
			return null;
		}
	}

	//public abstract string initCarte (); //Besoin carte Ref

	public virtual void OnMouseDown(){
		if (!JoueurUtils.getJoueur (idJoueurProprietaire).CarteEnVisuel) {
			if (this is CartePlaneteMetier) {
				((CartePlaneteMetier)this).onClick ();
			} else if (this is CarteBatimentMetier) {
				((CarteBatimentMetier)this).onClick ();
			} else if (this is CarteDefenseMetier) {
				((CarteDefenseMetier)this).onClick ();
			} else if (this is CarteVaisseauMetier) {
				((CarteVaisseauMetier)this).onClick ();
			} 
		}
	}

	[Command]
	protected void CmdAssignCard(){
		NetworkUtils.assignObjectToPlayer (GetComponent<NetworkIdentity> (), JoueurProprietaire.GetComponent<NetworkIdentity> (), .1f);
	}

	[Command]
	public abstract void CmdPiocheCard (NetworkInstanceId netIdJoueurSourceAction);


	public void deplacerCarte(IConteneurCarte nouveauEmplacement, NetworkInstanceId netIdNouveauPossesseur, NetworkInstanceId netIdTaskEvent){

		//TODO que faire pour deplacement vers la mains
		if (isDeplacable()) {

			if (nouveauEmplacement is EmplacementMetierAbstract) {
				((EmplacementMetierAbstract) nouveauEmplacement).putCard (this, this.getConteneur () is Mains, netIdTaskEvent);
			} else if (nouveauEmplacement is ISelectionnable) {
				JoueurUtils.getJoueurLocal ().CmdCreateTask (this.netId, this.idJoueurProprietaire, ((ISelectionnable) nouveauEmplacement).IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_DEPLACEMENT_STANDART, NetworkInstanceId.Invalid, false);
			}else if (!isServer) {
				//TODO bon comportement si emplacement pa sselectionnable?
				nouveauEmplacement.putCard (this);
			}

			if (netIdNouveauPossesseur != NetworkInstanceId.Invalid && netIdNouveauPossesseur != idJoueurProprietaire) {
				this.idJoueurProprietaire = netIdNouveauPossesseur;
			}
		}
	}

	public bool isDeplacable(){
		return 0 >= CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_IMMOBILE);
	}

	public virtual void generateVisualCard(){
		JoueurProprietaire.CarteEnVisuel = true;
		if (null == panelGO) {
			string pseudo = JoueurProprietaire.Pseudo;
			GameObject canvasGO = UIUtils.getCanvas ();
			Text text;

			panelGO = new GameObject ("Panel_" + pseudo + "_Carte_" + id);
			panelGO.AddComponent<CanvasRenderer> ();
			panelGO.transform.SetParent (canvasGO.transform, false);

			Image i = panelGO.AddComponent<Image> ();
			i.sprite = UIUtils.dictionnaryOfCardSprite[CarteUtils.getTypeCard(this)][UIUtils.KEY_FOND];

			panelGO.GetComponent<RectTransform> ().sizeDelta = UIUtils.getUICardSize ();

			/**
		(GameObject)Instantiate(Resources.Load("CarteConstructionGraphique"))
*/


			//TODO pour l'instant on ne fait apparaitre que le text de la carte
			/*CarteAbstractData carteRef = getCarteRef ();
		string strTextCarte = "Titre : " + carteRef.titreCarte;
		strTextCarte += "\nLibelle : " + carteRef.libelleCarte;
		strTextCarte += "\nCitation : \"" + carteRef.citationCarte + "\"";

		text.text = strTextCarte;*/
		} else {
			panelGO.SetActive (true);
		}
	}


	public virtual void generateGOCard(){

		beanTextCarte = GenerateCardUtils.generateCardBase (this, this.id);
		faceCarteGO = beanTextCarte.goFaceCarte;
	}

	protected abstract void updateVisuals ();
		

	/*********************************IAvecCapacite*********************/
	public void addCapacity (CapaciteMetier capaToAdd){
		listEffetCapacite.Add (capaToAdd);
	}

	public int removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listEffetCapacite) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listEffetCapacite.Remove(capaciteToDelete);
		}

		if (capacitesToDelete.Count > 0) {
			synchroniseListCapacite ();
		}

		return capacitesToDelete.Count;
	}

	public void capaciteFinTour (){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listEffetCapacite) {
			bool existeEncore = capacite.endOfTurn ();
			if (!existeEncore) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listEffetCapacite.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
	}
		
	public List<CapaciteMetier> containCapacityOfType(int idTypCapacity){
		List<CapaciteMetier> listCapacite = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listEffetCapacite) {
			if (capacite.IdTypeCapacite == idTypCapacity) {
				listCapacite.Add (capacite);
			}
		}
		return listCapacite;
	}

	public bool containCapacityWithId (int idCapacityDTO){
		bool contain = false;

		foreach (CapaciteMetier capacite in listEffetCapacite) {
			if (capacite.IdCapaciteProvenance == idCapacityDTO) {
				contain = true;
				break;
			}
		}
		return contain;
	}

	public void synchroniseListCapacite (){
		byte[] listeCapaData = SerializeUtils.SerializeToByteArray(this.listEffetCapacite);
		RpcSyncCapaciteList (listeCapaData);
	}

	/*******************ISelectionnable****************/
	public virtual void onClick (){
		EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
		if (this.etatSelectionne == 1 && null != eventTask && eventTask is EventTaskChoixCible) {
			((EventTaskChoixCible) eventTask).ListCibleChoisie.Add (this);

		} else if (JoueurProprietaire.CarteSelectionne == this) {
			JoueurProprietaire.CarteSelectionne = null;	//On deselectionne au second click
		} else {
			JoueurProprietaire.CarteSelectionne = this;
		}
	}

	[ClientRpc]
	public void RpcSetEtatSelectionPlayer(NetworkInstanceId netIdPlayer, int etat){
		if (netIdPlayer == NetworkInstanceId.Invalid || JoueurUtils.getJoueurLocal ().netId == netIdPlayer) {
			EtatSelectionnable = etat;
		}
	}

	public void initIdSelection(){
		if (null == idSelectionnable || idSelectionnable <= 0) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;
		}
	}

	public int IdISelectionnable {
		get { return idSelectionnable; }
	}

	public int EtatSelectionnable {
		get { return etatSelectionne; }
		set {
			if (value == SelectionnableUtils.ETAT_RETOUR_ATTIERE) {
				SelectionnableUtils.miseEnBrillance (etatSelectionne, transform);
			} else {
				SelectionnableUtils.miseEnBrillance (value, transform);
				if (value != SelectionnableUtils.ETAT_MOUSE_OVER) {
					etatSelectionne = value;
				}
			}
		}
	}


	public NetworkInstanceId Possesseur { 
		get { return NetIdJoueurPossesseur; }
	}

	/********************************Dialogue serveur client**************/
	[ClientRpc]
	public void RpcDestroyClientCard(){
		Destroy (gameObject);
	}
		
	[ClientRpc]
	public void RpcSyncCapaciteList(byte[] listeCapaData){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);
		if (null != listCapacite) {
			this.listEffetCapacite = listCapacite;
		}

		updateVisuals ();
	}

	[ClientRpc]
	public void RpcInitIdSelectionnable(int idSelectionnableFromServer){
		this.idSelectionnable = idSelectionnableFromServer;
	}

	[ClientRpc]
	public void RpcSyncNetIdJoueur(NetworkInstanceId netIdJoueur){
		this.NetIdJoueurPossesseur = netIdJoueur;
	}

	[Command]
	public void CmdChangeParent (NetworkInstanceId netIdParent, string pathParent){
		NetworkBehaviour scriptParent = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (netIdParent, false);

		Transform trfParent;

		if (null != scriptParent && null != pathParent) {
			trfParent = scriptParent.transform.Find (pathParent);
		} else {
			trfParent = null;
		}

		transform.parent = trfParent;
		//transform.localScale = Vector3.one;  
		transform.localScale = Vector3.Scale(ConstanteInGame.tailleCarte, CarteUtils.inverseVector(CarteUtils.getParentScale (transform)));
		RpcChangeParentOtherClient (netIdParent, pathParent);

	}

	[ClientRpc]
	public void RpcChangeParentOtherClient (NetworkInstanceId netIdParent, string pathParent){

		if (! this.JoueurProprietaire.isLocalPlayer) {
			NetworkBehaviour scriptParent = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (netIdParent, true);

			Transform trfParent;

			if (null != scriptParent && null != pathParent) {
				trfParent = scriptParent.transform.Find (pathParent);
			} else {
				trfParent = null;
			}

			transform.parent = trfParent;
			//transform.localScale = Vector3.one;
			transform.localScale = Vector3.Scale(ConstanteInGame.tailleCarte, CarteUtils.inverseVector(CarteUtils.getParentScale (transform)));
		}
	}

	/**********************************Getter Setter***************************/
	public string getId(){
		return id;
	}

	public NetworkInstanceId NetIdJoueurPossesseur {
		get{ return this.idJoueurProprietaire; }
		set {
			Joueur joueur = JoueurUtils.getJoueur (value);
				if (joueur.isServer) {
				this.idJoueurProprietaire = value;
				//RpcSyncNetIdJoueur (idJoueurProprietaire);
			}
		}
	}


	public Joueur JoueurProprietaire {
		get { return JoueurUtils.getJoueur (this.idJoueurProprietaire); }
	}
}
