using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertUtils {

	public static List<T> convertToListParent<T,U> (IEnumerable<U> listEnfant) where U : T{
		List<T> listParent = new List<T> ();

		foreach (U enfant in listEnfant) {
			listParent.Add ((T)enfant);
		}

		return listParent;
	}
}
