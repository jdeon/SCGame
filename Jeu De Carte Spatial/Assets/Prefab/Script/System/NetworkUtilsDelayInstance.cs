using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUtilsDelayInstance : MonoBehaviour {

	private NetworkIdentity netIdObject;
	private NetworkIdentity playerId;
	private float delay;
	private bool assign;

	public void init (bool assign, NetworkIdentity netIdObject, NetworkIdentity playerId, float delay){
		this.netIdObject = netIdObject;
		this.playerId = playerId;
		this.delay = delay;
		this.assign = assign;
	}

	void Update(){
		if (delay <= 0) {
			if (assign) {
				NetworkUtils.assignObjectToPlayer (netIdObject, playerId, -1);
			} else {
				NetworkUtils.unassignObjectFromPlayer (netIdObject, playerId, -1);
			}
			GameObject.Destroy(gameObject);
		} else {
			delay -= Time.deltaTime;
		}
	}
}
