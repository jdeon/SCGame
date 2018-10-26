﻿using System.Collections;
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
			deckConstruction.intiDeck ();
			deckConstruction.setJoueur (this);
		} else {
			transform.Find ("VueJoueur").gameObject.SetActive(false); //TODO créer en constante
		}
	}

	[Command]
	public void CmdTirerCarte(){
		Debug.Log ("command");

		GameObject carteTiree = deckConstruction.tirerCarte ();

		carteTiree.transform.SetParent(main.transform);

		int nbCarteEnMains = main.transform.childCount;

		carteTiree.transform.localPosition = new Vector3 (/*ConstanteInGame.coefPlane * */ carteTiree.transform.localScale.x * (nbCarteEnMains - .5f), 0, 0);
		carteTiree.transform.Rotate (new Vector3 (-60, 0) + main.transform.rotation.eulerAngles);

		NetworkServer.Spawn (carteTiree);

		CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();
		byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteConstructionScript.getCarteRef());
		RpcGenerate(carteTiree, carteRefData);
	}

	[ClientRpc]
	public void RpcGenerate(GameObject goScript, byte[] dataObject)
	{
		Debug.Log ("ClientRpc");
		CarteConstructionDTO carteRef = null;

		carteRef = SerializeUtils.Deserialize<CarteConstructionDTO> (dataObject);

		CarteConstructionMetierAbstract carteConstructionScript = goScript.GetComponent<CarteConstructionMetierAbstract> ();
		carteConstructionScript.initCarte (carteRef);
		carteConstructionScript.generateGOCard ();
	}

	public bool getIsLocalJoueur(){
		return isLocalPlayer;
	}
}