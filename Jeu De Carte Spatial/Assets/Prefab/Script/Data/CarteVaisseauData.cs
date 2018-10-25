using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarteVaisseauData", menuName = "Mes Objets/Carte/CarteVaisseauData")]
public class CarteVaisseauData : CarteConstructionAbstractData {

	public int pointAttaque;

	public int consommationCarburant;
}
