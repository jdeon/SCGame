using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransfertUtils {

	/*[Command]
	public void CmdTirerCarte(){
		GameObject carteTiree = deckConstruction.tirerCarte ();

		carteTiree.transform.SetParent(main.transform);

		int nbCarteEnMains = main.transform.childCount;

		carteTiree.transform.localPosition = new Vector3 ( carteTiree.transform.localScale.x * (nbCarteEnMains - .5f), 0, 0);
		carteTiree.transform.Rotate (new Vector3 (-60, 0) + main.transform.rotation.eulerAngles);

		NetworkServer.Spawn (carteTiree);

		CarteConstructionMetierAbstract carteConstructionScript = carteTiree.GetComponent<CarteConstructionMetierAbstract> ();
		byte[] carteRefData = SerializeUtils.SerializeToByteArray(carteConstructionScript.getCarteRef());
		RpcGenerate(carteTiree, carteRefData);
	}*/

	/*[ClientRpc]
	public void RpcGenerate(GameObject goScript, byte[] dataObject)
	{
		CarteConstructionDTO carteRef = null;

		carteRef = SerializeUtils.Deserialize<CarteConstructionDTO> (dataObject);

		CarteConstructionMetierAbstract carteConstructionScript = goScript.GetComponent<CarteConstructionMetierAbstract> ();
		carteConstructionScript.initCarte (carteRef);
		carteConstructionScript.generateGOCard ();
	}*/

}
