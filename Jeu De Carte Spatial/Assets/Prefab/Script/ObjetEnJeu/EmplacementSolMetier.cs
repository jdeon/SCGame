using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementSolMetier : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public override void onClick(){
		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);

		if (null != goJoueur) {
			Joueur joueur = goJoueur.GetComponent<Joueur> ();

			if (isMovableByPlayer (joueur)) {
				Joueur localJoueur = JoueurUtils.getJoueurLocal ();
				if (this.etatSelectionnable == 1 && null != localJoueur.PhaseChoixCible && !localJoueur.PhaseChoixCible.finChoix) {
					localJoueur.PhaseChoixCible.listCibleChoisi.Add (this);

				} else if (joueur.CarteSelectionne is CarteBatimentMetier || joueur.CarteSelectionne is CarteDefenseMetier || listNomCarteExeption.Contains (joueur.CarteSelectionne.name)) {
					if (isCardCostPayable (joueur.RessourceMetal, joueur.CarteSelectionne)) {
						joueur.CarteSelectionne.deplacerCarte (this, NetworkInstanceId.Invalid);
					}
				} else if (joueur.CarteSelectionne is CarteVaisseauMetier) {
					//TODO vaisseau en mode defense
				}
			}
		}
	}
}
