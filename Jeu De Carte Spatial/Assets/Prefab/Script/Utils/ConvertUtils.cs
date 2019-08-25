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

	public static T[] convertHashsetToArray<T> (HashSet<T> hashsetToChange){
		T[] result = new T[hashsetToChange.Count];
		int i = 0;

		foreach (T toConvert in hashsetToChange) {
			result [i] = toConvert;
			i++;
		}

		return result;
	}
}
