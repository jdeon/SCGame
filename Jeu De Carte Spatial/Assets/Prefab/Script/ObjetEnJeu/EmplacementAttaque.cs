using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAttaque : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();


	public void OnMouseDown(){
		//TODO fonction en cours

		GameObject goJoueur = NetworkServer.FindLocalObject (this.idJoueurPossesseur);
		Joueur joueur = goJoueur.GetComponent<Joueur> ();

		if(isMovableByPlayer(joueur)){
			if (joueur.carteSelectionne is CarteVaisseauMetier && ((CarteVaisseauMetier)joueur.carteSelectionne).isCapableAttaquer ()
			    && joueur.cartePlanetJoueur.isCarbuSuffisant (((CarteVaisseauMetier)joueur.carteSelectionne).getConsomationCarburant ())) {

				base.putCard ((CarteConstructionMetierAbstract) joueur.carteSelectionne);

				BoutonTour boutonJoueur = joueur.goPlateau.GetComponentInChildren<BoutonTour> ();
				if (null != boutonJoueur) {
					boutonJoueur.CmdSetEtatBouton(BoutonTour.enumEtatBouton.attaque);
				}
			} else if (listNomCarteExeption.Contains(joueur.carteSelectionne.name)){
				//TODO carte en exception
			}
		}
	}
}
