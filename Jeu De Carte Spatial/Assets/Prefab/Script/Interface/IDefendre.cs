using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefendre {

	void preDefense (CarteVaisseauMetier vaisseauAttaquant);

	void defenseSimultanee(CarteVaisseauMetier vaisseauAttaquant);

	bool isCapableDefendre ();

	bool DefenseSelectionne{ get; }

	void reinitDefenseSelect ();

	void reinitDefenseSelectTour ();

	bool SelectionnableDefense { get; set; }
}
