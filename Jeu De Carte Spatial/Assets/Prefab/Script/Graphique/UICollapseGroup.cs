using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollapseGroup : MonoBehaviour {

	[SerializeField]
	private List <UICollapseElement> listCollapseElement;




	private bool initialize = false;

	public void initializeGroup (){
		if (!initialize) {
			int totalTailleElement = 0;

			RectTransform rectTrans = gameObject.GetComponent<RectTransform> ();
			Vector2 taillePanelParent = new Vector2(rectTrans.rect.width,rectTrans.rect.height);

			//On determine la taille total de tous les titre
			foreach (UICollapseElement collapseElement in listCollapseElement) {
				totalTailleElement += collapseElement.TailleTitre;
			}

			float rapportTailleElementParent = totalTailleElement > 0 ? taillePanelParent.y / totalTailleElement : 0;

			//Redéfinition de taille de titre
			Vector2 ancreCoordonne = new Vector2();
			foreach (UICollapseElement collapseElement in listCollapseElement) {
				int nouvelleTaille = (int) (collapseElement.TailleTitre * rapportTailleElementParent);
				collapseElement.TailleTitre = nouvelleTaille;
				collapseElement.AncreSuperieur = ancreCoordonne;
				collapseElement.Collapse = true;
				collapseElement.initialisationEmement (this);
				ancreCoordonne.y -= nouvelleTaille;
			}
		
			initialize = true;
		}
	}

	public void groupReatction(){
		//On determine la taille total de tous les titre
		int totalTailleElement = 0;
		int numElement = 1;
		int numElementDeploy = 0;

		RectTransform rectTrans = gameObject.GetComponent<RectTransform> ();
		Vector2 taillePanelParent = new Vector2(rectTrans.rect.width,rectTrans.rect.height);

		foreach (UICollapseElement collapseElement in listCollapseElement) {
			totalTailleElement += collapseElement.TailleTitre;
			if (collapseElement.OnChange && collapseElement.Collapse) {
				numElementDeploy = numElement;
			} else if(!collapseElement.OnChange && !collapseElement.Collapse){
				StartCoroutine (collapseElement.collapseElement ());
			}
			numElement++;
		}

		float rapportTailleElementParent = totalTailleElement > 0 ? taillePanelParent.y / totalTailleElement : 0;

		//l'élément changeant est refermé
		if (numElementDeploy == 0) {
			//Redéfinition de taille de titre
			Vector2 ancreCoordonne = new Vector2 (0,taillePanelParent.y/2);
			foreach (UICollapseElement collapseElement in listCollapseElement) {
				int nouvelleTaille = (int)(collapseElement.TailleTitre * rapportTailleElementParent);
				ancreCoordonne.y -= nouvelleTaille/2;

				StartCoroutine(collapseElement.moveTitle(ancreCoordonne,new Vector2(taillePanelParent.x,nouvelleTaille)));

				ancreCoordonne.y -= nouvelleTaille/2;
			}
		} else {
			//Bord supérieur panel parent
			Vector2 ancreCoordonne = new Vector2 (0,taillePanelParent.y/2);
			int i = 1;
			foreach (UICollapseElement collapseElement in listCollapseElement) {
				int nouvelleTaille = (int)(collapseElement.TailleTitre * rapportTailleElementParent);
				nouvelleTaille -= listCollapseElement[numElementDeploy-1].TailleDescription / listCollapseElement.Count;
				ancreCoordonne.y -= nouvelleTaille / 2;

				StartCoroutine(collapseElement.moveTitle(ancreCoordonne,new Vector2(taillePanelParent.x,nouvelleTaille)));

				ancreCoordonne.y -= nouvelleTaille/2;
				if (i == numElementDeploy) {
					ancreCoordonne.y -= listCollapseElement[numElementDeploy-1].TailleDescription;
				}
				i++;
			}
		}

	}

	public List<UICollapseElement> ListCollapseElement{ 
		get{return listCollapseElement;}
		set{listCollapseElement = value;}
	}
}
