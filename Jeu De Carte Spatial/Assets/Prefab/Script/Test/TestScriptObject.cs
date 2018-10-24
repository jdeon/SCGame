using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestScriptObject : NetworkBehaviour {

	[SyncVar]
	public float x;

	[SyncVar]
	public float y;

	[SyncVar]
	public float z;
	
	public void generateCube(GameObject goParent){
		GameObject cubeGo = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cubeGo.transform.SetParent (goParent.transform);
		cubeGo.transform.localScale = new Vector3 (x, y, z);
	}
}
