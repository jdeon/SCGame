using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkManager {

	public override void OnServerDisconnect(NetworkConnection connection)
	{
		NetworkUtils.unassignAllObjectFromPlayer (connection);

		base.OnServerDisconnect(connection);
	}
}
