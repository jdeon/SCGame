using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConvertUtils {

	public static List<T> convertToListParent<T,U> (IEnumerable<U> listEnfant) where U : T{
		List<T> listParent = new List<T> ();

		foreach (U enfant in listEnfant) {
			listParent.Add ((T)enfant);
		}

		return listParent;
	}

	public static T convertNetIdToScript<T> (NetworkInstanceId netId, bool isLocalPlayer) where T : NetworkBehaviour{
		T result = null;
		GameObject go;

		if (isLocalPlayer) {
			go = ClientScene.FindLocalObject (netId);
		} else {
			go = NetworkServer.FindLocalObject (netId);
		}
			
		if (null != go) {
			result = go.GetComponent<T> ();
		}

		return result;
	}
}
