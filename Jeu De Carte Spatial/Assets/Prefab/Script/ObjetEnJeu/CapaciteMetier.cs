using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class CapaciteMetier  {

	private static int sequenceId;

	private string id;

	private NetworkInstanceId idCarteProvenance;

	private int idCapaciteProvenance;

	//CapaciteDTO
	private int idTypeCapacite;

	private bool reversible;

	private ConstanteEnum.TypeCalcul idTypeOperation;

	private int valeurOperation;

	private int nbTourRestant;

	public CapaciteMetier(int idTypeCapacite,int idCapaciteDTO, ConstanteEnum.TypeCalcul idTypeOperation, int valeurOperation, NetworkInstanceId idCarteProvenance, bool reversible, int nbTourRestant){
		id = "Capa_" + sequenceId++;
		this.idCapaciteProvenance = idCapaciteDTO;
		this.idTypeCapacite = idTypeCapacite;
		this.idCarteProvenance = idCarteProvenance;
		this.idTypeOperation = idTypeOperation;
		this.valeurOperation = valeurOperation;
		this.reversible = reversible;
		this.nbTourRestant = nbTourRestant;
	}

	/**
	 * Transforme la capacite en mode ajout mais conserve le meme impact
	 * util si la valeur de base peut changer
	 * */
	public void transformToAddMode(int baseValue){
		if (idTypeOperation != ConstanteEnum.TypeCalcul.Ajout) {
			int newValue = getNewValue (baseValue);

			idTypeOperation = ConstanteEnum.TypeCalcul.Ajout;
			valeurOperation = newValue - baseValue;
		}
	}

	public bool endOfTurn(){
		bool existToujours = true;

		nbTourRestant--;
		if (nbTourRestant < 0) {
			existToujours = false;
		}

		return existToujours;
	}

	public NetworkInstanceId IdCarteProvenance{
		get{ return idCarteProvenance; }
	}

	public int IdCapaciteProvenance{
		get {return idCapaciteProvenance;}
	}

	public int IdTypeCapacite{
		get {return idTypeCapacite;}
	}

	public bool Reversible {
		get{ return reversible; }
	}

	public int getNewValue(int oldValue){

		int newValue;

		switch (idTypeOperation) {
		case ConstanteEnum.TypeCalcul.RemiseA:
			newValue = valeurOperation;
			break;
		case ConstanteEnum.TypeCalcul.Ajout: 
			newValue = oldValue + valeurOperation;
			break;
		case ConstanteEnum.TypeCalcul.Multiplication :
			newValue = oldValue * valeurOperation;
			break;
		case ConstanteEnum.TypeCalcul.Division:	
			newValue = oldValue / valeurOperation;
			break;
		case ConstanteEnum.TypeCalcul.Des:
			newValue = (int) Random.Range(0,valeurOperation+1);
			break;
		case ConstanteEnum.TypeCalcul.Chance: 
			newValue = Random.Range(0,valeurOperation+1) >= valeurOperation ? 1 : 0;
			break;
		case ConstanteEnum.TypeCalcul.SuperieurARessource:
			int ressource = 2;//TODO modifier pour bonne value
			newValue = valeurOperation >= ressource ? ressource - valeurOperation : 0;
			break;
		default :
			newValue = oldValue;
			break;
		}

		return newValue;
	}

	public bool isActif(){
		return nbTourRestant >= 0;
	}
}
