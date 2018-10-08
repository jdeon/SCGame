using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarteDeteriorationDTO", menuName = "Mes Objets/Carte/CarteDeteriorationDTO")]
public class CarteDeteriorationDTO : CarteAbstractDTO {

	public int chanceDePioche;

	public List<CapaciteDTO> action;

	public int getNbTourAvantActif(){
		return -1; 
	}

	public override string getCarteType(){
		return "Deterioration";
	}
}
