using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public abstract class CarteMetierAbstract : NetworkBehaviour, IAvecCapacite, ISelectionnable {

	[SyncVar]
	protected string id;

	[SyncVar  (hook = "onChangeNetIdJoueur")]
	protected NetworkInstanceId idJoueurProprietaire;

	protected Joueur joueurProprietaire;

	protected List<CapaciteMetier> listEffetCapacite;

	protected GameObject panelGO;

	protected GameObject faceCarteGO;

	public int etatSelectionne;


	public abstract CarteDTO getCarteDTORef ();

	public abstract Color getColorCarte ();

	protected abstract void initId ();

	/**Retourne si l'init est faite*/
	public abstract bool initCarteRef (CarteDTO carteRef);

	public IConteneurCarte getConteneur (){
		return transform.GetComponentInParent<IConteneurCarte> ();
	}

	//public abstract string initCarte (); //Besoin carte Ref

	public virtual void OnMouseDown(){
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

	public void deplacerCarte(IConteneurCarte nouveauEmplacement, NetworkInstanceId netIdNouveauPossesseur){
		bool deplacementImpossible = 0 < CapaciteUtils.valeurAvecCapacite (0, listEffetCapacite, ConstanteIdObjet.ID_CAPACITE_ETAT_IMMOBILE);

		//TODO que faire pour deplacement vers la mains
		if (!deplacementImpossible && nouveauEmplacement is NetworkBehaviour) {
			ActionEventManager.EventActionManager.CmdCardDeplacement (joueurProprietaire.netId, this.netId, ((NetworkBehaviour) nouveauEmplacement).netId);

			nouveauEmplacement.putCard (this);

			if (netIdNouveauPossesseur != NetworkInstanceId.Invalid && netIdNouveauPossesseur != idJoueurProprietaire) {
				this.idJoueurProprietaire = netIdNouveauPossesseur;
			}
		}
	}

	public virtual void generateVisualCard(){
		string pseudo = joueurProprietaire.Pseudo;
		GameObject canvasGO = GameObject.Find("Canvas_" + pseudo);
		Text text;

		if (null == canvasGO) {
			canvasGO = new GameObject ("Canvas_" + pseudo);
			Canvas canvas = canvasGO.AddComponent<Canvas> ();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler> ();
			canvasGO.AddComponent<GraphicRaycaster> ();
			canvas.pixelPerfect = true;
		}

		//TODO doit on ne pas recreer sans cesse le panel?
			panelGO = new GameObject ("Panel_" + pseudo);
			panelGO.AddComponent<CanvasRenderer> ();
			panelGO.transform.SetParent (canvasGO.transform, false);

			Image i = panelGO.AddComponent<Image> ();
			i.sprite = ConstanteInGame.spriteBackgroundCarte;
			i.color = getColorCarte ();

			float height = Screen.height * 3 / 4;
			float width = height / 1.5f > Screen.width ? Screen.width : height / 1.5f;
			panelGO.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);

		/**
		(GameObject)Instantiate(Resources.Load("CarteConstructionGraphique"))
*/


		//TODO pour l'instant on ne fait apparaitre que le text de la carte
		/*CarteAbstractData carteRef = getCarteRef ();
		string strTextCarte = "Titre : " + carteRef.titreCarte;
		strTextCarte += "\nLibelle : " + carteRef.libelleCarte;
		strTextCarte += "\nCitation : \"" + carteRef.citationCarte + "\"";

		text.text = strTextCarte;*/
	}


	public virtual void generateGOCard(){

		transform.localScale = ConstanteInGame.tailleCarte;

		gameObject.AddComponent<BoxCollider> ().size = new Vector3 (10, .025f, 10);

		faceCarteGO = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//faceCarteGO = Instantiate<GameObject>(ConstanteInGame.planePrefab);
		faceCarteGO.name = "faceCarte_" + id;
		faceCarteGO.transform.SetParent (transform);
		faceCarteGO.transform.localScale = Vector3.one;
		faceCarteGO.transform.localPosition = Vector3.zero;
		faceCarteGO.transform.localRotation = Quaternion.identity;
		faceCarteGO.GetComponent<MeshRenderer> ().material.color = getColorCarte ();
		Debug.Log ("Creation de " + faceCarteGO.name);


		GameObject dosCarte = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject dosCarte =  Instantiate<GameObject>(ConstanteInGame.planePrefab);
		dosCarte.name = "dosCarte" + id;
		dosCarte.transform.SetParent (transform);
		dosCarte.transform.localScale = Vector3.one;
		dosCarte.transform.localRotation = Quaternion.identity;
		dosCarte.transform.Rotate( new Vector3(0,0,180));
		dosCarte.transform.localPosition = Vector3.zero;


		GameObject titre = new GameObject("Titre_" + id);
		//GameObject titre = Instantiate<GameObject>(ConstanteInGame.textPrefab);
		titre.transform.SetParent (faceCarteGO.transform);
		titre.transform.localPosition = new Vector3 (-4, 0.01f, 4.75f);
		titre.transform.localRotation = Quaternion.identity;
		titre.transform.Rotate(new Vector3(90,0,0));		//Le titre apparait face à z
		titre.transform.localScale = new Vector3(.5f,.5f,.5f);
		TextMesh txtTitre = titre.AddComponent<TextMesh> ();
		txtTitre.text = getCarteDTORef ().TitreCarte;
		txtTitre.color = Color.black;
		txtTitre.fontSize = 20;
		txtTitre.font = ConstanteInGame.fontChintzy;
		txtTitre.fontStyle = FontStyle.Bold;
		titre.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		GameObject image = GameObject.CreatePrimitive (PrimitiveType.Plane);
		//GameObject image = Instantiate<GameObject>( ConstanteInGame.planePrefab);
		image.name = "Image_" + id;
		image.transform.SetParent (faceCarteGO.transform);
		image.transform.localPosition = new Vector3 (0, 0.01f,2);
		image.transform.localRotation = Quaternion.identity;
		image.transform.Rotate (ConstanteInGame.rotationImage);
		image.transform.localScale = new Vector3(.9f,1,.25f);

		Material matImage = new Material(ConstanteInGame.shaderStandart);

		Sprite sprtImage = Resources.Load<Sprite>(getCarteDTORef().ImagePath);


		if (null == sprtImage) {
			Debug.Log (getCarteDTORef ().TitreCarte + " n'a pas d'image");
			sprtImage = ConstanteInGame.spriteTest;
		}

		matImage.SetTexture ("_MainTex", sprtImage.texture);
		image.GetComponent<Renderer> ().material = matImage;
	}
		

	/*********************************IAvecCapacite*********************/
	public void addCapacity (CapaciteMetier capaToAdd){
		listEffetCapacite.Add (capaToAdd);
		//TODO recalculate visual
	}

	public void removeLinkCardCapacity (NetworkInstanceId netIdCard){
		List<CapaciteMetier> capacitesToDelete = new List<CapaciteMetier> ();

		foreach (CapaciteMetier capacite in listEffetCapacite) {
			if (capacite.Reversible && capacite.IdCarteProvenance == netIdCard) {
				capacitesToDelete.Add (capacite);
			}
		}

		foreach (CapaciteMetier capaciteToDelete in capacitesToDelete) {
			listEffetCapacite.Remove(capaciteToDelete);
		}
		//TODO recalculate visual
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
		Joueur localJoueur = Joueur.getJoueurLocal ();
		if (this.etatSelectionne == 1 && null != localJoueur.PhaseChoixCible && !localJoueur.PhaseChoixCible.finChoix) {
			localJoueur.PhaseChoixCible.listCibleChoisi.Add (this);

		} else if (joueurProprietaire.CarteSelectionne == this) {
			joueurProprietaire.CarteSelectionne = null;	//On deselectionne au second click
		} else {
			joueurProprietaire.CarteSelectionne = this;
		}
	}

	public void miseEnBrillance(int etat){
		etatSelectionne = etat;
		//TODO mise en brillance
	}

	public int EtatSelectionnable {
		get { return etatSelectionne; }
	}

	/************************Hook***********************************/
	private void onChangeNetIdJoueur(NetworkInstanceId netIdJoueur){
		this.idJoueurProprietaire = netIdJoueur;
		joueurProprietaire = Joueur.getJoueur (netIdJoueur);
	}


	/********************************Dialogue serveur client**************/
	[ClientRpc]
	public void RpcDestroyClientCard(){
		Destroy (gameObject);
	}

	[Command]
	public void CmdSetJoueurProprietaire(NetworkInstanceId netIdJoueur){
		this.idJoueurProprietaire = netIdJoueur;
	}
		
	[ClientRpc]
	public void RpcSyncCapaciteList(byte[] listeCapaData){
		List<CapaciteMetier> listCapacite = SerializeUtils.Deserialize<List<CapaciteMetier>> (listeCapaData);
		if (null != listCapacite) {
			this.listEffetCapacite = listCapacite;
		}
	}

	/**********************************Getter Setter***************************/
	public string getId(){
		return id;
	}

	public void setJoueurProprietaireServer(NetworkInstanceId netIdJoueur){
		this.idJoueurProprietaire = netIdJoueur;
	}

	public Joueur getJoueurProprietaire(){
		return joueurProprietaire;
	}
}
