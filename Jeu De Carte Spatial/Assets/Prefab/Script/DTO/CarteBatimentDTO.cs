using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CarteBatimentDTO", menuName = "Mes Objets/Carte/CarteBatimentDTO")]
public class CarteBatimentDTO : CarteConstructionAbstractDTO {

	public override string getCarteType(){
		return "Batiment";
	}
}
