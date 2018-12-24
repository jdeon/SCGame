using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IConteneurCarte {

	bool isConteneurAllier (NetworkInstanceId netIdJoueur);

	List<CarteMetierAbstract> getCartesContenu ();

	void putCard (CarteMetierAbstract cartePoser);
}
