using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarteVaisseauMetier : CarteConstructionMetierAbstract {

	public CarteVaisseauDTO carteRef;

	public override CarteAbstractDTO getCarteRef ()
	{
		return carteRef;
	}

	/*public void OnMouseDown()
	{
		base.OnMouseDown ();
		/paternRessourceCarbu.SetActive (true);
		paternRessourceCarbu.GetComponentInChildren<Text> ().text = "" + carteRef.consommationCarburant;
	}*/
}
