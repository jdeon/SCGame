using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteModuleDTO", menuName = "Mes Objets/Carte/CarteModuleDTO")]
public class CarteModuleDTO : CarteAmeliorationDTO {

	public int pointBouclier;

	public List<int> listIdVehiculeAffecte;

	public List<string> listZoneEffet;	 
}
