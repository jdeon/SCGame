using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstanteIdObjet {

	/*********************************ID_CAPACITE**********************/
	public static readonly int ID_CAPACITE_MODIF_LVL = 1;

	public static readonly int ID_CAPACITE_MODIF_XP = 2;

	public static readonly int ID_CAPACITE_MODIF_PRODUCTION_RESSOURCE = 3;

	public static readonly int ID_CAPACITE_MODIF_STOCK_RESSOURCE = 4;

	public static readonly int ID_CAPACITE_MODIF_COUT_CONSTRUCTION = 5;

	//public static readonly int ID_CAPACITE_MODIF_PRODUCTION_CARBURANT = 6;

	//public static readonly int ID_CAPACITE_MODIF_STOCK_CARBURANT = 7;

	public static readonly int ID_CAPACITE_MODIF_CONSOMATION_CARBURANT = 8;

	public static readonly int ID_CAPACITE_MODIF_PV_MAX = 9;

	public static readonly int ID_CAPACITE_MODIF_PV = 10;

	public static readonly int ID_CAPACITE_MODIF_POINT_ATTAQUE = 11;

	public static readonly int ID_CAPACITE_MODIF_DEGAT_INFLIGE = 12;

	public static readonly int ID_CAPACITE_MODIF_PUISSANCE_AMELIORATION = 13;

	public static readonly int ID_CAPACITE_MODIF_NB_CARTE_PIOCHE = 14;

	//Peut attaquer planete sans activer defense
	public static readonly int ID_CAPACITE_ETAT_FURTIF = 15;
	//Peut attaquer au retour d'une attaque
	public static readonly int ID_CAPACITE_ETAT_ATTAQUE_RETOUR = 16;
	//Peut attaquer 2 fois ou plus TODO conserne defense aussi?
	public static readonly int ID_CAPACITE_MODIF_NB_ATTAQUE = 17;
	//Ne peut plus déplacer carte
	public static readonly int ID_CAPACITE_ETAT_IMMOBILE =18;
	//Ne peut plus attaquer ou defendre
	public static readonly int ID_CAPACITE_ETAT_DESARME = 19;
	//Ne peut plus utiliser de capacite
	public static readonly int ID_CAPACITE_ETAT_SANS_EFFET = 20;
	//Carte effectue une action aleatoire lors du click sur le bouton de tour
	public static readonly int ID_CAPACITE_ETAT_HORS_DE_CONTROLE = 21;
	//Revele une carte cache
	public static readonly int ID_CAPACITE_REVELE_CARTE = 22;

	public static readonly int ID_CAPACITE_VOLE_CARTE = 23;

	public static readonly int ID_CAPACITE_PERTE_TOUR_JEU = 24;

	public static readonly int ID_CAPACITE_DISSIMULER_PLATEAU = 25;

	public static readonly int ID_CAPACITE_MODIF_NB_PLACE_PLATEAU = 26;

	public static readonly int ID_CAPACITE_MODIF_EMPLACEMENT_CARTE = 27;

	public static readonly int ID_CAPACITE_INVOQUE_CARTE = 28;

	public static readonly int ID_CAPACITE_REORIENTE_ATTAQUE = 29;

	public static readonly int ID_CAPACITE_MODIF_PRODUCTION_XP = 30;

	public static readonly int ID_CAPACITE_MODIF_TYPE_RESSOURCE = 31;

	public static readonly int ID_CAPACITE_DESTRUCTION_CARTE = 32;

	public static readonly int ID_CAPACITE_ETAT_INVULNERABLE = 33;

	public static readonly int ID_CAPACITE_EVITE_ATTAQUE = 34;
	//Attaque en premier et subis la défense uniquement si elle survis
	public static readonly int ID_CAPACITE_ATTAQUE_OPPORTUNITE = 35;

	public static readonly int ID_CAPACITE_VOL_RESSOURCE = 36;

	//public static readonly int ID_CAPACITE_VOL_CARBURANT = 37;
	//Carte non ciblalble
	public static readonly int ID_CAPACITE_ETAT_INVISIBLE = 38;
	//Capacite au hasard parmis toute la liste
	public static readonly int ID_CAPACITE_ACTION_HASARD = 39;

	public static readonly int ID_CAPACITE_CONDITION = 100;


	/*
	Pour lles ID_CONDTION_EMPLACEMENT, ID_CONDITION_ACTION et ID_CONDTION_CIBLE
	Le chiffre peut être suivi d'un "-" et d'une ou plusieurs lettre
	*/
	public static readonly string STR_CONDITION_POUR_ALLIER = "A";

	public static readonly string STR_CONDITION_POUR_ENNEMIE = "E";

	//condition conserne uniquement la carte de provenance de la capacité
	public static readonly string STR_CONDITION_POUR_PROVENANCE = "P";


	/*********************************ID_CONDTION_CIBLE**********************/

	public static readonly int ID_CONDITION_CIBLE_DECK_CONSTRUCTION = 1;

	public static readonly int ID_CONDITION_CIBLE_DECK_AMELIORATION = 2;

	public static readonly int ID_CONDITION_CIBLE_MAIN = 3;

	public static readonly int ID_CONDITION_CIBLE_CARTE_AMELIORATION = 4;

	public static readonly int ID_CONDITION_CIBLE_PLANETE = 5;

	public static readonly int ID_CONDITION_CIBLE_BATIMENT = 6;

	public static readonly int ID_CONDITION_CIBLE_DEFENSE = 7;

	public static readonly int ID_CONDITION_CIBLE_VAISSEAU = 8;

	public static readonly int ID_CONDITION_CIBLE_RESSOURCE = 9;

	public static readonly int ID_CONDITION_CIBLE_MODULE = 10;

	public static readonly int ID_CONDITION_CIBLE_SYSTEME = 11;

	public static readonly int ID_CONDITION_CIBLE_PLATEAU = 12;

	public static readonly int ID_CONDITION_CIBLE_DETERIORATION = 13;

	//public static readonly int ID_CONDITION_CIBLE_METAUX = 14;

	//public static readonly int ID_CONDITION_CIBLE_CARBURANT = 15;


	/*********************************ID_CONDTION_EMPLACEMENT**********************/

	public static readonly int ID_CONDITION_EMPLACEMENT_CIBLE = 1;

	public static readonly int ID_CONDITION_EMPLACEMENT_ADJACENT_VERTICAL = 2;

	public static readonly int ID_CONDITION_EMPLACEMENT_ADJACENT_HORIZONTAL = 3;

	public static readonly int ID_CONDITION_EMPLACEMENT_ADJACENT_DIAGONAL_HAUT_GAUCHE = 4;

	public static readonly int ID_CONDITION_EMPLACEMENT_ADJACENT_DIAGONAL_BAS_GAUCHE = 5;

	public static readonly int ID_CONDITION_EMPLACEMENT_LIGNE_HORIZONTAL = 6;

	public static readonly int ID_CONDITION_EMPLACEMENT_LIGNE_VERTICAL = 7;

	public static readonly int ID_CONDITION_EMPLACEMENT_LIGNE_ATTAQUANT = 8;

	public static readonly int ID_CONDITION_EMPLACEMENT_LIGNE_ATMOSPHERE = 9;

	public static readonly int ID_CONDITION_EMPLACEMENT_LIGNE_SOL = 10;

	public static readonly int ID_CONDITION_EMPLACEMENT_MAIN = 11;

	/**Agit sur la carte d ou provient la capacite**/
	//public static readonly int ID_CONDTION_EMPLACEMENT_CARTE_PROVENANCE_CAPACITE = 12;

	public static readonly int ID_CONDTION_EMPLACEMENT_DECK = 13;

	/**Existe dans la base de reference des cartes*/
	public static readonly int ID_CONDTION_EMPLACEMENT_HASARD = 14;

	public static readonly int ID_CONDITION_EMPLACEMENT_CARTE_PLANETE = 15;

	public static readonly int ID_CONDITION_EMPLACEMENT_RESSOURCE_METAL = 16;

	public static readonly int ID_CONDITION_EMPLACEMENT_RESSOURCE_CARBURANT = 17;

	public static readonly int ID_CONDITION_EMPLACEMENT_RESSOURCE_XP = 18;


	/**********************************ID_CONDITION_ACTION*************************/

	public static readonly int ID_CONDITION_ACTION_PIOCHE_CONSTRUCTION = 1;

	public static readonly int ID_CONDITION_ACTION_PIOCHE_AMELIORATION = 2;

	public static readonly int ID_CONDITION_ACTION_POSE_CONSTRUCTION = 3;

	public static readonly int ID_CONDITION_ACTION_POSE_AMELIORATION = 4;

	public static readonly int ID_CONDITION_ACTION_DEBUT_TOUR = 5;

	public static readonly int ID_CONDITION_ACTION_FIN_TOUR = 6;

	public static readonly int ID_CONDITION_ACTION_ATTAQUE = 7;

	public static readonly int ID_CONDITION_ACTION_DEFEND = 8;

	public static readonly int ID_CONDITION_ACTION_UTILISE = 9;

	public static readonly int ID_CONDITION_ACTION_DESTRUCTION_CARTE = 10;

	public static readonly int ID_CONDITION_ACTION_FIN_ATTAQUE = 11;

	public static readonly int ID_CONDITION_ACTION_GAIN_XP = 12;

	public static readonly int ID_CONDITION_ACTION_INVOCATION = 13;

	public static readonly int ID_CONDITION_ACTION_RECOIT_DEGAT = 14;

	public static readonly int ID_CONDITION_ACTION_DEPLACEMENT_LIGNE_ATTAQUE = 15;

	public static readonly int ID_CONDITION_ACTION_EVOLUTION_CARTE = 16;

	public static readonly int ID_CONDITION_ACTION_EFFET_CAPACITE = 17;

	public static readonly int ID_CONDITION_ACTION_DEPLACEMENT_STANDART = 18;
}
