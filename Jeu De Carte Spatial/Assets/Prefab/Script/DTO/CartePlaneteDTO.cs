using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CartePlaneteDTO", menuName = "Mes Objets/Carte/CartePlaneteDTO")]
public class CartePlanete : CarteConstructionAbstractDTO {

	public override string getCarteType (){
		return "Planete";
	}
}
