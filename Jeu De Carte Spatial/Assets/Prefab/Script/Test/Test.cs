using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Test : NetworkBehaviour {

	public GameObject prefabSpawn;

	// Update is called once per frame
	void Update () {
		if (isLocalPlayer && Input.GetKeyDown(KeyCode.Space)) {
			CmdFire();
		}
	}

	[Command]
	void CmdFire (){
		GameObject objectSpawned = Instantiate<GameObject> (prefabSpawn);
		objectSpawned.transform.SetParent (transform);

		/*GameObject objectChildSpawned = Instantiate<GameObject> (prefabSpawn);
		objectChildSpawned.transform.SetParent (objectSpawned.transform);
		objectChildSpawned.transform.Rotate(new Vector3 (90, 180, 0));

		/*GameObject goText = Instantiate<GameObject> (ConstanteInGame.textPrefab);
		goText.name = "goText";
		goText.transform.SetParent (objectChildSpawned.transform);
		TextMesh text = goText.GetComponent<TextMesh> ();
		text.color = Color.black;
		text.font = ConstanteInGame.fontArial;
		text.fontSize = 20;
		text.text = "BLABLAbla";*//*
		//goText.AddComponent<NetworkIdentity> ();

		//ClientScene.RegisterPrefab(goText);


		objectSpawned.GetComponent<Rigidbody> ().velocity = new Vector3 (6, 0);

		NetworkServer.Spawn (objectSpawned);
		NetworkServer.Spawn (objectChildSpawned);
		NetworkServer.Spawn (goText);
		*/
		//RpcSyncBlockOnce (goText.transform.localPosition, goText.transform.localRotation, goText, goText.transform.parent.gameObject);

		TestScriptObject testScript = objectSpawned.GetComponent<TestScriptObject> ();
		testScript.x = 2;
		testScript.y = 3;
		testScript.z = 4;

		NetworkServer.Spawn (objectSpawned);

		RpcGenerate (objectSpawned);

		//RpcSyncBlockOnce (goText.transform.localPosition, goText.transform.localRotation, goText, goText.transform.parent.gameObject);


		Destroy (objectSpawned, 2);
	}

	[ClientRpc]
	public void RpcGenerate(GameObject goScript)
	{
		TestScriptObject testScript = goScript.GetComponent<TestScriptObject> ();
		testScript.generateCube (goScript);
	}

	/*[ClientRpc]
	public void RpcSyncBlockOnce(Vector3 localPos, Quaternion localRot, GameObject block, GameObject parent)
	{
		block.transform.parent = parent.transform;
		block.transform.localPosition = localPos;
		block.transform.localRotation = localRot;
	}*/

}
