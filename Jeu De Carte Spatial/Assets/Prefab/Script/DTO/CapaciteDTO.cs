using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapaciteDTO {

	public static int idSequence = 0;

	public CapaciteDTO (){
		Id = ++idSequence;
	}

	//Utilise pour clone
	public CapaciteDTO (int id){
		Id = id;
	}

	public int Id { get; }

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

	//L effet ce dissipe au bout de n tour
	public int Duree{ get; set; }

	public int NbCible{ get; set; }

	//stocker sous forme id+"_"+{A et/ou E si allier ou ennemie)
	public List<string> ConditionsCible{ get; set; }

	public List<string> ConditionsEmplacement{ get; set; }

	public List<string> ConditionsAction{ get; set; }

	public CarteDTO CarteInvocation{ get; set; }

	public ConstanteEnum.TypeInvocation ComportementSiModulSimilaire{ get; set; }

	public int NiveauInvocation{ get; set; }

	public CapaciteDTO Clone(){
		CapaciteDTO clone = new CapaciteDTO (this.Id);
		clone.Nom = this.Nom;
		clone.Capacite = this.Capacite;
		clone.Quantite = this.Quantite;
		clone.ModeCalcul = this.ModeCalcul;  //TODO clone enum obligatoire?
		clone.Specification = this.Specification;
		clone.AppelUnique = this.AppelUnique;
		clone.LierACarte = this.LierACarte;
		clone.ChoixCible = this.ChoixCible;
		clone.Duree = this.Duree;
		clone.NbCible = this.NbCible;

		clone.ConditionsCible = new List<string>();
		foreach (string conditionCible in this.ConditionsCible){
			clone.ConditionsCible.Add(conditionCible);
		}


		clone.ConditionsEmplacement = new List<string>();
		foreach (string conditionEmplacement in this.ConditionsEmplacement){
			clone.ConditionsEmplacement.Add(conditionEmplacement);
		}

		clone.ConditionsAction = new List<string>();
		foreach (string conditionAction in this.ConditionsAction){
			clone.ConditionsAction.Add(conditionAction);
		}
			
		//TODO Clone carte?
		clone.CarteInvocation = this.CarteInvocation;

		clone.ComportementSiModulSimilaire = this.ComportementSiModulSimilaire;   //TODO clone enum obligatoire?

		clone.NiveauInvocation = this.NiveauInvocation;

		return clone;
	}
}
