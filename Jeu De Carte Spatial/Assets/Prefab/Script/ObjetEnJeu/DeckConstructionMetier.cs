using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeckConstructionMetier : DeckMetierAbstract {

	[SerializeField]
	private DeckConstructionData deckContructionRef;

	public override void intiDeck (NetworkInstanceId joueurNetId){
		this.netIdJoueur = joueurNetId;

		List<CarteConstructionAbstractData> listCarteConstuctionData = deckContructionRef.listeCarte;
		foreach (CarteConstructionAbstractData carteConstructionData in listCarteConstuctionData) {
			GameObject carteGO = convertDataToGO (carteConstructionData);
			carteGO.transform.SetParent (transform);

			int cartePlace = Mathf.FloorToInt (Random.Range (0, transform.childCount));
			carteGO.transform.SetSiblingIndex (cartePlace);

		}
	}

	public void piocheDeckConstructionByServer(GameObject main){
		if (joueurProprietaire.isServer) {
			Debug.Log ("taille deck : " + getNbCarteRestante());

			GameObject carteTiree = tirerCarte ();

			Debug.Log ("carteTiree : " + carteTiree);
			Debug.Log ("carteTiree : " + main);

			carteTiree.transform.SetParent(main.transform);

			int nbCarteEnMains = main.transform.childCount;

			carteTiree.transform.localPosition = new Vector3 (/*ConstanteInGame.coefPlane * */ carteTiree.transform.localScale.x * (nbCarteEnMains - .5f), 0, 0);
			carteTiree.transform.Rotate (new Vector3 (-60, 0) + main.transform.rotation.eulerAngles);

			NetworkServer.Spawn (carteTiree);

			CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();

			NetworkUtils.assignObjectToPlayer (carteConstructionScript, joueurProprietaire.GetComponent<NetworkIdentity> ());
			byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteConstructionScript.getCarteRef());
			carteConstructionScript.RpcGenerate(carteRefData, NetworkInstanceId.Invalid);
		}
	}

	/**
	 * Appeler par le server
	 * */
	public override GameObject tirerCarte(){
		Debug.Log ("Begin tirerCarte()");

		int indexNextCarte = 0;
		GameObject cartePioche = null;

		for (indexNextCarte = 0; indexNextCarte < getNbCarteRestante (); indexNextCarte++) {
			GameObject carteTeste = transform.GetChild (indexNextCarte).gameObject;
			if (carteTeste.GetComponent<CarteConstructionMetierAbstract> () != null) {
				cartePioche = carteTeste;
				Debug.Log ("Next carte trouvé");
				break;
			}
		}

		if(null != cartePioche){
			cartePioche.SetActive (true);
			CarteConstructionMetierAbstract carteConstruction = cartePioche.GetComponent<CarteConstructionMetierAbstract> ();
			carteConstruction.setJoueurProprietaireServer (netIdJoueur);
		}

		Debug.Log ("End tirerCarte()");
		return cartePioche;
	}

	public void addCarte(CarteConstructionMetierAbstract carte){
		carte.transform.SetParent (transform);

		int cartePlace = Mathf.FloorToInt (Random.Range (0, transform.childCount));
		carte.transform.SetSiblingIndex (cartePlace);
		carte.gameObject.SetActive (false);
		carte.RpcDestroyClientCard ();
	}

	private GameObject convertDataToGO(CarteConstructionAbstractData carteConstructionData){
		GameObject carteConstructionGO;
		string idCarte = "";

		if(carteConstructionData is CarteBatimentData){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteBatimentPrefab);
			idCarte = carteConstructionGO.GetComponent<CarteBatimentMetier> ().initCarte(ConvertDataAndDTOUtils.convertCarteConstructionDataToDTO(carteConstructionData));
		} else if (carteConstructionData is CarteDefenseData){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteDefensePrefab);
			idCarte = carteConstructionGO.GetComponent<CarteDefenseMetier> ().initCarte(ConvertDataAndDTOUtils.convertCarteConstructionDataToDTO(carteConstructionData));
		} else if (carteConstructionData is CarteVaisseauData){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteVaisseauPrefab);
			idCarte = carteConstructionGO.GetComponent<CarteVaisseauMetier> ().initCarte(ConvertDataAndDTOUtils.convertCarteConstructionDataToDTO(carteConstructionData));
		} else {
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.emptyPrefab);
		}

		carteConstructionGO.name = "Carte_" + idCarte;
		carteConstructionGO.SetActive (false);

		return carteConstructionGO;
	}

	public override int getNbCarteRestante (){
		return transform.childCount;
	}
}