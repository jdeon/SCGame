using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTaskChoixCible : EventTask {

	private SelectionCiblesExecutionCapacite selectionCibles;

	private int actionOrigineCapacite;

	private List<ISelectionnable> listCiblesSelectionnes;

	public void initVariable(NetworkInstanceId origin, NetworkInstanceId joueur, int target, int typeEvent){
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

	protected new void activateTask (){
		base.activateTask ();
		if (selectionCibles.ListIdCiblesProbables.Count > 0) {
			if (selectionCibles.ChoixCible) {
				displayCapacityChoice ();
			} else {

				while (selectionCibles.ListIdCiblesProbables.Count > selectionCibles.NbCible) {
					int indexToDelete = Random.Range (0, selectionCibles.ListIdCiblesProbables.Count);
					selectionCibles.ListIdCiblesProbables.RemoveAt (indexToDelete);
				}

				ActionEventManager.EventActionManager.CmdExecuteCapacity (selectionCibles, netId);
			}
		}
	}
		
	private void displayCapacityChoice (){
		if (isLocalPlayer && JoueurUtils.getJoueurLocal().netId == joueur) {
			List<ISelectionnable> listCibleSelectionnable = new List<ISelectionnable> ();
			foreach (int idCible in selectionCibles.ListIdCiblesProbables) {
				ISelectionnable cible = SelectionnableUtils.getSelectiobleById (idCible);
				if (null != cible) {
					cible.miseEnBrillance (1);
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
		bool finChoix = false;
		//La phase dure 30 secondes ou jusqu' demande d'arret ou si le nombre de carte selectionne est correct
		while (tempsDecision > 0 && !finChoix && listCiblesSelectionnes.Count < selectionCibles.NbCible) {
			tempsDecision -= .25f;
			yield return new WaitForSeconds (.25f);
		}

		finChoix = true;

		//TODO vérifier listCiblesSelectionnes présent dans cibleProbale
		if (listCiblesSelectionnes.Count > 0) {
			selectionCibles.ListIdCiblesProbables.Clear ();
			foreach (ISelectionnable cible in listCiblesSelectionnes) {
				selectionCibles.ListIdCiblesProbables.Add(cible.IdISelectionnable);
			}
				
			ActionEventManager.EventActionManager.CmdExecuteCapacity (selectionCibles, netId);

			//TODO miseHorsBrillance toute les cartes
		}
	}

	public SelectionCiblesExecutionCapacite SelectionCibles{ get; set; }

	public List<ISelectionnable> ListCibleChoisie{
		get {return this.listCiblesSelectionnes;}
	}
}
