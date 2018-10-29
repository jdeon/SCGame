using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableCardPart : MonoBehaviour {

	private CarteMetierAbstract carteMere;

	public void setCarteMere(CarteMetierAbstract carteMere){
		this.carteMere = carteMere;
	}

	//Affiche la carte si clique dessus
	public virtual void OnMouseDown()
	{
		if (null != carteMere) {
			if (carteMere is CarteVaisseauMetier) {
				((CarteVaisseauMetier)carteMere).generateVisualCard ();
			} else if (carteMere is CarteDefenseMetier) {
				((CarteDefenseMetier)carteMere).generateVisualCard ();
			} else if (carteMere is CarteBatimentMetier) {
				((CarteBatimentMetier)carteMere).generateVisualCard ();
			} else if (carteMere is CartePlaneteMetier) {
				((CartePlaneteMetier)carteMere).generateVisualCard ();
			}
		}
	}
}
