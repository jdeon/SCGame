using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionnable  {

	void onClick ();

	int IdISelectionnable{ get; }

	/**
	 * 0:rien
	 * 1:selectionnable
	 * 2 : mouseOver
	 * 3 : Selectionne
	 * */
	int EtatSelectionnable{ get; set;}
}
