using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapaciteMannuelleDTO {

	public string TitreCarte{get;set;}

	public string LibelleCarte{get;set;}

	public string CitationCarte{get;set;}

	public bool RemplaceAttaque{get;set;}

	public List<string> PeriodeUtilisable{get;set;}

	public List<CapaciteDTO> CapaciteCondition{get;set;}

	public List<CapaciteDTO> CapaciteEffet{get;set;}
}
