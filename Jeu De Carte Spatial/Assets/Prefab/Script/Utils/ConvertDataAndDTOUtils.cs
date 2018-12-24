using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertDataAndDTOUtils {

	public static CapaciteDTO convertCapaciteDataToDTO(CapaciteData capaciteData){
		CapaciteDTO capaciteDTOResult = new CapaciteDTO ();

		capaciteDTOResult.Nom = capaciteData.nom;
		capaciteDTOResult.Capacite = capaciteData.capacite;
		capaciteDTOResult.Quantite = capaciteData.quantite;
		capaciteDTOResult.ModeCalcul = capaciteData.typeCalcul;
		capaciteDTOResult.Specification = capaciteData.specification;
		capaciteDTOResult.AppelUnique = capaciteData.appelUnique;
		capaciteDTOResult.LierACarte = capaciteData.lierACarte;
		capaciteDTOResult.ChoixCible = capaciteData.choixCible;
		capaciteDTOResult.Duree = capaciteData.duree;

		capaciteDTOResult.ConditionsCible = new List<string> ();
		if (null != capaciteData.conditionCible) {
			foreach (string CCData in capaciteData.conditionCible) {
				capaciteDTOResult.ConditionsCible.Add (CCData);
			}
		}

		capaciteDTOResult.ConditionsEmplacement = new List<string> ();
		if (null != capaciteData.conditionEmplacement) {
			foreach (string CEData in capaciteData.conditionEmplacement) {
				capaciteDTOResult.ConditionsEmplacement.Add (CEData);
			}
		}

		capaciteDTOResult.ConditionsAction = new List<string> ();
		if (null != capaciteData.conditionAction) {
			foreach (string CAData in capaciteData.conditionAction) {
				capaciteDTOResult.ConditionsAction.Add (CAData);
			}
		}

		if(capaciteData is CapaciteInvocationModuleCibleData){
			capaciteDTOResult.CarteInvocation = convertCarteAmeliorationDataToDTO(((CapaciteInvocationModuleCibleData)capaciteData).moduleAInvoquer);
			capaciteDTOResult.ComportementSiModulSimilaire = ((CapaciteInvocationModuleCibleData)capaciteData).comportementSiModulSimilaire;
			capaciteDTOResult.NiveauInvocation = 1;
		} else if (capaciteData is CapaciteInvocationVaisseauData){
			capaciteDTOResult.CarteInvocation = convertCarteConstructionDataToDTO(((CapaciteInvocationVaisseauData)capaciteData).moduleAInvoquer);
			capaciteDTOResult.ComportementSiModulSimilaire = ConstanteEnum.TypeInvocation.ajouter;
			capaciteDTOResult.NiveauInvocation = ((CapaciteInvocationVaisseauData)capaciteData).niveauInvocation;
		} else {
			capaciteDTOResult.CarteInvocation = null;
			capaciteDTOResult.ComportementSiModulSimilaire = ConstanteEnum.TypeInvocation.ajouter;
			capaciteDTOResult.NiveauInvocation = 0;
		}

		return capaciteDTOResult;
	}


	public static CapaciteMannuelleDTO convertCapaciteManuelleDataToDTO(CapaciteMannuelleData capaciteMannuelleData){
		CapaciteMannuelleDTO capaciteMannuelleDTOResult = new CapaciteMannuelleDTO ();

		capaciteMannuelleDTOResult.TitreCarte = capaciteMannuelleData.titreCarte;
		capaciteMannuelleDTOResult.LibelleCarte = capaciteMannuelleData.libelleCarte;
		capaciteMannuelleDTOResult.CitationCarte = capaciteMannuelleData.citationCarte;
		capaciteMannuelleDTOResult.RemplaceAttaque = capaciteMannuelleData.remplaceAttaque;

		capaciteMannuelleDTOResult.PeriodeUtilisable = new List<string>();
		if (null != capaciteMannuelleData.periodeUtilisable) {
			foreach (string periodeUtilisable in capaciteMannuelleData.periodeUtilisable) {
				capaciteMannuelleDTOResult.PeriodeUtilisable.Add (periodeUtilisable);
			}
		}

		capaciteMannuelleDTOResult.CapaciteCondition = new List<CapaciteDTO>();
		if (null != capaciteMannuelleData.capaciteCondition) {
			foreach (CapaciteData condition in capaciteMannuelleData.capaciteCondition) {
				capaciteMannuelleDTOResult.CapaciteCondition.Add (convertCapaciteDataToDTO(condition));
			}
		}

		capaciteMannuelleDTOResult.CapaciteEffet = new List<CapaciteDTO>();
		if (null != capaciteMannuelleData.capaciteEffet) {
			foreach (CapaciteData effet in capaciteMannuelleData.capaciteEffet) {
				capaciteMannuelleDTOResult.CapaciteEffet.Add (convertCapaciteDataToDTO(effet));
			}
		}

		return capaciteMannuelleDTOResult;
	}


	public static NiveauDTO convertNiveauDataToDTO(NiveauData niveauData){
		NiveauDTO niveauDTOResult = new NiveauDTO ();

		niveauDTOResult.TitreNiveau = niveauData.titreNiveau;
		niveauDTOResult.DescriptionNiveau = niveauData.descriptionNiveau;
		niveauDTOResult.CitationNiveau = niveauData.citationNiveau;
		niveauDTOResult.Cout = niveauData.cout;

		niveauDTOResult.Capacite = new List<CapaciteDTO> ();
		if(null != niveauData.capacite){
			foreach(CapaciteData capaciteData in niveauData.capacite){
				if (null != capaciteData) {
					niveauDTOResult.Capacite.Add (convertCapaciteDataToDTO (capaciteData));
				} else {
					Debug.Log (niveauData);
					Debug.Log (capaciteData);
				}
			}
		}

		niveauDTOResult.NbCapaciteAuxChoix = niveauData.typeNbCapaciteAuxChoix;

		niveauDTOResult.ValeurTypeCapaciteAuxChoix = niveauData.valeurTypeCapaciteAuxChoix;

		niveauDTOResult.CapaciteManuelle = new List<CapaciteMannuelleDTO> ();
		if(null != niveauData.capaciteManuelle){
			foreach(CapaciteMannuelleData capaciteMannuelleData in niveauData.capaciteManuelle){
				niveauDTOResult.CapaciteManuelle.Add (convertCapaciteManuelleDataToDTO (capaciteMannuelleData));
			}
		}

		return niveauDTOResult;
	}

	public static CarteConstructionDTO convertCarteConstructionDataToDTO(CarteConstructionAbstractData carteConstructionData){
		CarteConstructionDTO carteConstructionDTO = new CarteConstructionDTO ();

		carteConstructionDTO.TitreCarte = carteConstructionData.titreCarte;
		carteConstructionDTO.LibelleCarte = carteConstructionData.libelleCarte;
		carteConstructionDTO.CitationCarte = carteConstructionData.citationCarte;

		carteConstructionDTO.ImagePath = ConstanteInGame.strImageCartePath + "/" + getDossierCarte(carteConstructionData) + "/" + carteConstructionData.name;

		carteConstructionDTO.NbTourAvantActif = carteConstructionData.nbTourAvantActif;
		carteConstructionDTO.PointVieMax = carteConstructionData.pointVieMax;
		carteConstructionDTO.ListNiveau = new List<NiveauDTO> ();

		if(null != carteConstructionData.listNiveau){
			foreach(NiveauData niveauData in carteConstructionData.listNiveau){
				carteConstructionDTO.ListNiveau.Add (convertNiveauDataToDTO (niveauData));
			}
		}

		if (carteConstructionData is CarteBatimentData) {
			carteConstructionDTO.PointAttaque = 0;
			carteConstructionDTO.ConsommationCarburant = 0;
			carteConstructionDTO.TypeOfCarte = ConstanteInGame.strBatiment;
		} else if (carteConstructionData is CarteDefenseData) {
			carteConstructionDTO.PointAttaque = ((CarteDefenseData)carteConstructionData).pointAttaque;
			carteConstructionDTO.ConsommationCarburant = 0;
			carteConstructionDTO.TypeOfCarte = ConstanteInGame.strDefense;
		} else if (carteConstructionData is CarteVaisseauData) {
			carteConstructionDTO.PointAttaque = ((CarteVaisseauData)carteConstructionData).pointAttaque;
			carteConstructionDTO.ConsommationCarburant = ((CarteVaisseauData)carteConstructionData).consommationCarburant;
			carteConstructionDTO.TypeOfCarte = ConstanteInGame.strVaisseau;
		} else {
			carteConstructionDTO.PointAttaque = 0;
			carteConstructionDTO.ConsommationCarburant = 0;
		}

		return carteConstructionDTO;
	}

	public static CarteAmeliorationDTO convertCarteAmeliorationDataToDTO(CarteAmeliorationData carteAmeliorationData){
		//TODO methode a faire
		return null;
	}

	private static string getDossierCarte(CarteAbstractData carteData){
		string nomDossier = "";

		if (carteData is CarteVaisseauData) {
			nomDossier = ConstanteInGame.strVaisseau;
		} else if (carteData is CarteDefenseData) {
			nomDossier = ConstanteInGame.strDefense;
		} else if (carteData is CarteBatimentData) {
			nomDossier = ConstanteInGame.strBatiment;
		} else if (carteData is CarteAmeliorationData) {
			nomDossier = ConstanteInGame.strAmelioration;
		} else if (carteData is CarteDeteriorationData) {
			nomDossier = ConstanteInGame.strDeterioration;
		}

		return nomDossier;
	}
}