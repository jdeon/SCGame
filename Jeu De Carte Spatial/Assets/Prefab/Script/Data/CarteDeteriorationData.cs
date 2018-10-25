using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteDeteriorationData", menuName = "Mes Objets/Carte/CarteDeteriorationData")]
public class CarteDeteriorationData : CarteAbstractData {

	public int chanceDePioche;

	public List<CapaciteData> action;
}
