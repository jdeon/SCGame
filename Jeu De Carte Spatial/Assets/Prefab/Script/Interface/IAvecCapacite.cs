﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IAvecCapacite {

	//List<CapaciteMetier> ListCapacite{ get; }

	void addCapacity (CapaciteMetier capaToAdd);

	void removeLinkCardCapacity (NetworkInstanceId netIdCard);

	void capaciteFinTour ();

	List<CapaciteMetier> containCapacityOfType(int idTypCapacity);

	bool containCapacityWithId (int idCapacityDTO);
}