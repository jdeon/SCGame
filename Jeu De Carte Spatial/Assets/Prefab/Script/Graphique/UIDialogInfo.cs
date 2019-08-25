using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogInfo : UIDialogAbstract {

	private Button btnCancel;

	public UIDialogInfo(string descriptionAction) : base (descriptionAction) {

		GameObject goBoutonCancel = UIUtils.createPanel("BoutonCancel",goDialog,0f,height*(-.25f), width*.25f,height*.25f);
		Text txtCancel = UIUtils.createText ("TextValidCancel", goBoutonCancel, 1, 0, 0, .9f * width * .25f, height * .25f);
		txtCancel.text = "Retour";
		btnCancel = goBoutonCancel.AddComponent<Button> ();
		btnCancel.onClick.AddListener (hideDialog);

		goDialog.SetActive (false);

	}

	public void showDialog(){
		goDialog.SetActive (true);
	}

	public void hideDialog(){
		GameObject.Destroy(goDialog);
	}

	public Button BtnCancel{
		get{ return btnCancel; }
	}
}