using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarteAmeliorationDTO : CarteDTO {

	public int PointAmelio{ get; set; }

	public List<CapaciteData> Action{ get; set; }

}