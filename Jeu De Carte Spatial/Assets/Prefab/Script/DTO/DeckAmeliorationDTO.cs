using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckAmeliorationDTO", menuName = "Mes Objets/Deck/DeckAmeliorationDTO")]
public class DeckAmeliorationDTO : DeckAbstractDTO {

	public List<CarteAmeliorationDTO> listeCarte;

	/*public override int getNbCarteRestante(){
		return listeCarte.Count;
	}*/

	/*public int getNbPointDeck(){
	//TODO implementer les fonctions
	return null;
	}*/

}
