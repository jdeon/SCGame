using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DeckMetierAbstract : NetworkBehaviour {

	protected Joueur joueur;

	public abstract void intiDeck ();

	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();

	public void setJoueur (Joueur joueurParent){
		joueur = joueurParent;
	}
}
