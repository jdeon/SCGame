using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarteBatimentMetier : CarteConstructionMetierAbstract {

	public CarteBatimentDTO carteRef;

	public override CarteAbstractDTO getCarteRef ()
	{
		return carteRef;
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorBatiment;
	}

	protected override void initId(){
		if (null == id || id == "") {
			id = "BAT_" + sequenceId;
			sequenceId++;
		}
	}

	public string initCarte (CarteBatimentDTO carteConstructionDTO){
		carteRef = carteConstructionDTO;

		return base.initCarte();
	}
}
