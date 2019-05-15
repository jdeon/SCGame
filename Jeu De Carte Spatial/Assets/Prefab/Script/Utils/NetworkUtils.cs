using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUtils : MonoBehaviour {

	public static void assignObjectToPlayer(NetworkIdentity netIdObject, NetworkIdentity playerId, float delay){

		if (delay >= 0) {
			GameObject goNetworkUtils = new GameObject ("NetworkUtilsDelayInstance");
			NetworkUtilsDelayInstance instance = goNetworkUtils.AddComponent<NetworkUtilsDelayInstance> ();
			instance.init(true,netIdObject,playerId,delay);

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
			GameObject goNetworkUtils = new GameObject ("NetworkUtilsDelayInstance");
			NetworkUtilsDelayInstance instance = goNetworkUtils.AddComponent<NetworkUtilsDelayInstance> ();
			instance.init(false,netIdObject,playerId,delay);

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
}