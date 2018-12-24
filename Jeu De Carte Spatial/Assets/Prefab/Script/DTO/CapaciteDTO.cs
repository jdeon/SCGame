using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapaciteDTO {

	public string Nom{ get; set; }

	public int Capacite{ get; set; }

	public int Quantite{ get; set; }

	public ConstanteEnum.TypeCalcul ModeCalcul{ get; set; }

	public string Specification{ get; set; }

	//L action est elle appeler a tout les tours
	public bool AppelUnique{ get; set; }

	//Si la carte de capacité est détruite, la capacité s'arrete pareil
	public bool LierACarte{ get; set; }

	public bool ChoixCible{ get; set; }

	//L effet ce dissipe au bout de n tour (-1 pour infini)
	public int Duree{ get; set; }

	//stocker sous forme id+"_"+{A et/ou E si allier ou ennemie)
	public List<string> ConditionsCible{ get; set; }

	public List<string> ConditionsEmplacement{ get; set; }

	public List<string> ConditionsAction{ get; set; }

	public CarteDTO CarteInvocation{ get; set; }

	public ConstanteEnum.TypeInvocation ComportementSiModulSimilaire{ get; set; }

	public int NiveauInvocation{ get; set; }
}
