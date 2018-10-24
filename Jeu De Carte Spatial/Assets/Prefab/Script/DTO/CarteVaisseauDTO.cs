using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarteVaisseauDTO", menuName = "Mes Objets/Carte/CarteVaisseauDTO")]
public class CarteVaisseauDTO : CarteConstructionAbstractDTO {

	public int pointAttaque;

	public int consommationCarburant;


	public override string getCarteType(){
		return "Vaisseau";
	}

	/*public void attaque (CarteConstructionAbstract cible, int degat){
		//TODO gestion attaque
	}*/
}
