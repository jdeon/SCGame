using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarteBatimentMetier : CarteConstructionMetierAbstract {

	protected override void initId(){
		if (null == id || id == "") {
			id = "BAT_" + sequenceId;
			sequenceId++;
		}
	}

	//Affiche la carte si clique dessus
	public virtual void OnMouseDown()
	{
		generateVisualCard ();
	}
		

	public override Color getColorCarte (){
		return ConstanteInGame.colorBatiment;
	}
}
