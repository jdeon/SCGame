using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarteDefenseDTO", menuName = "Mes Objets/Carte/CarteDefenseDTO")]
public class CarteDefenseDTO : CarteConstructionAbstractDTO {

	public int pointAttaque;

	public override string getCarteType(){
		return "Defense";
	}

	/*public void defend(int degat, CarteVaisseau vaisseauAttaquant){
		//TODO gérer défense
	}*/
}
