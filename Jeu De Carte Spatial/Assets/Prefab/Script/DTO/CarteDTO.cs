using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarteDTO {

	public string TitreCarte{ get; set; }

	public string LibelleCarte{ get; set; }

	public string CitationCarte{ get; set; }

	public string ImagePath{ get; set; }

	public int NbTourAvantActif{ get; set; }

	public string TypeOfCarte{ get; set; }
}