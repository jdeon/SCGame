using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeckConstructionMetier : DeckMetierAbstract {

	[SerializeField]
	private DeckConstructionDTO deckContructionRef;

	public void OnMouseDown(){
		joueur.CmdTirerCarte ();
	}

	public override void intiDeck (){
		List<CarteConstructionAbstractDTO> listCarteConstuctionDTO = deckContructionRef.listeCarte;
		foreach (CarteConstructionAbstractDTO carteConstructionDTO in listCarteConstuctionDTO) {
			GameObject carteGO = convertDTOToGO (carteConstructionDTO);
			carteGO.transform.SetParent (transform);

			int cartePlace = Mathf.FloorToInt(Random.Range(0,transform.childCount));
			carteGO.transform.SetSiblingIndex (cartePlace);
		}
	}

	public override int getNbCarteRestante (){
		return transform.childCount;
	}

	public override GameObject tirerCarte(){
		int indexNextCarte = 0;
		GameObject cartePioche = null;

		for (indexNextCarte = 0; indexNextCarte < getNbCarteRestante (); indexNextCarte++) {
			GameObject carteTeste = transform.GetChild (indexNextCarte).gameObject;
			if (carteTeste.GetComponent<CarteConstructionMetierAbstract> () != null) {
				cartePioche = carteTeste;
				break;
			}
		}

		if(null != cartePioche){
			cartePioche.SetActive (true);
			CarteConstructionMetierAbstract carteConstruction = cartePioche.GetComponent<CarteConstructionMetierAbstract> ();
			//carteConstruction.CmdGenerateGOCard ();
			carteConstruction.setJoueurProprietaire (joueur);
		}

		return cartePioche;
	}

	private GameObject convertDTOToGO(CarteConstructionAbstractDTO carteConstructionDTO){
		GameObject carteConstructionGO;
		string idCarte = "";

		if(carteConstructionDTO is CarteBatimentDTO){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteBatimentPrefab);
			idCarte = carteConstructionGO.GetComponent<CarteBatimentMetier> ().initCarte((CarteBatimentDTO)carteConstructionDTO);
		} else if (carteConstructionDTO is CarteDefenseDTO){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteDefensePrefab);
			idCarte = carteConstructionGO.GetComponent<CarteDefenseMetier> ().initCarte((CarteDefenseDTO)carteConstructionDTO);
		} else if (carteConstructionDTO is CarteVaisseauDTO){
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.carteVaisseauPrefab);
			idCarte = carteConstructionGO.GetComponent<CarteVaisseauMetier> ().initCarte((CarteVaisseauDTO)carteConstructionDTO);
		} else {
			carteConstructionGO = Instantiate<GameObject> (ConstanteInGame.emptyPrefab);
		}

		carteConstructionGO.name = "Carte_" + idCarte;
		carteConstructionGO.SetActive (false);

		return carteConstructionGO;
	}

}