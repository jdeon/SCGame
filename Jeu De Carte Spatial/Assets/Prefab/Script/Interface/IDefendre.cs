using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefendre {

	IEnumerator preDefense (CarteVaisseauMetier vaisseauAttaquant);

	IEnumerator defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant);

	bool isCapableDefendre ();

	bool DefenseSelectionne{ get; }

	void reinitDefenseSelect ();

	void reinitDefenseSelectTour ();

	bool SelectionnableDefense { get; set; }
}
