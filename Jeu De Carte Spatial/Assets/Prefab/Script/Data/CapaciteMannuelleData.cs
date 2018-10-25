using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteManuelleData", menuName = "Mes Objets/Capacite/CapaciteManuelleData")]
public class CapaciteMannuelleData : ScriptableObject {

	public string titreCarte;

	public string libelleCarte;

	public string citationCarte;

	public bool remplaceAttaque;

	public List<string> periodeUtilisable;

	public List<CapaciteData> capaciteCondition;

	public List<CapaciteData> capaciteEffet;
}
