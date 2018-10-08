using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DeckAbstract : ScriptableObject {

	public abstract int getNbCarteRestante ();

	public abstract void melangerCarte();

	//public abstract CarteAbstract tirerCarte();
}
