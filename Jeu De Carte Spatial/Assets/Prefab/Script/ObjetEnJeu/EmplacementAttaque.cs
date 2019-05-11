using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAttaque : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public override void onClick(){
		//TODO fonction en cours

		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);

		if (null != goJoueur) {
			Joueur joueur = goJoueur.GetComponent<Joueur> ();

			if (isMovableByPlayer (joueur)) {		
				EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
				if (this.etatSelectionnable == 1 && null != eventTask && eventTask is EventTaskChoixCible) {
					((EventTaskChoixCible) eventTask).ListCibleChoisie.Add (this);

				} else if (joueur.CarteSelectionne is CarteVaisseauMetier && ((CarteVaisseauMetier)joueur.CarteSelectionne).isCapableAttaquer ()
				          && joueur.RessourceCarburant.StockWithCapacity >= ((CarteVaisseauMetier)joueur.CarteSelectionne).getConsomationCarburant ()) {

					joueur.CmdPayerRessource (joueur.RessourceCarburant.TypeRessource, ((CarteVaisseauMetier)joueur.CarteSelectionne).getConsomationCarburant ());
					joueur.CarteSelectionne.deplacerCarte (this,joueur.netId,NetworkInstanceId.Invalid);

					//TODO doit on mettre tous de suite le bouton (si le déplacement est impossible?
					BoutonTour boutonJoueur = joueur.GoPlateau.GetComponentInChildren<BoutonTour> ();
					if (null != boutonJoueur) {
						boutonJoueur.CmdSetEtatBouton (BoutonTour.enumEtatBouton.attaque);
					}
				} else if (listNomCarteExeption.Contains (joueur.CarteSelectionne.name)) {
					//TODO carte en exception
				}
			}
		}
	}
}
