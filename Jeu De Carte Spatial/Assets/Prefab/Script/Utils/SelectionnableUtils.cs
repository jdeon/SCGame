using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionnableUtils {

	public static readonly int ETAT_RETOUR_ATTIERE = -1;

	public static readonly int ETAT_NON_SELECTION = 0;

	public static readonly int ETAT_SELECTIONNABLE = 1;

	public static readonly int ETAT_MOUSE_OVER = 2;

	public static readonly int ETAT_SELECTIONNE = 3;


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

	public static void miseEnBrillance(int etat, Transform tranformSelectionnable){
		Transform tfmGlow = tranformSelectionnable.Find ("GlowLayer");
	
		//S'il n'y pas de transform, et que l'état en necessite 1 on le crée
		if (null == tfmGlow && etat != SelectionnableUtils.ETAT_NON_SELECTION) {
			GameObject goGlow = GameObject.CreatePrimitive(PrimitiveType.Plane);
			goGlow.name = "GlowLayer";
			goGlow.transform.SetParent (tranformSelectionnable);
			goGlow.transform.localPosition = Vector3.zero;
			goGlow.transform.localRotation = Quaternion.identity;
			goGlow.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
			tfmGlow = goGlow.transform;
		}

		if(null != tfmGlow){

			if (etat == SelectionnableUtils.ETAT_NON_SELECTION) {//Supprime la transform
				GameObject.Destroy (tfmGlow.gameObject);
			} else if (etat == SelectionnableUtils.ETAT_SELECTIONNABLE) {
				Material matSelectionnable = new Material (ConstanteInGame.materialGlow);
				matSelectionnable.SetColor("_TintColor", Color.yellow);
				tfmGlow.gameObject.GetComponent<MeshRenderer> ().material = matSelectionnable;
			} else if (etat == SelectionnableUtils.ETAT_MOUSE_OVER) {
				Material matSelectionnable = new Material (ConstanteInGame.materialGlow);
				matSelectionnable.SetColor("_TintColor",Color.white);
				tfmGlow.gameObject.GetComponent<MeshRenderer> ().material = matSelectionnable;
			} else if (etat == SelectionnableUtils.ETAT_SELECTIONNE) {
				Material matSelectionnable = new Material (ConstanteInGame.materialGlow);
				matSelectionnable.SetColor("_TintColor", Color.green);
				tfmGlow.gameObject.GetComponent<MeshRenderer> ().material = matSelectionnable;
			}

		}
	}
}
