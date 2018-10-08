using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckDeterirationDTO", menuName = "Mes Objets/Deck/DeckDeteriorationDTO")]
public class DeckDeriorationDTO : DeckAbstractDTO {

		public List<CarteDeteriorationDTO> listeCarte;

		public int getTotalChance(){
		int totalPointChance = 0;
		if (null != listeCarte) {
			foreach (CarteDeteriorationDTO carteDeterioration in listeCarte){
				totalPointChance += carteDeterioration.chanceDePioche;
			}
		} 
		return totalPointChance;
	}

}
