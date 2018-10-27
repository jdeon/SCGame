using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Joueur : NetworkBehaviour {

	public CarteMetierAbstract carteSelectionne;

	public bool carteEnVisuel;

	public DeckConstructionMetier deckConstruction;

	public GameObject main;

	public GameObject ligneSol;

	public GameObject ligneAtmosphere;

	public GameObject ligneAttaque;


	void Start (){
		if (isLocalPlayer) {
			CmdInitDeck ();
			deckConstruction.setJoueur (this);

			CmdGenerateCardAlreadyLaid (this.netId);
		} else {
			transform.Find ("VueJoueur").gameObject.SetActive(false); //TODO créer en constante
		}
	}

	[Command]
	public void CmdInitDeck(){
		Debug.Log ("Begin CmdInitDeck");

		deckConstruction.intiDeck ();

		Debug.Log ("End CmdInitDeck");
	}

	[Command]
	public void CmdGenerateCardAlreadyLaid (NetworkInstanceId networkIdJoueur){
		Debug.Log ("Begin CmdGenerateCardAlreadyLaid");
		CarteMetierAbstract[] listeCarteDejaPose = GameObject.FindObjectsOfType<CarteMetierAbstract> ();

		if (null != listeCarteDejaPose) {
			for (int index = 0; index < listeCarteDejaPose.Length; index++) {
				if (null != listeCarteDejaPose[index]) {
					listeCarteDejaPose [index].generateGOCard ();
					byte[] carteRefData = SerializeUtils.SerializeToByteArray(listeCarteDejaPose [index].getCarteDTORef());
					RpcGenerate(listeCarteDejaPose [index].gameObject, carteRefData, networkIdJoueur);
				}
			}
		}

		Debug.Log ("End CmdGenerateCardAlreadyLaid");
	}



	[Command]
	public void CmdTirerCarte(){
		Debug.Log ("command");
		Debug.Log ("taille deck : " + deckConstruction.getNbCarteRestante());

		GameObject carteTiree = deckConstruction.tirerCarte ();

		Debug.Log ("carteTiree : " + carteTiree);
		Debug.Log ("carteTiree : " + main);

		carteTiree.transform.SetParent(main.transform);

		int nbCarteEnMains = main.transform.childCount;

		carteTiree.transform.localPosition = new Vector3 (/*ConstanteInGame.coefPlane * */ carteTiree.transform.localScale.x * (nbCarteEnMains - .5f), 0, 0);
		carteTiree.transform.Rotate (new Vector3 (-60, 0) + main.transform.rotation.eulerAngles);

		NetworkServer.Spawn (carteTiree);

		CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();
		byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteConstructionScript.getCarteRef());
		RpcGenerate(carteTiree, carteRefData, NetworkInstanceId.Invalid);
	}

	/**
	 * Genere le visuel de la carte chez le client
	 * goScript : GameObject de prefab avec le script de la carte et quelque variable sync
	 * dataObject : carteRef sérialize en bytes
	 * networkIdJoueur : id du jour chez qui générer la carte, si NetworkInstanceId.Invalid alors générer chez tous le monde
	 * 
	 * */
	[ClientRpc]
	public void RpcGenerate(GameObject goScript, byte[] dataObject, NetworkInstanceId networkIdJoueur)
	{
		Debug.Log ("ClientRpc");

		if (NetworkInstanceId.Invalid == networkIdJoueur || networkIdJoueur == this.netId) {

			CarteConstructionDTO carteRef = null;

			carteRef = SerializeUtils.Deserialize<CarteConstructionDTO> (dataObject);

			CarteConstructionMetierAbstract carteConstructionScript = goScript.GetComponent<CarteConstructionMetierAbstract> ();
			carteConstructionScript.initCarte (carteRef);
			carteConstructionScript.generateGOCard ();
		}
	}

	public bool getIsLocalJoueur(){
		return isLocalPlayer;
	}
}
