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
}
