using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTask : NetworkBehaviour {

	protected NetworkInstanceId originAction;

	protected NetworkInstanceId joueur;

	protected int idSelectionnableTarget;

	protected int typeEvent;

	protected bool activate = false;

	protected bool pause = true;

	public void initVariable(NetworkInstanceId origin, NetworkInstanceId joueur, int target, int typeEvent){
		this.originAction = origin;
		this.idSelectionnableTarget = target;
		this.typeEvent = typeEvent;
		this.joueur = joueur;
	}

	public void Update(){
		if (isServer) {
			if (transform.GetSiblingIndex () == 0 && transform.childCount == 0) {
				if (this.activate) {
					this.pause = false;
				} else {
					activateTask ();
				}
			} else if (this.activate) {
				this.pause = true;
			}

			if (this.activate && !this.pause) {
				EventTaskUtils.launchEventAction (this.typeEvent, this.originAction, this.joueur, this.idSelectionnableTarget, this.netId);
				//TODO annimation 
				endOfTask();
			}
		}
	}

	protected void activateTask (){
		this.activate = true;
		EventTaskUtils.launchEventCapacity(this.typeEvent, this.originAction, this.joueur, this.idSelectionnableTarget, this.netId);
	}

	public void endOfTask(){
		GameObject goParent = transform.parent.gameObject;
		EventTask eventParent = goParent.GetComponent<EventTask> ();
		if (null != eventParent) { //appartient a une tache parent
			eventParent.CmdNextTask ();
		} else {
			CmdDestroyTask ();
		}
	}


	[Command]
	public void CmdNextTask(){
		if (transform.childCount > 0) {
			NetworkManager.Destroy (transform.GetChild (0).gameObject);
		} else {
			//TODO is finish
			//endOfTask ();
		}
	}

	[Command]
	public void CmdDestroyTask(){
		NetworkManager.Destroy (this.gameObject);
	}

	public bool EnCours {
		get {return this.activate && !this.pause;}
	}
}
