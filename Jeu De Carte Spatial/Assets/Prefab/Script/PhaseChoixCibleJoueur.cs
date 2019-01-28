using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PhaseChoixCibleJoueur : MonoBehaviour {

	private Joueur joueur;

	private SelectionCiblesExecutionCapacite selectionCibles;

	public bool finChoix = false;

	public List<ISelectionnable> listCibleChoisi;


	public PhaseChoixCibleJoueur (SelectionCiblesExecutionCapacite selectionCibles, Joueur joueur){

		this.selectionCibles = selectionCibles;
		this.joueur = joueur;
		this.finChoix = false;
		this.listCibleChoisi = new List<ISelectionnable>();

		foreach (ISelectionnable cible in selectionCibles.ListCiblesProbables) {
			cible.miseEnBrillance (1);
		}

		StartCoroutine(phaseChoixCible(selectionCibles));
	}
		

	public IEnumerator phaseChoixCible(SelectionCiblesExecutionCapacite selectionCiblesCapacite){
		List<ISelectionnable> listCibleChoisi = new List<ISelectionnable> ();
		float tempsDecision = 30;
		bool finChoix = false;
		//La phase dure 30 secondes ou jusqu' demande d'arret ou si le nombre de carte selectionne est correct
		while (tempsDecision > 0 && !finChoix && listCibleChoisi.Count < selectionCiblesCapacite.NbCible) {



			tempsDecision -= .25f;
			yield return new WaitForSeconds (.25f);
		}

		finChoix = true;

		if (listCibleChoisi.Count > 0) {
			selectionCiblesCapacite.ListCiblesProbables.Clear ();
			foreach (ISelectionnable cible in listCibleChoisi) {
				selectionCiblesCapacite.ListCiblesProbables.Add(cible);
			}

			byte[] selectionCibleByte = SerializeUtils.SerializeToByteArray (selectionCiblesCapacite);
			joueur.CmdExecuteCapaciteSurCibleChoisi (selectionCibleByte);

			//TODO miseHorsBrillance toute les cartes
		}
	}



}
