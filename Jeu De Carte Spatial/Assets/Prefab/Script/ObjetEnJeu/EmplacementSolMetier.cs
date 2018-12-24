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
			if (joueur.CarteSelectionne is CarteBatimentMetier || joueur.CarteSelectionne is CarteDefenseMetier || listNomCarteExeption.Contains (joueur.CarteSelectionne.name)) {
				if (isCardCostPayable (joueur.RessourceMetal, joueur.CarteSelectionne)) {
					joueur.CarteSelectionne.deplacerCarte (this, NetworkInstanceId.Invalid);
				}
			} else if (joueur.CarteSelectionne is CarteVaisseauMetier) {
				//TODO vaisseau en mode defense
			}
		}
	}
}
