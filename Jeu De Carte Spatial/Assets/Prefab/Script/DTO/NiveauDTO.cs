using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NiveauDTO", menuName = "Mes Objets/NiveauDTO")]
public class NiveauDTO : ScriptableObject {

	public enum TypeNbCapaciteAuxChoix {
		Egal,Min,Max
	}


	public int idNiveau;

	public string titreNiveau;

	public string descriptionNiveau;

	public string citationNiveau;

	public int numNiveau;

	public int cout;

	public List<CapaciteDTO> capacite;

	public TypeNbCapaciteAuxChoix typeNbCapaciteAuxChoix;

	public int valeurTypeCapaciteAuxChoix;

	public List<CapaciteMannuelleDTO> capaciteManuelle;



}
