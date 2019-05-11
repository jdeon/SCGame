using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IAttaquer  {

	//void goLigneAttaque (EmplacementAttaque cible);

	void attaqueCarte (CarteConstructionMetierAbstract cible, NetworkInstanceId netIdTaskEvent);

	void attaquePlanete (CartePlaneteMetier cible, NetworkInstanceId netIdTaskEvent);

	bool isCapableAttaquer ();

	bool AttaqueCeTour { get; set;}
}
