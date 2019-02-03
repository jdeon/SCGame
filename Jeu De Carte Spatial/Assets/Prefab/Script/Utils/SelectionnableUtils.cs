using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionnableUtils {

	public static int sequenceSelectionnable;

	public static ISelectionnable getSelectiobleById(int id){
		ISelectionnable selectionnableResult = null;

		List<ISelectionnable> listSelectionnable = GameObject.FindObjectsOfType<MonoBehaviour> ().OfType<ISelectionnable>().ToList<ISelectionnable>();

		if (id > 0 && null != listSelectionnable && listSelectionnable.Count > 0) {
			foreach (ISelectionnable selectionnable in listSelectionnable) {
				if (selectionnable.IdISelectionnable == id) {
					selectionnableResult = selectionnable;
					break;
				}
			}
		}

		return selectionnableResult;
	}
}
