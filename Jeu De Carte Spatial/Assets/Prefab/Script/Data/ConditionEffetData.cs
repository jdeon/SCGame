using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionEffet", menuName = "Mes Objets/ConditionEffet")]
public class ConditionEffet : ScriptableObject{

	//stocker sous forme id+"_"+{A et/ou E si allier ou ennemie)
	public List<string> typesCible;

	public List<string> typesEmplacement;

	public List<string> typesAction;

	public ConditionEffet (){
		this.typesCible = new List<string> ();
		this.typesEmplacement = new List<string> ();
		this.typesAction = new List<string> ();
	}
}
