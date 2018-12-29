using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmplacementAtomsphereMetier : EmplacementMetierAbstract {

	private static List<string> listNomCarteExeption = new List<string>();

	public override void onClick(){
		GameObject goJoueur = ClientScene.FindLocalObject (this.idJoueurPossesseur);
		Joueur joueur = goJoueur.GetComponent<Joueur> ();

		if(isMovableByPlayer(joueur) && (joueur.CarteSelectionne is CarteVaisseauMetier || listNomCarteExeption.Contains(joueur.CarteSelectionne.name))){
			if (isCardCostPayable(joueur.RessourceMetal,joueur.CarteSelectionne)) {
				joueur.CarteSelectionne.deplacerCarte (this, NetworkInstanceId.Invalid);
			}
		}
	}



}
