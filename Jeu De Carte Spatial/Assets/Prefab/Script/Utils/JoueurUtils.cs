using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JoueurUtils {

	private static Dictionary<NetworkInstanceId, Joueur> allJoueurByNetId = new Dictionary<NetworkInstanceId, Joueur>();

	public static void initAllJoueurDictionnary(){
		allJoueurByNetId.Clear ();

		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();

		if (null != listJoueur && listJoueur.Length > 0) {
			foreach (Joueur joueur in listJoueur) {
				allJoueurByNetId.Add (joueur.netId, joueur);
			}
		}
	}

	public static Joueur getJoueurLocal(){
		Joueur joueurResult = null;

		if (null == allJoueurByNetId || allJoueurByNetId.Count == 0) {
			initAllJoueurDictionnary ();
		}

		foreach (Joueur joueur in allJoueurByNetId.Values) {
			if (joueur.isLocalPlayer) {
				joueurResult = joueur;
				break;
			}
		}

		return joueurResult;
	}


	public static Joueur getJoueur(NetworkInstanceId netIdJoueur){
		Joueur joueurResult = null;

		if (null == allJoueurByNetId || allJoueurByNetId.Count == 0) {
			initAllJoueurDictionnary ();
		}

		allJoueurByNetId.TryGetValue (netIdJoueur, out joueurResult);

		return joueurResult;
	}

	public static string getPathJoueur(MonoBehaviour scriptOfPath){
		string resultPath;

		if (scriptOfPath is Joueur) {
			resultPath = "";
		} else if (scriptOfPath is Mains) {
			resultPath = ConstanteInGame.strMainJoueur;
		} else if (scriptOfPath is DeckConstructionMetier){
			resultPath = ConstanteInGame.strPiocheConstruction;
		} else if (scriptOfPath is DeckMetierAbstract){ //TODO remplacer par amelioration
			resultPath = ConstanteInGame.strPiocheAmelioration;
		} else {
			resultPath = null;
		}

		return resultPath;
	}
}
