using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GenerateObjectUtils {

	public static TextMesh createText (string nameGO, Vector3 position, Quaternion rotation, Vector3 taille,int sizeText, GameObject goParent){
		GameObject cadreGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
		cadreGO.name = nameGO;
		cadreGO.transform.SetParent (goParent.transform);
		cadreGO.transform.localPosition = position;
		cadreGO.transform.localRotation = rotation ;
		cadreGO.transform.localScale = taille/ ConstanteInGame.coefPlane;

		GameObject textGO = new GameObject("txt" + nameGO);
		textGO.transform.SetParent (cadreGO.transform);
		textGO.transform.localPosition = new Vector3(0,0.01f,0);
		textGO.transform.localScale = new Vector3(1,1/(taille.z / ConstanteInGame.coefPlane)/10,1);
		textGO.transform.localRotation = Quaternion.identity ;
		textGO.transform.Rotate (new Vector3 (90, 0, 0));

		TextMesh text = textGO.AddComponent<TextMesh> ();
		text.font = ConstanteInGame.fontChintzy;
		text.color = Color.black;
		text.fontSize = sizeText;
		text.anchor = TextAnchor.MiddleCenter;

		textGO.GetComponent<Renderer> ().material = ConstanteInGame.matChintzy;

		return text;
	}

}
