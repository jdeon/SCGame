using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DeckMetierAbstract : MonoBehaviour {

	public abstract void intiDeck ();

	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();
}
