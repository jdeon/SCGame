using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttaquer  {

	//void goLigneAttaque (EmplacementAttaque cible);

	void attaqueCarte (CarteConstructionMetierAbstract cible);

	void attaquePlanete (CartePlaneteMetier cible);

	bool isCapableAttaquer ();

	bool AttaqueCeTour { get; }
}
