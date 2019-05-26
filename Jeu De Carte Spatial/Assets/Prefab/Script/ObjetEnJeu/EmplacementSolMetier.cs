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
				EventTask eventTask = EventTaskUtils.getEventTaskEnCours ();
				if (this.etatSelectionnable == SelectionnableUtils.ETAT_SELECTIONNABLE && null != eventTask && eventTask is EventTaskChoixCible) {
					((EventTaskChoixCible) eventTask).ListCibleChoisie.Add (this);


				} else if (joueur.CarteSelectionne is CarteBatimentMetier || joueur.CarteSelectionne is CarteDefenseMetier || listNomCarteExeption.Contains (joueur.CarteSelectionne.name)) {
					if (isCardCostPayable (joueur.RessourceMetal, joueur.CarteSelectionne)) {
						joueur.CmdPayerRessource(joueur.RessourceMetal.TypeRessource,((CarteConstructionMetierAbstract)joueur.CarteSelectionne).getCoutMetal ());
						joueur.CarteSelectionne.deplacerCarte (this,joueur.netId,NetworkInstanceId.Invalid);
					}
				} else if (joueur.CarteSelectionne is CarteVaisseauMetier) {
					//TODO vaisseau en mode defense
				}
			}
		}
	}
}
