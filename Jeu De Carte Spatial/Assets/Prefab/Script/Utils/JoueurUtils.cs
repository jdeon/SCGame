using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JoueurUtils {

	public static Joueur getJoueurLocal(){
		Joueur joueurResult = null;

		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();

		if (null != listJoueur && listJoueur.Length > 0) {
			foreach (Joueur joueur in listJoueur) {
				if (joueur.isLocalPlayer) {
					joueurResult = joueur;
					break;
				}
			}
		}

		return joueurResult;
	}


	public static Joueur getJoueur(NetworkInstanceId netIdJoueur){
		Joueur joueurResult = null;

		Joueur[] listJoueur = GameObject.FindObjectsOfType<Joueur> ();

		if (null != listJoueur && listJoueur.Length > 0) {
			foreach (Joueur joueur in listJoueur) {
				if (joueur.netId == netIdJoueur) {
					joueurResult = joueur;
					break;
				}
			}
		}

		return joueurResult;
	}
}
