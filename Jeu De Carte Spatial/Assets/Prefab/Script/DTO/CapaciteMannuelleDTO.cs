using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CapaciteManuelleDTO", menuName = "Mes Objets/Capacite/CapaciteManuelleDTO")]
public class CapaciteMannuelleDTO : ScriptableObject {

	public string titreCarte;

	public string libelleCarte;

	public string citationCarte;

	public bool remplaceAttaque;

	public List<string> periodeUtilisable;

	public List<CapaciteDTO> capaciteCondition;

	public List<CapaciteDTO> capaciteEffet;
}
