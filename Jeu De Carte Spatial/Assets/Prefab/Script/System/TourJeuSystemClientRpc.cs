using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TourJeuSystemClientRpc {

	[ClientRpc]
	public void RpcReinitBoutonNewTour(NetworkInstanceId networkIdBtn)
	{
		GameObject goBtn = NetworkServer.FindLocalObject (networkIdBtn);

		if (null != goBtn && null != goBtn.GetComponent<BoutonTour> ()) {
			BoutonTour boutonTour = goBtn.GetComponent<BoutonTour> ();
			boutonTour.initTour ();

		}
	}
}
