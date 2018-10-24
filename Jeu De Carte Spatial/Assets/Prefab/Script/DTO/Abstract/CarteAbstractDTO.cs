using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CarteAbstractDTO : ScriptableObject {

	public int idCarte;

	public string titreCarte;

	public string libelleCarte;

	public string citationCarte;

	public Sprite image;

	public int nbTourAvantActif;

	public abstract string getCarteType();
}
