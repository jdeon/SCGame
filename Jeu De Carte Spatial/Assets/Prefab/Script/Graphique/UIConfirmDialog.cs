﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConfirmDialog : UIDialogAbstract {

	private Button btnValid;

	private Button btnCancel;

	public UIConfirmDialog(string descriptionAction) : base (descriptionAction) {

		GameObject goBoutonValid = UIUtils.createPanel("BoutonValid",goDialog,width*(-0.25f),height*(-.25f), width*.25f,height*.25f);
		Text txtValid = UIUtils.createText ("TextValidDialog", goBoutonValid, 1, 0, 0, .9f * width * .25f, height * .25f);
		txtValid.text = "Valider";
		btnValid = goBoutonValid.AddComponent<Button> ();

		GameObject goBoutonCancel = UIUtils.createPanel("BoutonCancel",goDialog,width*(0.25f),height*(-.25f), width*.25f,height*.25f);
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

	public Button BtnValid {
		get{ return btnValid; }
	}

	public Button BtnCancel{
		get{ return btnCancel; }
	}
}
