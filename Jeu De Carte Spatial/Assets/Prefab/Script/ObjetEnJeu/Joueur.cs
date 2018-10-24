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
			deckConstruction.intiDeck ();
			deckConstruction.setJoueur (this);
		} else {
			transform.Find ("VueJoueur").gameObject.SetActive(false); //TODO créer en constante
		}
	}

	[Command]
	public void CmdTirerCarte(){
		GameObject carteTiree = deckConstruction.tirerCarte ();

		carteTiree.transform.SetParent(main.transform);

		int nbCarteEnMains = main.transform.childCount;

		carteTiree.transform.localPosition = new Vector3 (/*ConstanteInGame.coefPlane * */ carteTiree.transform.localScale.x * (nbCarteEnMains - .5f), 0, 0);
		carteTiree.transform.Rotate (new Vector3 (-60, 0) + main.transform.rotation.eulerAngles);

		NetworkServer.Spawn (carteTiree);

		CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();
		byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteConstructionScript.getCarteRef());
		RpcGenerate(carteTiree, carteConstructionScript.getCarteRef().GetType().ToString(), carteRefData);
	}

	[ClientRpc]
	public void RpcGenerate(GameObject goScript, string strTypeObject, byte[] dataObject)
	{
		CarteConstructionAbstractDTO carteRef = null;

		if (strTypeObject == "CarteVaisseauDTO") {
			carteRef = SerializeUtils.Deserialize<CarteVaisseauDTO> (dataObject);
		} else if (strTypeObject == "CarteDefenseDTO") {
			carteRef = SerializeUtils.Deserialize<CarteDefenseDTO> (dataObject);
		} else if (strTypeObject == "CarteBatimentDTO") {
			carteRef = SerializeUtils.Deserialize<CarteBatimentDTO> (dataObject);
		}

		CarteConstructionMetierAbstract carteConstructionScript = goScript.GetComponent<CarteConstructionMetierAbstract> ();
		carteConstructionScript.initCarteRef (carteRef);
		carteConstructionScript.generateGOCard ();
	}
}
