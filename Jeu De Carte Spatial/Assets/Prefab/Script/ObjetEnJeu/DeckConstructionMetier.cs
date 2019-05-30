using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeckConstructionMetier : DeckMetierAbstract {

	[SerializeField]
	private DeckConstructionData deckContructionRef;

	public override void intiDeck (Joueur joueurInitiateur, bool isServer){
		base.intiDeck(joueurInitiateur, isServer);

		if (isServer && null != deckContructionRef) {
			List<CarteConstructionAbstractData> listCarteConstuctionData = deckContructionRef.listeCarte;
			foreach (CarteConstructionAbstractData carteConstructionData in listCarteConstuctionData) {
				GameObject carteGO = convertDataToGO (carteConstructionData, isServer);
				carteGO.transform.SetParent (transform);

				int cartePlace = Mathf.FloorToInt (Random.Range (0, transform.childCount));
				carteGO.transform.SetSiblingIndex (cartePlace);
			}
		}
	}

	public void piocheDeckConstructionByServer(){
		if (joueurProprietaire.isServer && getNbCarteRestante() > 0) {
			Debug.Log ("taille deck : " + getNbCarteRestante());

			GameObject carteTiree = tirerCarte ();

			Debug.Log ("carteTiree : " + carteTiree);

			NetworkServer.Spawn (carteTiree);
			carteTiree.transform.parent = null; //Carte pioche en attente de plassement

			CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();
			ActionEventManager.EventActionManager.CreateTask (carteConstructionScript.netId, joueurProprietaire.netId, this.IdISelectionnable, ConstanteIdObjet.ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION, NetworkInstanceId.Invalid, false);
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
			carteConstruction.NetIdJoueurProprietaire = NetIdJoueur;
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

	private GameObject convertDataToGO(CarteConstructionAbstractData carteConstructionData, bool isServer){
		CarteConstructionDTO carteDTO = ConvertDataAndDTOUtils.convertCarteConstructionDataToDTO (carteConstructionData);

		GameObject carteConstructionGO = CarteUtils.convertCarteDTOToGameobject (carteDTO, isServer);

		carteConstructionGO.SetActive (false);

		return carteConstructionGO;
	}

	public override int getNbCarteRestante (){
		return transform.childCount;
	}
}