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

		if(null != joueur && null != joueur.carteSelectionne && (joueur.carteSelectionne is CarteVaisseauMetier || listNomCarteExeption.Contains(joueur.carteSelectionne.name))){
			if (joueur.carteSelectionne is CarteVaisseauMetier && ((CarteVaisseauMetier)joueur.carteSelectionne).isCapableAttaquer ()
			    && joueur.cartePlanetJoueur.isCarbuSuffisant (((CarteVaisseauMetier)joueur.carteSelectionne).getConsomationCarburant ())) {

				base.putCard (joueur.carteSelectionne);

				BoutonTour boutonJoueur = joueur.goPlateau.GetComponentInChildren<BoutonTour> ();
				if (null != boutonJoueur) {
					boutonJoueur.etatBouton = BoutonTour.enumEtatBouton.attaque;
				}
			} else {
				//TODO carte en exception
			}
		}
	}
}
