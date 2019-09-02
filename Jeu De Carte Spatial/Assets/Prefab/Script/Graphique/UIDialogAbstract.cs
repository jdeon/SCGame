using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class  UIDialogAbstract {

	protected GameObject goDialog;

	protected string descriptionAction;

	protected float height;
	protected float width;

	public UIDialogAbstract(string descriptionAction){
		this.descriptionAction = descriptionAction;

		GameObject goCanvas = UIUtils.getCanvas ();
		goDialog = new GameObject ("ConfirmDialog");
		goDialog.AddComponent<CanvasRenderer> ();
		goDialog.transform.SetParent (goCanvas.transform);
		goDialog.transform.localPosition = Vector3.zero;

		Image i = goDialog.AddComponent<Image> ();
		i.sprite = ConstanteInGame.spriteBackgroundCarte;

		Vector2 vUICardSize = UIUtils.getUICardSize ();
		height = vUICardSize.x / 3f;
		width = vUICardSize.y * .75f;
		goDialog.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, height);
		Text txtDialog = UIUtils.createText ("TextDialog", goDialog, 3, 0, .2f, .9f * width, height*.6f);
		txtDialog.text = descriptionAction;

		goDialog.SetActive (false);

	}

	public void showDialog(){
		goDialog.SetActive (true);
	}

	public void hideDialog(){
		GameObject.Destroy(goDialog);
	}

	public Vector3 Anchor {
		get { return goDialog.transform.localPosition; }
		set { goDialog.transform.localPosition = value; }
	}

	public Vector2 Size {
		get { return goDialog.GetComponent<RectTransform> ().sizeDelta; }
	}
}
