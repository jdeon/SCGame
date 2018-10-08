using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteAmeliorationDTO", menuName = "Mes Objets/Carte/CarteAmeliorationDTO")]
public class CarteAmeliorationDTO : CarteAbstractDTO {

	//Un deck amelioration monte a 100point
	public int pointDeck;

	public List<CapaciteDTO> action;

	public override string getCarteType(){
		return "Amelioration";
	}
}
