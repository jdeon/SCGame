using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUtils  {

	public static void assignObjectToPlayer(NetworkBehaviour netObject, NetworkIdentity playerId){

		NetworkIdentity netIdentity = netObject.GetComponent<NetworkIdentity> ();
		netIdentity.localPlayerAuthority = true;
		NetworkConnection otherOwner = netIdentity.clientAuthorityOwner;        

		if (otherOwner == playerId.connectionToClient) {
			return;
		} else if (otherOwner != null) {
			netIdentity.RemoveClientAuthority (otherOwner);
		}

		netIdentity.AssignClientAuthority (playerId.connectionToClient);
	}

	public static void unassignObjectFromPlayer(NetworkBehaviour netObject, NetworkIdentity playerId){

		NetworkIdentity netIdentity = netObject.GetComponent<NetworkIdentity> ();
		if (null != netIdentity) {
			netIdentity.localPlayerAuthority = false;     
			netIdentity.RemoveClientAuthority (playerId.connectionToClient);
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
