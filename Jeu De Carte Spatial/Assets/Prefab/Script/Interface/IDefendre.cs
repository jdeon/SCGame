using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IDefendre {

	void preDefense (CarteVaisseauMetier vaisseauAttaquant, NetworkInstanceId netIdTaskEvent);

	void defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant, NetworkInstanceId netIdTaskEvent);

	bool isCapableDefendre ();

	bool DefenseSelectionne{ get; }

	void reinitDefenseSelect ();

	void reinitDefenseSelectTour ();

	bool SelectionnableDefense { get; set; }
}
