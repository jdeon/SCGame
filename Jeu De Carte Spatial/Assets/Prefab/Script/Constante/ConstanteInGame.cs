using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstanteInGame {

	public static readonly Sprite spriteBackgroundCarte = Resources.Load<Sprite>("Background");

	public static readonly Font fontArial = Resources.GetBuiltinResource<Font>("Arial.ttf");

	public static readonly Color colorVaisseau = new Color (1, 0, 0, .75f);





	/*************************Propriété en proportion sur le design des carte
	 * list de propriété d'élément en partant d'en haut à gauche par rapport au parent
	 * Vector4 (propotionPositionX, propotionPositionY, propotionLargeur, propotionHauteur)
	 * /

	/******Carte construction ******/

	/******Propriété carte niveau 1****/
	public static readonly Vector4  propDesignTitre = new Vector4 (.5f, .058f, .95f, .083f);
	public static readonly Vector4  propDesignImage = new Vector4 (.5f, .271f, .95f, .292f);
	public static readonly Vector4  propDesignRessource = new Vector4 (.5f, .5f, .95f, .083f);
	public static readonly Vector4  propDesignListNiveaux = new Vector4 (.5f, .75f, .95f, .333f);
	public static readonly Vector4  propDesignBouton = new Vector4 (.5f, .954f, .25f, .042f);
	public static readonly Vector4  propDesignPointAttaque = new Vector4 (.075f, .967f, .125f, .05f);
	public static readonly Vector4  propDesignPointDefense = new Vector4 (.925f, .967f, .125f, .05f);


	/******Propriété carte niveau 2****/
	/******Sous propriété de ressource****/
	public static readonly Vector4  propDesignMetalRessource = new Vector4 (.224f, .5f, .263f, .4f);
	public static readonly Vector4  propDesignNiveauRessource = new Vector4 (.5f, .5f,.105f, .8f);
	public static readonly Vector4  propDesignCarburantRessource = new Vector4 (.776f, .5f, .263f, .4f);
}
