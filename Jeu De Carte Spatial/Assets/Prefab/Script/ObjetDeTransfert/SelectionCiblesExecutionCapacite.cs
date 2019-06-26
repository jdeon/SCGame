using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct SelectionCiblesExecutionCapacite {

	private string libelleCapacite;
	private int idTypeCapacite;
	private int idCapaciteSource;
	private bool choixManuelle;
	private int nbChoixCible;

	private int idActionAppelante;

	private NetworkInstanceId idCarteSource;

	private NetworkInstanceId idJoueurCarteSource;

	private List<int> listIdCiblesProbables;

	private List<int> listIdRessouceCible;

	//CARTE INVOQUER
	//Niv CARTE INVOQUER

	public SelectionCiblesExecutionCapacite (CapaciteDTO capacite, CarteMetierAbstract carteSource, int idActionAppelante){
		this.idTypeCapacite = capacite.Capacite;
		this.libelleCapacite = capacite.Nom; // TODO rajouter fonction descriptive de action
		this.choixManuelle = capacite.ChoixCible;
		this.nbChoixCible = capacite.NbCible;

		int newIdCapacityInUse = ActionEventManager.sequenceCapacityInUse++;
		this.idCapaciteSource = newIdCapacityInUse;
		ActionEventManager.capacityInUse.Add (newIdCapacityInUse, capacite.Clone());

		//this.capaciteBase = capacite.Clone ();
		this.idActionAppelante = idActionAppelante;
		this.idCarteSource = carteSource.netId;
		this.idJoueurCarteSource = carteSource.JoueurProprietaire.netId;
		this.listIdCiblesProbables = new List<int> ();

		this.listIdRessouceCible = null;
	}

	//Constructeur pour le choix de cible sans capacite
	public SelectionCiblesExecutionCapacite (int nbCibleMax, CarteMetierAbstract carteSource, int idActionAppelante){
		this.idTypeCapacite = -1;
		this.idCapaciteSource = -1;
		this.libelleCapacite = "Action type" + idActionAppelante; // TODO rajouter fonction descriptive de action
		this.choixManuelle = true;
		this.nbChoixCible = nbCibleMax;

		this.idActionAppelante = idActionAppelante;
		this.idCarteSource = carteSource.netId;
		this.idJoueurCarteSource = carteSource.JoueurProprietaire.netId;
		this.listIdCiblesProbables = new List<int> ();

		this.listIdRessouceCible = null;
	}

	public void initModeRessourceCapa(List<RessourceMetier> listeRessourceMetier){
		this.listIdRessouceCible = new List<int> ();

		foreach(RessourceMetier ressourceMetier in listeRessourceMetier) {
			this.listIdRessouceCible.Add(ressourceMetier.IdISelectionnable);
		}
	}

	public void initSelectionForClient(List<int> listCibleProbable, int nbChoixMax, int idCapacite, int idTypeCapacite){
		this.listIdCiblesProbables = listCibleProbable;
		this.nbChoixCible = nbChoixMax;
		this.idCapaciteSource = idCapacite;
		this.idTypeCapacite = idTypeCapacite;
	}

	public int IdTypeCapacite {
		get{ return idTypeCapacite; }
	}

	public bool ChoixCible{
		get{ return choixManuelle; }
	}

	public int NbCible{
		get{ return nbChoixCible; }
	}

	public NetworkInstanceId IdCarteSource{
		get{ return idCarteSource; }
	}

	public NetworkInstanceId IdJoueurCarteSource{
		get{ return idJoueurCarteSource; }
	}

	public int IdCapaciteSource{
		get{ return idCapaciteSource; }
	}


	public List<int> ListIdCiblesProbables{
		get{ return listIdCiblesProbables; }
	}


	public int IdActionAppelante{
		get{ return idActionAppelante; }
	}

	public List<int> ListIdRessouceCible {
		get{ return listIdRessouceCible; }
	}
}
