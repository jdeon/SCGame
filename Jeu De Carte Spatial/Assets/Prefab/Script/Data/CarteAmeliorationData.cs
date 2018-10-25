using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteAmeliorationData", menuName = "Mes Objets/Carte/CarteAmeliorationData")]
public class CarteAmeliorationData : CarteAbstractData {

	//Un deck amelioration monte a 100point
	public int pointDeck;

	public List<CapaciteData> action;
}
