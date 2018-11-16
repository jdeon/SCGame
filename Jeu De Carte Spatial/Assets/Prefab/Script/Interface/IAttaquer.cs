using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttaquer  {

	//void goLigneAttaque (EmplacementAttaque cible);

	void attaque (CarteConstructionMetierAbstract cible);

	bool isCapableAttaquer ();

	bool isAttaqueEnCours ();
}
