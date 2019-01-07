using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectionCiblesExecutionCapaciteRessource : SelectionCiblesExecutionCapacite {

	private List<RessourceMetier> listRessouceCible;

	public SelectionCiblesExecutionCapaciteRessource (CapaciteDTO capacite, CarteMetierAbstract carteSource, int actionAppelante) : base(capacite,carteSource,actionAppelante){
		listRessouceCible = new List<RessourceMetier>();
	}

	public List<RessourceMetier> ListRessouceCible{
		get { return listRessouceCible; }
	}
}
