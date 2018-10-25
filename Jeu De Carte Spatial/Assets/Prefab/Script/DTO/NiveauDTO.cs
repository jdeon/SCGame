using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NiveauDTO {

	public string TitreNiveau{ get; set; }

	public string DescriptionNiveau{ get; set; }

	public string CitationNiveau{ get; set; }

	public int Cout{ get; set; }

	public List<CapaciteDTO> Capacite{ get; set; }

	public ConstanteEnum.TypeNbCapaciteAuxChoix NbCapaciteAuxChoix{ get; set; }

	public int ValeurTypeCapaciteAuxChoix{ get; set; }

	public List<CapaciteMannuelleDTO> CapaciteManuelle{ get; set; }
}
