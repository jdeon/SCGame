using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class JoueurMinimalDTO  {

	public string Pseudo { get; set; }

	public NetworkInstanceId netIdJoueur { get; set;}

	public NetworkInstanceId netIdBtnTour { get; set;}
}
