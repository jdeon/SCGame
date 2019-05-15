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
		if (null != carteAdded && null != carteAdded.getJoueurProprietaire () && carteAdded.getJoueurProprietaire ().isLocalPlayer) {
			carteEnMains.Add (carteAdded);
			carteAdded.transform.SetParent (transform);

			carteAdded.CmdChangeParent (netIdJoueurPossesseur, JoueurUtils.getPathJoueur (this));

			int nbCarteEnMains = transform.childCount;

			Vector3 position = Vector3.zero;
			if (carteAdded is CarteConstructionMetierAbstract) {
				//On ajoute a gauche
				position.x = carteAdded.transform.localScale.x * (nbCarteEnMains - .5f);
			} else if (carteAdded is CarteMetierAbstract) {
				//TODO nbCarte ne doit compter separement les ameliration et les construction
				position.x = -carteAdded.transform.localScale.x * (nbCarteEnMains - .5f);
			}

			carteAdded.transform.localPosition = position;
			carteAdded.transform.Rotate (new Vector3 (-60, 0) + transform.rotation.eulerAngles);
		}
	}

	public void removeCarte(CarteMetierAbstract carteToRemove){
		carteEnMains.Remove (carteToRemove);
		//TODO
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

	public void miseEnBrillance(int etat){
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
	}

	public NetworkInstanceId NetIdJoueur {
		get{ return netIdJoueurPossesseur;}
	}
}
