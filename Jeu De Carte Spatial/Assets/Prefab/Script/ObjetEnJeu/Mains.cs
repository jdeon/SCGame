using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mains : MonoBehaviour, IConteneurCarte {

	private NetworkInstanceId netIdJoueurPossesseur;

	private List<CarteMetierAbstract> carteEnMains;

	public void init(NetworkInstanceId netIdJoueur){
		netIdJoueurPossesseur = netIdJoueur;
		carteEnMains = new List<CarteMetierAbstract> ();
	}

	public void putCard(CarteMetierAbstract carteAdded){
		carteEnMains.Add (carteAdded);
		carteAdded.transform.SetParent(transform);

		int nbCarteEnMains = transform.childCount;

		Vector3 position = Vector3.zero;
		if(carteAdded is CarteConstructionMetierAbstract){
			//On ajoute a gauche
			position.x = carteAdded.transform.localScale.x * (nbCarteEnMains - .5f);
		} else if (carteAdded is CarteMetierAbstract){
			//TODO nbCarte ne doit compter separement les ameliration et les construction
			position.x = -carteAdded.transform.localScale.x * (nbCarteEnMains - .5f);
		}

		carteAdded.transform.localPosition = position;
		carteAdded.transform.Rotate (new Vector3 (-60, 0) + transform.rotation.eulerAngles);
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
}
