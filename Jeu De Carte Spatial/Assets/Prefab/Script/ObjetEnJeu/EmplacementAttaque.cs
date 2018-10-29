using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAttaque : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();


	public virtual void OnMouseDown(){
		//TODO fonction en cours

		GameObject goJoueur = NetworkServer.FindLocalObject (this.idJoueurPossesseur);
		Joueur joueur = goJoueur.GetComponent<Joueur> ();

		if(null != joueur && null != joueur.carteSelectionne && (joueur.carteSelectionne is CarteVaisseauMetier || listNomCarteExeption.Contains(joueur.carteSelectionne.name))){
			base.putCard (joueur.carteSelectionne);
		}
	}
}
