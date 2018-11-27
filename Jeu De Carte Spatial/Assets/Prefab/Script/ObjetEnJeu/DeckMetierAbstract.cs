using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DeckMetierAbstract : NetworkBehaviour {

	protected Joueur joueurProprietaire;

	[SyncVar  (hook = "onChangeNetIdJoueur")]
	protected NetworkInstanceId netIdJoueur;

	public abstract void intiDeck (NetworkInstanceId joueurNetId);

	public abstract int getNbCarteRestante ();

	public abstract GameObject tirerCarte();

	private void onChangeNetIdJoueur(NetworkInstanceId netIdJoueur){
		joueurProprietaire = Joueur.getJoueur (netIdJoueur);
	}
}
