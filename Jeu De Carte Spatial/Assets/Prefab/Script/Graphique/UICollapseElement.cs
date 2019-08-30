using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO probleme si changement ecran
public class UICollapseElement : MonoBehaviour {

	[SerializeField]
	private string titre;

	[SerializeField]
	private string description;

	[SerializeField]
	private Vector2 ancreSuperieur;

	[SerializeField]
	private int tailleTitre;

	[SerializeField]
	private int tailleDescription;

	[SerializeField]
	private float tempsDecompression;

	[SerializeField]
	private bool collapse;


	private Text txtTitre;

	private Text txtDescription;

	private Button buttonAction;

	private RectTransform rectTitre;

	private RectTransform rectDescription;

	private float heightParent;

	private float witdhParent;

	private bool onChange;

	private UICollapseGroup collapseGroup;

	// Use this for initialization
	public void initialisationEmement (UICollapseGroup groupParent) {
		this.collapseGroup = groupParent;
		onChange = false;

		heightParent = gameObject.GetComponent<RectTransform>().rect.height;
		witdhParent = gameObject.GetComponent<RectTransform>().rect.width;

		GameObject panelTitre = UIUtils.createPanelAnchorCenterHigh ("Titre_UICollapseElement", gameObject, ancreSuperieur.x, ancreSuperieur.y, witdhParent, tailleTitre);
		rectTitre = panelTitre.GetComponent<RectTransform> ();
		txtTitre = UIUtils.createTextStretch ("Titre_Texte_UICollapseElement", panelTitre,(int) (rectTitre.sizeDelta.y * .75f / 2), 5, 5, 5, 5);
		txtTitre.text = titre;
		Button boutonCollapse = panelTitre.AddComponent<Button> ();
		boutonCollapse.onClick.AddListener (collapseChange);

		//On ancre le panel de description au centre en haut à la limite du panel titre
		GameObject panelDescription = UIUtils.createPanelAnchorCenterHigh ("Description_UICollapseElement", gameObject, ancreSuperieur.x, ancreSuperieur.y-tailleTitre, witdhParent, collapse? 0 : tailleDescription);
		rectDescription = panelDescription.GetComponent<RectTransform> ();

		//On ancre le text au centre et avec une hauteur extensible
		txtDescription = UIUtils.createTextStretch ("Description_Texte_UICollapseElement", rectDescription.gameObject,(int) (rectDescription.sizeDelta.y * .75f / 5), 5, 5, 5, 5);
		txtDescription.text = description;
		txtDescription.fontSize = (int)(tailleDescription * 3 / 20);
		//Distance entre borne parent et enfant
		//Rect rect
		//rectTxtDescription.yMin = 5;
		//rectTxtDescription.rect.yMax = 5;

		GameObject buttonGO = UIUtils.createPanel ("Button_UICollapseElement", panelTitre, witdhParent * 3 / 8, 0, witdhParent/8, tailleTitre/2);
		buttonAction = buttonGO.AddComponent<Button> ();

	}

	void collapseChange()
	{
		if (!onChange) {
			onChange = true;

			if (null != collapseGroup) {
				collapseGroup.groupReatction ();
			}

			if (collapse) {
				StartCoroutine (deployElement ());
			} else {
				StartCoroutine (collapseElement ());
			}
		}
	}

	IEnumerator deployElement(){
		float tempsRestant = tempsDecompression;
		Vector2 tailleRectangleDescription;

			
		while (tempsRestant>0){
			float tailleDescription = this.tailleDescription * (tempsDecompression - tempsRestant) / tempsDecompression;

			tailleRectangleDescription = new Vector2 (rectDescription.sizeDelta.x, tailleDescription);
			rectDescription.sizeDelta = tailleRectangleDescription;
			rectDescription.localPosition = new Vector3(ancreSuperieur.x, ancreSuperieur.y + (heightParent - tailleDescription)/2 - rectTitre.rect.height);

			tempsRestant -= Time.deltaTime;
			yield return null;
		} 
			
		tailleRectangleDescription = new Vector2 (rectDescription.sizeDelta.x, this.tailleDescription);
		rectDescription.sizeDelta = tailleRectangleDescription;
		rectDescription.localPosition = new Vector3(ancreSuperieur.x,ancreSuperieur.y + (heightParent - this.tailleDescription)/2 - rectTitre.rect.height);

		collapse = false;
		onChange = false;
	}

	public IEnumerator collapseElement(){
		float tempsRestant = tempsDecompression;
		Vector2 tailleRectangleDescription;


		while (tempsRestant>0){
			float tailleDescription = this.tailleDescription * tempsRestant / tempsDecompression;

			tailleRectangleDescription = new Vector2 (rectDescription.sizeDelta.x, tailleDescription);
			rectDescription.sizeDelta = tailleRectangleDescription;
			rectDescription.localPosition = new Vector3(ancreSuperieur.x, ancreSuperieur.y + (heightParent - tailleDescription)/2 - rectTitre.rect.height);

			tempsRestant -= Time.deltaTime;
			yield return null;
		} 
			
		tailleRectangleDescription = new Vector2 (rectDescription.sizeDelta.x, 0);
		rectDescription.sizeDelta = tailleRectangleDescription;
		rectDescription.localPosition = new Vector3(ancreSuperieur.x,ancreSuperieur.y + heightParent/2 - rectTitre.rect.height);

		collapse = true;
		onChange = false;
	}

	public IEnumerator moveTitle(Vector2 newAnchor, Vector2 newSize){
		float tempsRestant = tempsDecompression;
		Vector2 tailleRectangleDescription;
		Vector2 tailleTitreInitial = new Vector2 (rectTitre.rect.width, rectTitre.rect.height);
		Vector2 ancreTitreInitial = new Vector2 (rectTitre.localPosition.x, rectTitre.localPosition.y);


		while (tempsRestant>0){

			rectTitre.sizeDelta = newSize + (tailleTitreInitial - newSize ) * tempsRestant / tempsDecompression;
			rectTitre.localPosition = newAnchor + (ancreTitreInitial - newAnchor ) * tempsRestant / tempsDecompression;

			ancreSuperieur = new Vector2 (rectTitre.localPosition.x, rectTitre.localPosition.y + rectTitre.sizeDelta.y / 2 - heightParent/2);

			txtTitre.fontSize = (int)(rectTitre.sizeDelta.y * .75f / 2);

			tempsRestant -= Time.deltaTime;
			yield return null;
		} 
			
		rectTitre.sizeDelta = newSize;
		rectTitre.localPosition = newAnchor;

		ancreSuperieur = new Vector2 (newAnchor.x, newAnchor.y + newSize.y / 2 - heightParent/2);

		txtTitre.fontSize = (int)(newSize.y * .75f / 2);
	}
		
	public Vector2 AncreSuperieur{ 
		get{return ancreSuperieur;}
		set{ancreSuperieur = value;}
	}

	public int TailleTitre{  
		get{return tailleTitre;}
		set{tailleTitre = value;}
		}

	public int TailleDescription{  
		get{return tailleDescription;}
		set{tailleDescription = value;}
		}

	public float TempsDecompression{ 
		get{return tempsDecompression;}
		set{tempsDecompression = value;}
		}

	public bool Collapse{ 
		get{return collapse;}
		set{collapse = value;}
		}

	public bool OnChange{ 
		get{return onChange;}
	}

	public string Titre{  
		get{return titre;}
		set{titre = value;}
	}

	public string Description{  
		get{return description;}
		set{description = value;}
	}

	public Button BoutonAction {
		get { return buttonAction; }
	}
}
