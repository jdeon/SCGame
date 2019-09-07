using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollapseUseCapa : UICollapseElement {

	private int numLvl;
	private CarteConstructionMetierAbstract carteSource;
	private List<CapaciteMannuelleDTO> listCapaManuelle;

	private List<UIDialogInfo> listDialogOUverte = new List<UIDialogInfo> ();


	public void gestionBoutonNiveau(Text textBtnLvl){
		bool actionTrouve = false;
		foreach (CapaciteMannuelleDTO capaManuelle in listCapaManuelle) {
			if (capaManuelle.PeriodeUtilisable.Contains ("5-A")) { //TODO vérifier l'ID periode
				actionTrouve = true;
				break;
			}
		}

		if (actionTrouve) {
			textBtnLvl.text = "Use";
			BoutonAction.onClick.RemoveAllListeners ();
			BoutonAction.onClick.AddListener (useCapa);
			BoutonAction.interactable = true;
		} else {
			BoutonAction.gameObject.SetActive (false);
		}
	}

	private void useCapa(){
		List<CapaciteMannuelleDTO> capaciteUtilisable = new List<CapaciteMannuelleDTO> ();
		foreach (CapaciteMannuelleDTO capaManuelle in listCapaManuelle) {
			if(capaManuelle.PeriodeUtilisable.Contains("5-A")){ //TODO vérifier l'ID periode
				capaciteUtilisable.Add(capaManuelle);
			}
		}

		if (capaciteUtilisable.Count > 1) {
			showChoiceCapa (capaciteUtilisable);
		} else {
			CapaciteManuelleUtils.useCapacite(carteSource, numLvl, 0);
		}
	}

	private void useCapa(int index){
		CapaciteManuelleUtils.useCapacite (carteSource, numLvl, index);
	}
	

	private void showChoiceCapa (List<CapaciteMannuelleDTO> listCapaciteUtilisable){
		int nbOption = listCapaciteUtilisable.Count + 1;

		for(int i = 0 ; i < listCapaciteUtilisable.Count; i++){
			UIDialogInfo choixDialog = new UIDialogInfo (listCapaciteUtilisable[i].LibelleCarte);
			Vector3 baseAnchor = choixDialog.Anchor;

			float anchorX = -1f * choixDialog.Size.x * (nbOption - (1f+ 2*i)) / 2f; 
			choixDialog.Anchor = new Vector3 (anchorX, baseAnchor.y, baseAnchor.z);

			choixDialog.TextBtnCancel.text = "Use";
			choixDialog.BtnCancel.onClick.RemoveAllListeners ();
			addListenerWithParam (choixDialog.BtnCancel, i);
			choixDialog.BtnCancel.onClick.AddListener (fermerToutChoix);

			choixDialog.showDialog ();

			listDialogOUverte.Add (choixDialog);
		}

		UIDialogInfo cancelChoixDialog = new UIDialogInfo ("Utiliser aucune capacite");
		Vector3 baseCancelAnchor = cancelChoixDialog.Anchor;

		float anchorCancelX = cancelChoixDialog.Size.x * (nbOption - 1f) / 2f; 
		cancelChoixDialog.Anchor = new Vector3 (anchorCancelX, baseCancelAnchor.y, baseCancelAnchor.z);

		cancelChoixDialog.BtnCancel.onClick.RemoveAllListeners ();
		cancelChoixDialog.BtnCancel.onClick.AddListener (fermerToutChoix);

		cancelChoixDialog.showDialog ();
		listDialogOUverte.Add (cancelChoixDialog);
	}

	private void fermerToutChoix(){
		while(listDialogOUverte.Count > 0){
			UIDialogInfo dialogAFermer = listDialogOUverte [0];
			listDialogOUverte.RemoveAt (0);
			dialogAFermer.hideDialog ();
		}
	}

	//Methode permetante de sortir le int du context pour mettre la valeur dans une nouvelle variable sinon erreur
	private void addListenerWithParam(Button bouton, int param){
		bouton.onClick.AddListener (delegate {useCapa (param);});
	}

	public int NumLvl {
		get{ return numLvl; }
		set{ numLvl = value; }
	}

	public List<CapaciteMannuelleDTO> ListCapaManuelle{
		get{ return listCapaManuelle; }
		set{ listCapaManuelle = value; }
	}

	public CarteConstructionMetierAbstract CarteSource{
		get{ return carteSource; }
		set{ carteSource = value;}
 	}
}
