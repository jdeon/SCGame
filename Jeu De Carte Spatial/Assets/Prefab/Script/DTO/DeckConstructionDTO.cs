using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckConstructionDTO", menuName = "Mes Objets/Deck/DeckConstructionDTO")]
public class DeckConstructionDTO : DeckAbstractDTO {

	public List<CarteConstructionAbstractDTO> listeCarte;

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
