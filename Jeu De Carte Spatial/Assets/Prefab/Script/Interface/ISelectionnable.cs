﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionnable  {

	void onClick ();

	void miseEnBrillance(int etat);

	int IdISelectionnable{ get; }

	int EtatSelectionnable{ get; }
}
