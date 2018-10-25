using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteModuleData", menuName = "Mes Objets/Carte/CarteModuleData")]
public class CarteModuleData : CarteAmeliorationData {

	public int pointBouclier;

	public List<int> listIdVehiculeAffecte;

	public List<string> listZoneEffet;	 
}
