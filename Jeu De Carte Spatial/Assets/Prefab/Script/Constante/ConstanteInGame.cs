using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstanteInGame {

	public static readonly float coefPlane = 10f;

	public static readonly float tempChoixDefense = 10f;



	public static readonly string strSlash = "/";

	public static readonly string strImageCartePath = "Sprite/CarteImage";

	public static readonly string strVaisseau = "Vaisseau";

	public static readonly string strDefense = "Defense";

	public static readonly string strBatiment = "Batiment";

	public static readonly string strAmelioration = "Amelioration";

	public static readonly string strDeterioration = "Deterioration";


	public static readonly string STR_TYPE_RESSOURCE_METAL = "Metal";

	public static readonly string STR_TYPE_RESSOURCE_CARBU = "Carburant";

	public static readonly string STR_TYPE_RESSOURCE_XP = "Xp";


	public static readonly string strSystemActionEvent = "SystemActionEvent";

	/**Nom composant fils de joueur*/
	public static readonly string strMainJoueur = "MainJoueur";

	public static readonly string strPiocheAmelioration = "PiocheAmelioration";

	public static readonly string strPiocheConstruction = "SystemActionEvent";



	public static readonly Sprite spriteBackgroundCarte = Resources.Load<Sprite>("Background");

	public static readonly Sprite spriteLvl1 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/1");

	public static readonly Sprite spriteLvl2 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/2");

	public static readonly Sprite spriteLvl3 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/3");

	public static readonly Sprite spriteLvl4 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/4");

	public static readonly Sprite spriteLvl5 = Resources.Load<Sprite>("UI button sample pack 1/Part 1/5");

	public static readonly Sprite spriteCroixCancel = Resources.Load<Sprite>("UI button sample pack 1/Part 1/Close");

	public static readonly Sprite spriteTest = Resources.Load<Sprite>("Sprite/testImage");

	public static readonly Sprite spriteConstructionImage = Resources.Load<Sprite>("Sprites/imageContainer-Centrer");

	/**
	 * Sprite pour carte batiment
	*/
	public static readonly Sprite spriteBatimentFond = Resources.Load<Sprite>("Sprites/cardBase_08-Jaune");

	public static readonly Sprite spriteBatimentCardreTitre = Resources.Load<Sprite>("Sprites/titleBarV2_06-Centrer");

	public static readonly Sprite spriteBatimentCardreRessource = Resources.Load<Sprite>("Sprites/titleBarV4_03-Centrer");

	public static readonly Sprite spriteBatimentCardreNiveau = Resources.Load<Sprite>("Sprites/cardDiamond_05-Centrer");

	public static readonly Sprite spriteBatimentCardreDescription = Resources.Load<Sprite>("Sprites/textBoxV2_04-Double");

	public static readonly Sprite spriteBatimentPointAttDef = Resources.Load<Sprite>("Sprites/goldOrb-Centrer");


	/**
	 * Sprite pour carte Defense
	*/
	public static readonly Sprite spriteDefenseFond = Resources.Load<Sprite>("Sprites/cardBase_09");

	public static readonly Sprite spriteDefenseCardreTitre = Resources.Load<Sprite>("Sprites/titleBarV2_02-Centrer");

	public static readonly Sprite spriteDefenseCardreRessource = Resources.Load<Sprite>("Sprites/titleBarV4_02-Centrer");

	public static readonly Sprite spriteDefenseCardreNiveau = Resources.Load<Sprite>("Sprites/cardDiamond_01-Centrer");

	public static readonly Sprite spriteDefenseCardreDescription = Resources.Load<Sprite>("Sprites/textBoxV2_02-Double");

	public static readonly Sprite spriteDefensePointAttDef = Resources.Load<Sprite>("Sprites/greenOrb-Centrer");

	/**
	 * Sprite pour carte vaisseau
	*/
	public static readonly Sprite spriteVaisseauFond = Resources.Load<Sprite>("Sprites/cardBase_08");

	public static readonly Sprite spriteVaisseauCardreTitre = Resources.Load<Sprite>("Sprites/titleBarV2_01-Centrer");

	public static readonly Sprite spriteVaisseauCardreRessource = Resources.Load<Sprite>("Sprites/titleBarV4_11-Centrer");

	public static readonly Sprite spriteVaisseauCardreNiveau = Resources.Load<Sprite>("Sprites/cardDiamond_02-Centrer");

	public static readonly Sprite spriteVaisseauCardreDescription = Resources.Load<Sprite>("Sprites/textBoxV2_06-Double");

	public static readonly Sprite spriteVaisseauPointAttDef = Resources.Load<Sprite>("Sprites/blueOrb-Centrer");


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

	public static readonly Material materialGlow = Resources.Load<Material>("Materials/MaterialGlow");

	public static readonly Material materialConstructionImage = Resources.Load<Material>("Materials/imageContainer-Centrer");

	public static readonly Material materialCarteBackground = Resources.Load<Material>("Materials/cardSpaceBackground_12");


	/**
	 * Materiaux pour carte batiment
	*/
	public static readonly Material materialBatimentFond = Resources.Load<Material>("Materials/cardBase_08-Jaune");

	public static readonly Material materialBatimentCardreTitre = Resources.Load<Material>("Materials/titleBarV2_06-Centrer");

	public static readonly Material materialBatimentCardreCarbrant = Resources.Load<Material>("Materials/titleBarV4_03-ReverseCentrer"); 

	public static readonly Material materialBatimentCardreMetal = Resources.Load<Material>("Materials/titleBarV4_03-Centrer");

	public static readonly Material materialBatimentCardreNiveau = Resources.Load<Material>("Materials/cardDiamond_05-Centrer");

	public static readonly Material materialBatimentCardreDescription = Resources.Load<Material>("Materials/textBoxV2_04-Double");

	public static readonly Material materialBatimentPointAttDef = Resources.Load<Material>("Materials/goldOrb-Centrer");


	/**
	 * Materiaux pour carte Defense
	*/
	public static readonly Material materialDefenseFond = Resources.Load<Material>("Materials/cardBase_09");

	public static readonly Material materialDefenseCardreTitre = Resources.Load<Material>("Materials/titleBarV2_02-Centrer");

	public static readonly Material materialDefenseCardreCarbrant = Resources.Load<Material>("Materials/titleBarV4_02-ReverseCentrer"); 

	public static readonly Material materialDefenseCardreMetal = Resources.Load<Material>("Materials/titleBarV4_02-Centrer");

	public static readonly Material materialDefenseCardreNiveau = Resources.Load<Material>("Materials/cardDiamond_01-Centrer");

	public static readonly Material materialDefenseCardreDescription = Resources.Load<Material>("Materials/textBoxV2_02-Double");

	public static readonly Material materialDefensePointAttDef = Resources.Load<Material>("Materials/greenOrb-Centrer");

	/**
	 * Materiaux pour carte vaisseau
	*/
	public static readonly Material materialVaisseauFond = Resources.Load<Material>("Materials/cardBase_08");

	public static readonly Material materialVaisseauCardreTitre = Resources.Load<Material>("Materials/titleBarV2_01-Centrer");

	public static readonly Material materialVaisseauCardreCarbrant = Resources.Load<Material>("Materials/titleBarV4_11-ReverseCentrer"); 

	public static readonly Material materialVaisseauCardreMetal = Resources.Load<Material>("Materials/titleBarV4_11-Centrer");

	public static readonly Material materialVaisseauCardreNiveau = Resources.Load<Material>("Materials/cardDiamond_02-Centrer");

	public static readonly Material materialVaisseauCardreDescription = Resources.Load<Material>("Materials/textBoxV2_06-Double");

	public static readonly Material materialVaisseauPointAttDef = Resources.Load<Material>("Materials/blueOrb-Centrer");




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
	public static readonly Vector4  propDesignTitre = new Vector4 (.5f, .15f, .95f, .25f);
	public static readonly Vector4  propDesignImage = new Vector4 (.5f, .35f, .75f, .30f);
	public static readonly Vector4  propDesignRessource = new Vector4 (.5f, .5f, .95f, .2f);
	public static readonly Vector4  propDesignListNiveaux = new Vector4 (.5f, .725f, .95f, .333f);
	//public static readonly Vector4  propDesignBouton = new Vector4 (.5f, .954f, .25f, .042f);
	public static readonly Vector4  propDesignPointAttaque = new Vector4 (.2f, .94f, .30f, .10f);
	public static readonly Vector4  propDesignPointDefense = new Vector4 (.8f, .94f, .30f, .10f);


	/******Propriété carte niveau 2****/
	/******Sous propriété de ressource****/
	public static readonly Vector4  propDesignMetalRessource = new Vector4 (.275f, .5f, .45f, .4f);
	public static readonly Vector4  propDesignNiveauRessource = new Vector4 (.5f, .5f,.25f, .8f);
	public static readonly Vector4  propDesignCarburantRessource = new Vector4 (.725f, .5f, .45f, .4f);
}
