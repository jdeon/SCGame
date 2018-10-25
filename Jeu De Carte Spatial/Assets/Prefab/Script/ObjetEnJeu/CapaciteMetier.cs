using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapaciteMetier  {

	private static int sequenceId;

	private string id;

	private int idVaisseauProvenance;

	//CapaciteData
	private int idTypeCapacite;

	private int idTypeOperation;

	private int valeurOperation;

	public CapaciteMetier(){
		id = "Capa_" + sequenceId++;
	}
}
