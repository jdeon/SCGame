using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementSolMetier : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public void OnMouseDown(){
		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);
		Joueur joueur = goJoueur.GetComponent<Joueur> ();

		if(isMovableByPlayer(joueur)){
			if (joueur.carteSelectionne is CarteBatimentMetier || joueur.carteSelectionne is CarteDefenseMetier || listNomCarteExeption.Contains (joueur.carteSelectionne.name)) {
				if (isCardCostPayable (joueur.cartePlanetJoueur, joueur.carteSelectionne)) {
					base.putCard ((CarteConstructionMetierAbstract) joueur.carteSelectionne);
				}
			} else if (joueur.carteSelectionne is CarteVaisseauMetier) {
				//TODO vaisseau en mode defense
			}
		}
	}
}
