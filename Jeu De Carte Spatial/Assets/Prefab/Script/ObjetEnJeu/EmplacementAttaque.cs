using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAttaque : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public override void onClick(){
		//TODO fonction en cours

		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);
		Joueur joueur = goJoueur.GetComponent<Joueur> ();

		if(isMovableByPlayer(joueur)){
			if (joueur.CarteSelectionne is CarteVaisseauMetier && ((CarteVaisseauMetier)joueur.CarteSelectionne).isCapableAttaquer ()
				&& joueur.RessourceCarburant.payerRessource (((CarteVaisseauMetier)joueur.CarteSelectionne).getConsomationCarburant ())) {

				joueur.CarteSelectionne.deplacerCarte (this, NetworkInstanceId.Invalid);

				BoutonTour boutonJoueur = joueur.GoPlateau.GetComponentInChildren<BoutonTour> ();
				if (null != boutonJoueur) {
					boutonJoueur.CmdSetEtatBouton(BoutonTour.enumEtatBouton.attaque);
				}
			} else if (listNomCarteExeption.Contains(joueur.CarteSelectionne.name)){
				//TODO carte en exception
			}
		}
	}
}
