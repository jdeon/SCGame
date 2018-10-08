using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteDTO", menuName = "Mes Objets/Capacite/CapaciteDTO")]
public class CapaciteDTO : ScriptableObject{

	public enum TypeCalcul {
		RemiseA,Ajout,Multiplication,Division,Des,Chance,SuperieurARessource
		}

	public string nom;

	public int capacite;

	public int quantite;

	public TypeCalcul typeCalcul;

	public string specification;

	//L action est elle appeler a tout les tours
	public bool appelUnique;

	//Si la carte de capacité est détruite, la capacité s'arrete pareil
	public bool lierACarte;

	public bool choixCible;

	//L effet ce dissipe au bout de n tour (-1 pour infini)
	public int duree;

	//stocker sous forme id+"_"+{A et/ou E si allier ou ennemie)
	public List<string> conditionCible;

	public List<string> conditionEmplacement;

	public List<string> conditionAction;
}
