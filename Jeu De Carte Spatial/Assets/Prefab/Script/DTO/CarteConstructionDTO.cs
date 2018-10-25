using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarteConstructionDTO : CarteDTO {

	public int PointVieMax{ get; set; }

	public int PointAttaque{ get; set; }

	public int ConsommationCarburant{ get; set; }

	public List<NiveauDTO> ListNiveau{ get; set; }
}
