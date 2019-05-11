using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstanteInGame {

	public static readonly float coefPlane = 10f;

	public static readonly float tempChoixDefense = 10f;


	public static string strImageCartePath = "Sprite/CarteImage";

	public static string strVaisseau = "Vaisseau";

	public static string strDefense = "Defense";

	public static string strBatiment = "Batiment";

	public static string strAmelioration = "Amelioration";

	public static string strDeterioration = "Deterioration";

	public static string strSystemActionEvent = "SystemActionEvent";


	public static readonly Sprite spriteBackgroundCarte = Resources.Load<Sprite>("Background");

	public static readonly Sprite spriteLvl1 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/1");

	public static readonly Sprite spriteLvl2 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/2");

	public static readonly Sprite spriteLvl3 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/3");

	public static readonly Sprite spriteLvl4 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/4");

	public static readonly Sprite spriteLvl5 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/5");

	public static readonly Sprite spriteCroixCancel = Resources.Load<Sprite>("UI button sample pack 1/Part 1/Close");

	public static readonly Sprite spriteTest = Resources.Load<Sprite>("Sprite/testImage");


	public static readonly Font fontArial = Resources.GetBuiltinResource<Font>("Arial.ttf");

	public static readonly Font fontChintzy = Resources.Load<Font>("Font/chintzy");


	public static readonly Material matChintzy = Resources.Load<Material>("Materials/Material Text/Chintzy");




	public static readonly Shader shaderStandart = Shader.Find("Standard");


	public static readonly Color colorVaisseau = new Color (1, 0, 0, .75f);

	public static readonly Color colorDefense = new Color (0, 0, 1, .75f);

	public static readonly Color colorBatiment = new Color (0, 1, 0, .75f);


	public static readonly Vector3 rotationImage = new Vector3 (0, 180, 0);

	public static readonly Vector3 tailleCarte = new Vector3 (.1f, 1, .15f);


	public static readonly Material materialBackgroundCarte = Resources.Load<Material>("Materials/Material Carte/BackgroundMat");


	public static readonly GameObject planePrefab = Resources.Load<GameObject>("Basic/goPlane");

	public static readonly GameObject textPrefab = Resources.Load<GameObject>("Basic/goText");

	public static readonly GameObject emptyPrefab = Resources.Load<GameObject>("Basic/goEmpty");

	public static readonly GameObject carteVaisseauPrefab = Resources.Load<GameObject>("goCard/goVaisseau");

	public static readonly GameObject carteDefensePrefab = Resources.Load<GameObject>("goCard/goDefense");

	public static readonly GameObject carteBatimentPrefab = Resources.Load<GameObject>("goCard/goBatiment");

	public static readonly GameObject cartePlanetePrefab = Resources.Load<GameObject>("goCard/goPlanete");

	public static readonly GameObject eventTaskPrefab = Resources.Load<GameObject>("eventTask");

	public static readonly GameObject eventTaskChooseTargetPrefab = Resources.Load<GameObject>("eventTaskChooseTarget");



	/*************************Propriété en proportion sur le design des carte
	 * list de propriété d'élément en partant d'en haut à gauche par rapport au parent
	 * Vector4 (propotionPositionX, propotionPositionY, propotionLargeur, propotionHauteur)
	 * /

	/******Carte construction ******/

	/******Propriété carte niveau 1****/
	public static readonly Vector4  propBoutonRetour = new Vector4 (1f, 0f, .20f, .1f);
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
