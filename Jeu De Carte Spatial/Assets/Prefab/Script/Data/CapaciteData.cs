using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteData", menuName = "Mes Objets/Capacite/CapaciteData")]
public class CapaciteData : ScriptableObject{

	public string nom;

	public int capacite;

	public int quantite;

	public ConstanteEnum.TypeCalcul typeCalcul;

	public string specification;

	//L action est elle appeler a tout les tours
	public bool appelUnique;

	//Si la carte de capacité est détruite, la capacité s'arrete pareil
	public bool lierACarte;

	public bool choixCible;

	//L effet ce dissipe au bout de n tour (-1 pour infini)
	public int duree;

	public int nbCible;

	//stocker sous forme id+"_"+{A et/ou E si allier ou ennemie)
	public List<string> conditionCible;

	public List<string> conditionEmplacement;

	public List<string> conditionAction;
}
