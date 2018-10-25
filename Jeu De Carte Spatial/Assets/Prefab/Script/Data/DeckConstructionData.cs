using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckConstructionData", menuName = "Mes Objets/Deck/DeckConstructionData")]
public class DeckConstructionData : DeckAbstractData {

	public List<CarteConstructionAbstractData> listeCarte;

	/*public override int getNbCarteRestante(){
		return listeCarte.Count;
	}

	public override void melangerCarte(){
		//TODO implementer les fonctions
	}

	public override CarteAbstract tirerCarte(){
		//TODO implementer les fonctions
		return null;
	}*/
}
