using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckConstructionMetier : DeckMetierAbstract {

	[SerializeField]
	private DeckConstructionDTO deckContructionRef;


	private GameObject mains;

	void Start(){
		intiDeck ();
	}

	public void OnMouseDown(){
		GameObject carteTiree = tirerCarte ();

		int nbCarteEnMains = mains.transform.childCount;

		carteTiree.transform.SetParent (mains.transform);
		carteTiree.transform.localPosition = new Vector3 (ConstanteInGame.coefPlane * carteTiree.transform.localScale.x * (nbCarteEnMains + .5f), 0, 0);
		carteTiree.transform.Rotate (new Vector3 (-60, 0));
	}

	public override void intiDeck (){
		mains = GameObject.Find ("MainJoueur1");

		List<CarteConstructionAbstractDTO> listCarteConstuctionDTO = deckContructionRef.listeCarte;
		foreach (CarteConstructionAbstractDTO carteConstructionDTO in listCarteConstuctionDTO) {
			GameObject carteGO = convertDTOToGO (carteConstructionDTO);
			carteGO.transform.localScale = ConstanteInGame.tailleCarte;
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
			cartePioche.GetComponent<CarteConstructionMetierAbstract> ().generateGOCard ();
			cartePioche.transform.localPosition = new Vector3 (0, 1f, 0);
		}

		return cartePioche;
	}

	private GameObject convertDTOToGO(CarteConstructionAbstractDTO carteConstructionDTO){
		GameObject carteConstructionGO = new GameObject ();

		CarteConstructionMetierAbstract carteConstructionMetier = null;
		string idCarte = "";
		if(carteConstructionDTO is CarteBatimentDTO){
			idCarte = carteConstructionGO.AddComponent<CarteBatimentMetier> ().initCarte((CarteBatimentDTO)carteConstructionDTO);
		} else if (carteConstructionDTO is CarteDefenseDTO){
			idCarte = carteConstructionGO.AddComponent<CarteDefenseMetier> ().initCarte((CarteDefenseDTO)carteConstructionDTO);
		} else if (carteConstructionDTO is CarteVaisseauDTO){
			idCarte = carteConstructionGO.AddComponent<CarteVaisseauMetier> ().initCarte((CarteVaisseauDTO)carteConstructionDTO);
		}

		carteConstructionGO.name = "Carte_" + idCarte;
		carteConstructionGO.SetActive (false);

		return carteConstructionGO;
	}

}