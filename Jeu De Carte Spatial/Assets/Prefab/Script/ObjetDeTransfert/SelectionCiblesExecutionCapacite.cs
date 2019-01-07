using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectionCiblesExecutionCapacite : MonoBehaviour {

	protected CapaciteDTO capaciteBase;

	protected NetworkInstanceId idCarteSource;

	protected NetworkInstanceId idJoueurCarteSource;

	protected int idActionAppelante;

	protected List<ISelectionnable> listCiblesProbables;

	public SelectionCiblesExecutionCapacite (CapaciteDTO capacite, CarteMetierAbstract carteSource, int idActionAppelante){
		/*this.idTypeCapacite = capacite.Capacite;
		this.choixCible = capacite.ChoixCible;
		this.nbCible = capacite.NbCible;
		this.idCapaciteSource = capacite.Id;
		this.nbTour = capacite.Duree;
		this.valeurCalcul = capacite.Quantite;
		this.typeCalcul*/

		this.capaciteBase = capacite.Clone ();

		this.idActionAppelante = idActionAppelante;
		this.idCarteSource = carteSource.netId;
		this.idJoueurCarteSource = carteSource.getJoueurProprietaire().netId;
		this.listCiblesProbables = new List<ISelectionnable> ();
	}

	public int IdTypeCapacite {
		get{ return capaciteBase.Capacite; }
	}

	public bool ChoixCible{
		get{ return capaciteBase.ChoixCible; }
	}

	public int NbCible{
		get{ return capaciteBase.NbCible; }
	}

	public NetworkInstanceId IdCarteSource{
		get{ return idCarteSource; }
	}

	public NetworkInstanceId IdJoueurCarteSource{
		get{ return IdJoueurCarteSource; }
	}

	public int IdCapaciteSource{
		get{ return capaciteBase.Id; }
	}

	public List<ISelectionnable> ListCiblesProbables{
		get{ return listCiblesProbables; }
	}

	public CapaciteDTO Capacite {
		get{ return capaciteBase; }
	}

	public int IdActionAppelante{
		get{ return idActionAppelante; }
	}
}
