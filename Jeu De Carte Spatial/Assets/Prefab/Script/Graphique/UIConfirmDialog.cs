using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConfirmDialog {

	private GameObject goDialog;

	private string descriptionAction;

	private Button btnValid;

	private Button btnCancel;

	public UIConfirmDialog(string descriptionAction){
		this.descriptionAction = descriptionAction;

		GameObject goCanvas = UIUtils.getCanvas ();
		goDialog = new GameObject ("ConfirmDialog");
		goDialog.AddComponent<CanvasRenderer> ();
		goDialog.transform.SetParent (goCanvas.transform);
		goDialog.transform.localPosition = Vector3.zero;

		Image i = goDialog.AddComponent<Image> ();
		i.sprite = ConstanteInGame.spriteBackgroundCarte;

		Vector2 vUICardSize = UIUtils.getUICardSize ();
		float height = vUICardSize.x / 3f;
		float width = vUICardSize.y * .75f;
		goDialog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		Text txtDialog = UIUtils.createText ("TextDialog", goDialog, 3, 0, .2f, .9f * width, height*.6f);
		txtDialog.text = descriptionAction;

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
