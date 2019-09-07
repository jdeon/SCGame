using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mains : MonoBehaviour, IConteneurCarte, ISelectionnable {

	private NetworkInstanceId netIdJoueurPossesseur;

	private List<CarteMetierAbstract> carteEnMains;

	private int idSelectionnable;

	private int etatSelection;

	public void init(Joueur joueurPossesseur){
		netIdJoueurPossesseur = joueurPossesseur.netId;
		carteEnMains = new List<CarteMetierAbstract> ();

		if (joueurPossesseur.isServer) {
			idSelectionnable = ++SelectionnableUtils.sequenceSelectionnable;
			joueurPossesseur.RpcInitMainIdSelectionnable (idSelectionnable);
		}
	}
		
	public void putCard(CarteMetierAbstract carteAdded){

		//CarteMetierAbstract carteAdded = ConvertUtils.convertNetIdToScript<CarteMetierAbstract> (netIdcarteAdded, true);
		if (null != carteAdded && null != carteAdded.JoueurProprietaire && carteAdded.JoueurProprietaire.isLocalPlayer) {
			carteEnMains.Add (carteAdded);
			carteAdded.transform.parent = transform;

			carteAdded.CmdChangeParent (netIdJoueurPossesseur, JoueurUtils.getPathJoueur (this));

			int nbCarteEnMains = transform.childCount;

			Vector3 position = Vector3.zero;
			if (carteAdded is CarteConstructionMetierAbstract) {
				//On ajoute a gauche
				position.x =  (nbCarteEnMains - .5f) /* * carteAdded.transform.localScale.x*/;
			} else if (carteAdded is CarteMetierAbstract) {
				//TODO nbCarte ne doit compter separement les ameliration et les construction
				position.x = -(nbCarteEnMains - .5f) /* * carteAdded.transform.localScale.x*/;
			}

			carteAdded.transform.localPosition = position;
			carteAdded.transform.Rotate (new Vector3 (-60, 0) + transform.rotation.eulerAngles);
		}
	}

	public void removeCarte(CarteMetierAbstract carteToRemove){
		carteEnMains.Remove (carteToRemove);

		float position = carteToRemove.transform.position.x;

		foreach (CarteMetierAbstract carte in carteEnMains) {
			float positionxCarte = carte.transform.position.x;
			if (position > 0 && positionxCarte > position) {
				carte.transform.position = new Vector3(positionxCarte - 1f, carte.transform.position.y, carte.transform.position.z);
			} else if (position < 0 && positionxCarte < position) {
				carte.transform.position = new Vector3(positionxCarte + 1f, carte.transform.position.y, carte.transform.position.z);
			}
		}
	}


	/*****************	IContenerCarte *****************/

	public bool isConteneurAllier (NetworkInstanceId netIdJoueur){
		return netIdJoueur == this.netIdJoueurPossesseur;
	}

	public List<CarteMetierAbstract> getCartesContenu (){
		return new List<CarteMetierAbstract> (transform.GetComponentsInChildren<CarteMetierAbstract> ());
	}


	/***************** ISelectionnable ******************/
	public void onClick (){
		//TODO
	}

	public int IdISelectionnable{ 
		get{return idSelectionnable;}
		set{
			if (null == idSelectionnable || idSelectionnable <= 0) {
				idSelectionnable = value;
			}
		}
	}

	public int EtatSelectionnable { 
		get{ return etatSelection; }
		set {
			if (value == SelectionnableUtils.ETAT_RETOUR_ATTIERE) {
				SelectionnableUtils.miseEnBrillance (etatSelection, transform);
			} else {
				SelectionnableUtils.miseEnBrillance (value, transform);
				if (value != SelectionnableUtils.ETAT_MOUSE_OVER) {
					etatSelection = value;
				}
			}
		}
	}


	public NetworkInstanceId Possesseur { 
		get { return netIdJoueurPossesseur; }
	}

	public NetworkInstanceId NetIdJoueur {
		get{ return netIdJoueurPossesseur;}
	}
}
