using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CarteBatimentMetier : CarteConstructionMetierAbstract {

	//[SyncVar]
	public CarteBatimentDTO carteRef;

	public override CarteAbstractDTO getCarteRef ()
	{
		return carteRef;
	}

	public override Color getColorCarte (){
		return ConstanteInGame.colorBatiment;
	}

	public override bool initCarteRef (CarteAbstractDTO initCarteRef){
		bool initDo = false;
		if (null == carteRef && initCarteRef is CarteBatimentDTO) {
			carteRef = (CarteBatimentDTO)initCarteRef;
			initDo = true;
		}

		return initDo;
	}

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

	public string initCarte (CarteBatimentDTO carteConstructionDTO){
		carteRef = carteConstructionDTO;

		return base.initCarte();
	}
}
