using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceUtils  {

	public static readonly int STOCK_METAL_DEBUT_PARTIE = 5;

	public static readonly int PROD_METAL_DEBUT_PARTIE = 1;

	public static readonly int STOCK_CARBURANT_DEBUT_PARTIE = 3;

	public static readonly int PROD_CARBURANT_DEBUT_PARTIE = 1;

	public static readonly int STOCK_NIVEAU_DEBUT_PARTIE = 1;

	public static readonly int STOCK_XP_DEBUT_PARTIE = 0;

	public static readonly int PROD_XP_PAR_TOUR = 1;

	public static readonly int PROD_XP_PAR_DEGAT = 1;

	public static readonly int PROD_XP_DESTRUCTION_PAR_NIVEAU = 3;

	public static string getPrefixeProd(string typeRessource){
		string result;

		if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_METAL){
			result = "Prod M -";
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_CARBU){
			result = "Prod C - ";
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_XP){
			result = "XP - ";
		} else {
			result = "";
		}

		return result;
	}

	public static string getPrefixeStock(string typeRessource){
		string result;

		if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_METAL){
			result = "Stock M -";
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_CARBU){
			result = "Stock C - ";
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_XP){
			result = "Niv - ";
		} else {
			result = "";
		}

		return result;
	}

	public static int getValeurProdInitialeRessource(string typeRessource){
		int result;

		if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_METAL){
			result = PROD_METAL_DEBUT_PARTIE;
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_CARBU){
			result = PROD_CARBURANT_DEBUT_PARTIE;
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_XP){
			result = STOCK_XP_DEBUT_PARTIE;
		} else {
			result = 0;
		}

		return result;
	}

	public static int getValeurStockInitialeRessource(string typeRessource){
		int result;

		if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_METAL){
			result = STOCK_METAL_DEBUT_PARTIE;
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_CARBU){
			result = STOCK_CARBURANT_DEBUT_PARTIE;
		} else if(typeRessource == ConstanteInGame.STR_TYPE_RESSOURCE_XP){
			result = STOCK_NIVEAU_DEBUT_PARTIE;
		} else {
			result = 0;
		}

		return result;
	}

	public static void gainXPDegat (int nbDegat, Joueur joueur){
		joueur.CmdGainXP (nbDegat * PROD_XP_PAR_DEGAT);
	}

	public static void gainXPDestruction (int numNiveauDetruit, Joueur joueur){
		joueur.CmdGainXP (numNiveauDetruit * PROD_XP_DESTRUCTION_PAR_NIVEAU);
	}
}
