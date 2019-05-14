using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUtils : MonoBehaviour {

	public static void assignObjectToPlayer(NetworkIdentity netIdObject, NetworkIdentity playerId, float delay){

		if (delay >= 0) {
			NetworkUtilsDelayInstance instance = new NetworkUtilsDelayInstance (false,netIdObject,playerId,delay);

		} else if (null != netIdObject) {
			netIdObject.localPlayerAuthority = true;
			NetworkConnection otherOwner = netIdObject.clientAuthorityOwner;        

			if (otherOwner == playerId.connectionToClient) {
				return;
			} else if (otherOwner != null) {
				netIdObject.RemoveClientAuthority (otherOwner);
			}

			netIdObject.AssignClientAuthority (playerId.connectionToClient);
		}
	}

	public static void unassignObjectFromPlayer(NetworkIdentity netIdObject, NetworkIdentity playerId, float delay){

		if (delay >= 0) {
			NetworkUtilsDelayInstance instance = new NetworkUtilsDelayInstance (false,netIdObject,playerId,delay);

		} else if (null != netIdObject) {
			netIdObject.localPlayerAuthority = false;     
			netIdObject.RemoveClientAuthority (playerId.connectionToClient);
		}
	}


	public static void unassignAllObjectFromPlayer(NetworkConnection netConnexion){
		HashSet<NetworkInstanceId> setOwnedObject = new HashSet<NetworkInstanceId> ();
		setOwnedObject.UnionWith (netConnexion.clientOwnedObjects);

		foreach (NetworkInstanceId ownedObject in setOwnedObject) {
			GameObject goOwned = NetworkServer.FindLocalObject (ownedObject);
			if (null != goOwned && null != goOwned.GetComponent<NetworkIdentity> ()) {
				goOwned.GetComponent<NetworkIdentity> ().RemoveClientAuthority (netConnexion);
			}
		}
	}

	public class NetworkUtilsDelayInstance : MonoBehaviour {

		private NetworkIdentity netIdObject;
		private NetworkIdentity playerId;
		private float delay;
		private bool assign;

		public NetworkUtilsDelayInstance (bool assign, NetworkIdentity netIdObject, NetworkIdentity playerId, float delay){
			this.netIdObject = netIdObject;
			this.playerId = playerId;
			this.delay = delay;
			this.assign = assign;
		}

		public void Update(){
			if (delay <= 0) {
				if (assign) {
					assignObjectToPlayer (netIdObject, playerId, -1);
				} else {
					unassignObjectFromPlayer (netIdObject, playerId, -1);
				}
				Destroy (this);
			} else {
				delay -= Time.deltaTime;
			}
		}
	}
}
