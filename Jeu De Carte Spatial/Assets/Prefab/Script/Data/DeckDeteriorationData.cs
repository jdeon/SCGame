using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckDeterirationData", menuName = "Mes Objets/Deck/DeckDeteriorationData")]
public class DeckDeteriorationData : DeckAbstractData {

		public List<CarteDeteriorationData> listeCarte;

		public int getTotalChance(){
		int totalPointChance = 0;
		if (null != listeCarte) {
			foreach (CarteDeteriorationData carteDeterioration in listeCarte){
				totalPointChance += carteDeterioration.chanceDePioche;
			}
		} 
		return totalPointChance;
	}

}
