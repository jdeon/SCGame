using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable {

	//Retourne PV restant
	int recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat);

	void destruction ();

}
