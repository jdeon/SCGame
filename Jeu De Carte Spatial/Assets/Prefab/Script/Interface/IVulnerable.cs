using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IVulnerable {

	//Est la cible d'une attauqe mais ne perd pas forcément de PV
	void recevoirAttaque (CarteMetierAbstract sourceDegat, NetworkInstanceId netdTaskEvent, bool attaqueSimultane);

	//Retourne PV restant
	int recevoirDegat (int nbDegat, CarteMetierAbstract sourceDegat, NetworkInstanceId netdTaskEvent);

	void destruction (Joueur joueurSourceAction, NetworkInstanceId netdTaskEvent);

}
