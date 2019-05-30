using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTaskChoixCible : EventTask {

	[SyncVar]
	private SelectionCiblesExecutionCapacite selectionCibles;

	[SyncVar]
	private int actionOrigineCapacite;

	private List<ISelectionnable> listCiblesSelectionnes;

	public void initVariable(NetworkInstanceId origin, NetworkInstanceId joueur, int target, int typeEvent, bool createTaskBrother){
		this.originAction = origin;
		this.idSelectionnableTarget = target;
		this.typeEvent = ConstanteIdObjet.ID_CONDITION_ACTION_EFFET_CAPACITE;
		this.actionOrigineCapacite = typeEvent;
		this.joueur = joueur;

		listCiblesSelectionnes = new List<ISelectionnable> ();
	}

	/*public void Update(){
		base.Update ();
	}*/

	protected override void activateTask (){
		this.activate = true;
		if (SelectionCibles.ListIdCiblesProbables.Count > 0) {
			if (selectionCibles.ChoixCible) {
				RpcDisplayCapacityChoice ();
			} else {

				while (SelectionCibles.ListIdCiblesProbables.Count > selectionCibles.NbCible) {
					int indexToDelete = Random.Range (0, selectionCibles.ListIdCiblesProbables.Count);
					SelectionCibles.ListIdCiblesProbables.RemoveAt (indexToDelete);
				}

				ActionEventManager.EventActionManager.CmdExecuteCapacity (SelectionCibles, netId);

				finish = true;
			}
		} else {
			finish = true;
		}
	}

	protected override void launchEventAction (){
		//TODO annimation

		if (finish) {
			endOfTask ();
		}
	}

	[ClientRpc]	
	private void RpcDisplayCapacityChoice (){
		if (null != JoueurUtils.getJoueurLocal() && JoueurUtils.getJoueurLocal().netId == joueur) {
			List<ISelectionnable> listCibleSelectionnable = new List<ISelectionnable> ();
			foreach (int idCible in selectionCibles.ListIdCiblesProbables) {
				ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idCible);
				if (null != cible) {
					cible.EtatSelectionnable = SelectionnableUtils.ETAT_SELECTIONNABLE;
					listCibleSelectionnable.Add (cible);
				}
			}

			StartCoroutine(phaseChoixCible(listCibleSelectionnable));
		} else {
			//TODO mise en attente
			//TODO coroutine pour fin de mise en attente
		}
	}

	public IEnumerator phaseChoixCible(List<ISelectionnable> listCibleSelectionnable){
		float tempsDecision = 30;
		//La phase dure 30 secondes ou jusqu' demande d'arret ou si le nombre de carte selectionne est correct
		while (tempsDecision > 0 && !finish && listCiblesSelectionnes.Count < selectionCibles.NbCible) {
			tempsDecision -= .25f;
			yield return new WaitForSeconds (.25f);
		}

		finish = true;

		//TODO vérifier listCiblesSelectionnes présent dans cibleProbale
		if (listCiblesSelectionnes.Count > 0) {
			selectionCibles.ListIdCiblesProbables.Clear ();

			if (selectionCibles.IdCapaciteSource > 0 && selectionCibles.IdTypeCapacite > 0) {
				//Cas où l'on choisi les cibles d'une capacité
				foreach (ISelectionnable cible in listCiblesSelectionnes) {
					selectionCibles.ListIdCiblesProbables.Add(cible.IdISelectionnable);
				}

				ActionEventManager.EventActionManager.CmdExecuteCapacity (selectionCibles, netId);
			} else {
				NetworkBehaviour netBehaviour = ConvertUtils.convertNetIdToScript<NetworkBehaviour> (originAction, true);
				ISelectionnable cible = SelectionnableUtils.getSelectiobleById (this.idSelectionnableTarget);

				if (typeEvent == ConstanteIdObjet.ID_CONDITION_ACTION_DEFEND && netBehaviour is CartePlaneteMetier && cible is CarteVaisseauMetier) {
					//Cas où le choix de cible provient de la défense d'une plante
					foreach (ISelectionnable selection in listCiblesSelectionnes) {
						if (selection is IDefendre) {
							((IDefendre) selection).preDefense((CarteVaisseauMetier) cible, netId);
						}
					}
				}

			}



			//TODO miseHorsBrillance toute les cartes
		}
	}

	[ClientRpc]
	public void RpcInitSelection(SelectionCiblesExecutionCapacite selectCibleServer){
		selectionCibles = selectCibleServer;
	}


	public SelectionCiblesExecutionCapacite SelectionCibles { 
		get { return selectionCibles; }
		set { 
			selectionCibles = value;

			if (selectionCibles.IdCapaciteSource > 0 && selectionCibles.IdTypeCapacite > 0) {
				this.typeEvent = ConstanteIdObjet.ID_CONDITION_ACTION_EFFET_CAPACITE;
			} else {
				//Cas où le choix ne vienne pas d'une capacité
				this.typeEvent = selectionCibles.IdActionAppelante;
			}
		}
	}

	public List<ISelectionnable> ListCibleChoisie{
		get {return this.listCiblesSelectionnes;}
	}
}
