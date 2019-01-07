using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVulnerable {

	//Retourne PV restant
	IEnumerator recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat);

	IEnumerator destruction ();

}
