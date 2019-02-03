using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAtomsphereMetier : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public override void onClick(){
		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);

		if (null != goJoueur) {
			Joueur joueur = goJoueur.GetComponent<Joueur> ();

			if (isMovableByPlayer (joueur) && (joueur.CarteSelectionne is CarteVaisseauMetier || listNomCarteExeption.Contains (joueur.CarteSelectionne.name))) {
				Joueur localJoueur = JoueurUtils.getJoueurLocal ();
				if (this.etatSelectionnable == 1 && null != localJoueur.PhaseChoixCible && !localJoueur.PhaseChoixCible.finChoix) {
					localJoueur.PhaseChoixCible.listCibleChoisi.Add (this);

				} else if (isCardCostPayable (joueur.RessourceMetal, joueur.CarteSelectionne)) {
					joueur.CmdPayerRessource(joueur.RessourceMetal.TypeRessource,((CarteConstructionMetierAbstract)joueur.CarteSelectionne).getCoutMetal ());


					joueur.CarteSelectionne.deplacerCarte (this, NetworkInstanceId.Invalid);
				}
			}
		}
	}



}
