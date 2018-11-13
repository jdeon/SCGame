using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapaciteMetier  {

	private static int sequenceId;

	private string id;

	private int idVaisseauProvenance;

	//CapaciteData
	private int idTypeCapacite;

	private int idTypeOperation;

	private int valeurOperation;

	public CapaciteMetier(){
		id = "Capa_" + sequenceId++;
	}

	public int getIdTypeCapacite(){
		return idTypeCapacite;
	}

	public int getNewValue(int oldValue){

		int newValue;

		switch (idTypeOperation) {
		case 0: //RemiseA
			newValue = valeurOperation;
			break;
		case 1: //Addition
			newValue = oldValue + valeurOperation;
			break;
		case 2: //Multiply
			newValue = oldValue * valeurOperation;
			break;
		case 3:	//divise
			newValue = oldValue / valeurOperation;
			break;
		case 4:	//des
			newValue = (int) Random.Range(0,valeurOperation+1);
			break;
		case 5:	//Chance
			newValue = Random.Range(0,valeurOperation+1) >= valeurOperation ? 1 : 0;
			break;
		case 6:	//ressourceSuperieurA
			int ressource = 2;//TODO modifier pour bonne value
			newValue = valeurOperation >= ressource ? ressource - valeurOperation : 0;
			break;
		default :
			newValue = oldValue;
			break;
		}

		return newValue;
	}

}
