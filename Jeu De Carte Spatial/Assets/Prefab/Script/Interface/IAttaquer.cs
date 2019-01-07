using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttaquer  {

	//void goLigneAttaque (EmplacementAttaque cible);

	IEnumerator attaqueCarte (CarteConstructionMetierAbstract cible, int idCoroutine);

	IEnumerator attaquePlanete (CartePlaneteMetier cible, int idCoroutine);

	bool isCapableAttaquer ();

	bool AttaqueCeTour { get; set;}
}
