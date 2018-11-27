using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkManager {

	public override void OnServerDisconnect(NetworkConnection connection)
	{
		HashSet<NetworkInstanceId> setOwnedObject = new HashSet<NetworkInstanceId> ();
		setOwnedObject.UnionWith(connection.clientOwnedObjects);

		foreach(NetworkInstanceId ownedObject in setOwnedObject){
			GameObject goOwned = NetworkServer.FindLocalObject (ownedObject);
			if (null != goOwned && null != goOwned.GetComponent<NetworkIdentity> ()) {
				goOwned.GetComponent<NetworkIdentity> ().RemoveClientAuthority (connection);
			}
		}

		base.OnServerDisconnect(connection);
	}
}
