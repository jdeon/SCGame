using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NiveauData", menuName = "Mes Objets/NiveauData")]
public class NiveauData : ScriptableObject {

	public int idNiveau;

	public string titreNiveau;

	public string descriptionNiveau;

	public string citationNiveau;

	public int numNiveau;

	public int cout;

	public List<CapaciteData> capacite;

	public ConstanteEnum.TypeNbCapaciteAuxChoix typeNbCapaciteAuxChoix;

	public int valeurTypeCapaciteAuxChoix;

	public List<CapaciteMannuelleData> capaciteManuelle;
}
